using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.order;
using Imperatur_v2.shared;
using Imperatur_v2.trade.analysis;
using Imperatur_v2.trade.rss;
using Imperatur_v2.securites;
using Ninject;
using Imperatur_v2.trade.recommendation;
using Imperatur_v2.handler;
using Imperatur_v2.account;
using Imperatur_v2.monetary;
using Imperatur_v2.events;

namespace Imperatur_v2.trade.automation
{
    public class TradeAutomation : ITradeAutomation
    {
        private List<InstrumentRecommendation> m_oTradingOverview;
        private ISecurityAnalysis m_oSA;
        private RSSReader m_oRSS;
        private string[] SearchPlaces;
        private IAccountHandlerInterface m_oAccountHandler;
        private readonly string PROCESSNAME = "Trading automation process";

        public delegate void SystemNotificationHandler(object sender, IMPSystemNotificationEventArg e);

        public event ImperaturMarket.SystemNotificationHandler SystemNotificationEvent;


        protected virtual void OnSystemNotification(IMPSystemNotificationEventArg e)
        {
            if (SystemNotificationEvent != null)
                SystemNotificationEvent(this, e);
        }

        public TradeAutomation(IAccountHandlerInterface AccountHandler)
        {
            m_oTradingOverview = new List<InstrumentRecommendation>();
            //TODO, add this to an external resource
            SearchPlaces = new string[] {
                "http://investors.avanza.se/sv/feed/reports"
            };
            m_oRSS = new RSSReader();
            m_oAccountHandler = AccountHandler;

        }

        private List<InstrumentRecommendation> GetRecommendationFromInstruments(List<Instrument> Instruments)
        {
            List<InstrumentRecommendation> InstrumentRecommendations = new List<InstrumentRecommendation>();
            foreach (Instrument oI in Instruments)
            {
                OnSystemNotification(new IMPSystemNotificationEventArg
                {
                    Message = string.Format("{0} - combining trading info about {1}", PROCESSNAME, oI.Name)
                });

                m_oSA = ImperaturGlobal.Kernel.Get<ISecurityAnalysis>(new Ninject.Parameters.ConstructorArgument("Instrument", oI));

                if (m_oSA.HasValue && m_oSA.Instrument != null && m_oSA.QuoteFromInstrument != null)
                {
                    InstrumentRecommendations.Add(
                        new InstrumentRecommendation
                        {
                            InstrumentInfo = oI,
                            ExternalSearchHits = m_oRSS.GetOccurancesOfString(SearchPlaces, new string[] { oI.Name, oI.Symbol }),
                            VolumeIndication = m_oSA.GetRangeOfVolumeIndicator(DateTime.Now.AddDays(-50), DateTime.Now).Last().Item2,
                            TradingRecommendations = m_oSA.GetTradingRecommendations()
                        });
                }
            }
            return InstrumentRecommendations;
        }

        private List<IOrder> GetBuyOrdersFromTradingRecommencation(List<InstrumentRecommendation> TradingRecommendations)
        {
            var BuyRecommendations = TradingRecommendations.Where(x => x.TradingRecommendations.Where(tr => tr.TradingForecastMethod != TradingForecastMethod.Undefined && tr.BuyPrice.Amount > 0 && tr.SellPrice.Amount == 0).Count() > 0).ToList();
            var BuyForecastMethods= TradingRecommendations.Where(x => x.TradingRecommendations.Where(tr => tr.TradingForecastMethod != TradingForecastMethod.Undefined && tr.BuyPrice.Amount > 0 && tr.SellPrice.Amount == 0).Count() > 0)
                .Select( x=> new
                {
                    ForeCastMethod = x.TradingRecommendations.First().TradingForecastMethod,
                    Symbol = x.InstrumentInfo.Symbol
                }
                );

            var BuyCounts = BuyRecommendations.GroupBy(x => x.InstrumentInfo.Symbol)
                      .Select(g => new { g.Key, Count = g.Count() });
            var SearchCounts = BuyRecommendations.GroupBy(x => x.InstrumentInfo.Symbol)
                        .Select(g => new { g.Key, Sum = g.Sum(x => x.ExternalSearchHits) });
            var VolumeIndicator = BuyRecommendations.Where(x => !x.VolumeIndication.VolumeIndicatorType.Equals(VolumeIndicatorType.VolumeClimaxUp)).GroupBy(x => x.InstrumentInfo.Symbol)
                        .Select(g => new { g.Key, Count = g.Count() });

            var CondensedBuyData = from bc in BuyCounts
                                   join sc in SearchCounts on bc.Key equals sc.Key
                                   join vi in VolumeIndicator on bc.Key equals vi.Key
                                   select new
                                   {
                                       bc.Key,
                                       BuyCounts = bc.Count,
                                       SearchSum = sc.Sum,
                                       VolumeCount = vi.Count
                                   };


            var BuyDenseRanked = CondensedBuyData
            .GroupBy(rec => new { rec.Key, rec.SearchSum, rec.BuyCounts, rec.VolumeCount })
            .Where(@group => @group.Any())


            .OrderBy(@group => @group.Key.BuyCounts)
            .ThenBy(@group => @group.Key.VolumeCount)
            .ThenBy(@group => @group.Key.SearchSum)
            .AsEnumerable()


             .Select((@group, i) => new
             {
                 Items = @group,
                 Rank = ++i
             })
            .SelectMany(v => v.Items, (s, i) => new
            {
                Item = i,
                DenseRank = s.Rank
            }).ToList();

            var ActualBuyPositions = from b in BuyDenseRanked
                                     join i in ImperaturGlobal.Instruments on b.Item.Key equals i.Symbol
                                     select new
                                     {
                                         SecAnalysis = ImperaturGlobal.Kernel.Get<ISecurityAnalysis>(new Ninject.Parameters.ConstructorArgument("Instrument", i)),
                                     };
            var ActualBuyOrders = from ac in m_oAccountHandler.Accounts().Where(a => a.GetAccountType().Equals(AccountType.Customer))
                                  from bp in ActualBuyPositions
                                  where bp.SecAnalysis.HasValue && ac.GetAvailableFunds(
                                      new List<ICurrency> {
                                    ImperaturGlobal.GetMoney(0m, bp.SecAnalysis.Instrument.CurrencyCode).CurrencyCode
                                      }
                                      ).FirstOrDefault().GreaterOrEqualThan(bp.SecAnalysis.QuoteFromInstrument.LastTradePrice)
                                  select new
                                  {

                                      Order = ImperaturGlobal.Kernel.Get<IOrder>(
                                           new Ninject.Parameters.ConstructorArgument("Symbol", bp.SecAnalysis.Instrument.Symbol),
                                           new Ninject.Parameters.ConstructorArgument("Trigger", new List<ITrigger> {
                                           ImperaturGlobal.Kernel.Get<ITrigger>(
                                                                new Ninject.Parameters.ConstructorArgument("m_oOperator", TriggerOperator.EqualOrless),
                                                                new Ninject.Parameters.ConstructorArgument("m_oValueType", TriggerValueType.TradePrice),
                                                                new Ninject.Parameters.ConstructorArgument("m_oTradePriceValue", bp.SecAnalysis.QuoteFromInstrument.LastTradePrice.Amount),
                                                                new Ninject.Parameters.ConstructorArgument("m_oPercentageValue", 0m)
                                                                )
                                           }),
                                           new Ninject.Parameters.ConstructorArgument("AccountIdentifier", ac.Identifier),
                                           new Ninject.Parameters.ConstructorArgument("Quantity", (int)
                                           ac.GetAvailableFunds(
                                                  new List<ICurrency> {
                                                ImperaturGlobal.GetMoney(0m, bp.SecAnalysis.Instrument.CurrencyCode).CurrencyCode
                                                  }
                                                  ).FirstOrDefault().Divide(bp.SecAnalysis.QuoteFromInstrument.LastTradePrice).Amount
                                           ),
                                           new Ninject.Parameters.ConstructorArgument("OrderType", OrderType.StopLoss),
                                           new Ninject.Parameters.ConstructorArgument("ValidToDate", DateTime.Now.AddDays(2)),
                                           new Ninject.Parameters.ConstructorArgument("ProcessCode", BuyForecastMethods.Where(br=>br.Symbol.Equals(bp.SecAnalysis.Instrument.Symbol)).First().ForeCastMethod.ToString()),
                                           new Ninject.Parameters.ConstructorArgument("StopLossValidDays", 30),
                                           new Ninject.Parameters.ConstructorArgument("StopLossAmount", 0m),
                                           new Ninject.Parameters.ConstructorArgument("StopLossPercentage", 0.8m)
                                       )
                                  };
            return ActualBuyOrders.Select(i => i.Order).ToList();
        }

        private List<IOrder> GetSellOrdersFromTradingRecommencation(List<InstrumentRecommendation> TradingRecommendations)
        {
            var SellRecommendations = TradingRecommendations.Where(x => x.TradingRecommendations.Where(tr => tr.TradingForecastMethod != TradingForecastMethod.Undefined && tr.BuyPrice.Amount == 0 && tr.SellPrice.Amount > 0).Count() > 0).ToList();
            var SellForecastMethods = TradingRecommendations.Where(x => x.TradingRecommendations.Where(tr => tr.TradingForecastMethod != TradingForecastMethod.Undefined && tr.BuyPrice.Amount == 0 && tr.SellPrice.Amount > 0).Count() > 0)
            .Select(x => new
            {
                ForeCastMethod = x.TradingRecommendations.First().TradingForecastMethod,
                Symbol = x.InstrumentInfo.Symbol
            }
            );


            var SellCounts = SellRecommendations.GroupBy(x => x.InstrumentInfo.Symbol)
                      .Select(g => new { g.Key, Count = g.Count() });
            var SellSearchCounts = SellRecommendations.GroupBy(x => x.InstrumentInfo.Symbol)
                        .Select(g => new { g.Key, Sum = g.Sum(x => x.ExternalSearchHits) });
            var SellVolumeIndicator = SellRecommendations.Where(x => !x.VolumeIndication.VolumeIndicatorType.Equals(VolumeIndicatorType.VolumeClimaxDown)).GroupBy(x => x.InstrumentInfo.Symbol)
                        .Select(g => new { g.Key, Count = g.Count() });


            var CondensedSellData = from bc in SellCounts
                                    join sc in SellSearchCounts on bc.Key equals sc.Key
                                    join vi in SellVolumeIndicator on bc.Key equals vi.Key
                                    select new
                                    {
                                        bc.Key,
                                        SellCounts = bc.Count,
                                        SearchSum = sc.Sum,
                                        VolumeCount = vi.Count
                                    };


            var SellDenseRanked = CondensedSellData
            .GroupBy(rec => new { rec.Key, rec.SearchSum, rec.SellCounts, rec.VolumeCount })
            .Where(@group => @group.Any())


            .OrderBy(@group => @group.Key.SellCounts)
            .ThenBy(@group => @group.Key.VolumeCount)
            .ThenBy(@group => @group.Key.SearchSum)
            .AsEnumerable()


             .Select((@group, i) => new
             {
                 Items = @group,
                 Rank = ++i
             })
            .SelectMany(v => v.Items, (s, i) => new
            {
                Item = i,
                DenseRank = s.Rank
            }).ToList();

            var ActualSellPositions = from b in SellDenseRanked
                                      join i in ImperaturGlobal.Instruments on b.Item.Key equals i.Symbol
                                      select new
                                      {
                                          SecAnalysis = ImperaturGlobal.Kernel.Get<ISecurityAnalysis>(new Ninject.Parameters.ConstructorArgument("Instrument", i)),
                                      };
   
            var ActualSellOrders = from ac in m_oAccountHandler.Accounts().Where(a => a.GetAccountType().Equals(AccountType.Customer))
                                       from bp in ActualSellPositions
                                       where
                                         bp.SecAnalysis.HasValue
                                         &&
                                         ac.GetHoldings().Count() > 0
                                         &&
                                         ac.GetHoldings().Where(h => h.Symbol != null
                                         &&
                                         h.Symbol.Equals(bp.SecAnalysis.Instrument.Symbol)).Count() > 0
                                         &&
                                         ac.GetAverageAcquisitionCostFromHolding(bp.SecAnalysis.Instrument.Symbol).Multiply(1.02m).GreaterOrEqualThan(bp.SecAnalysis.QuoteFromInstrument.LastTradePrice)
                                       select new
                                       {
                                           Order = ImperaturGlobal.Kernel.Get<IOrder>(
                                                new Ninject.Parameters.ConstructorArgument("Symbol", bp.SecAnalysis.Instrument.Symbol),
                                                new Ninject.Parameters.ConstructorArgument("Trigger", new List<ITrigger> {
                                                ImperaturGlobal.Kernel.Get<ITrigger>(
                                                        new Ninject.Parameters.ConstructorArgument("m_oOperator", TriggerOperator.EqualOrGreater),
                                                        new Ninject.Parameters.ConstructorArgument("m_oValueType", TriggerValueType.TradePrice),
                                                        new Ninject.Parameters.ConstructorArgument("m_oTradePriceValue", bp.SecAnalysis.QuoteFromInstrument.LastTradePrice.Amount),
                                                        new Ninject.Parameters.ConstructorArgument("m_oPercentageValue", 0m)
                                                        )
                                                }),
                                                new Ninject.Parameters.ConstructorArgument("AccountIdentifier", ac.Identifier),
                                                new Ninject.Parameters.ConstructorArgument("Quantity",
                                                Convert.ToInt32(ac.GetHoldings().Where(h => h.Symbol.Equals(bp.SecAnalysis.Instrument.Symbol)).Sum(ho => ho.Quantity))
                                                ),
                                                new Ninject.Parameters.ConstructorArgument("OrderType", OrderType.Sell),
                                                new Ninject.Parameters.ConstructorArgument("ValidToDate", DateTime.Now.AddDays(2)),
                                                new Ninject.Parameters.ConstructorArgument("ProcessCode", SellForecastMethods.Where(br => br.Symbol.Equals(bp.SecAnalysis.Instrument.Symbol)).First().ForeCastMethod.ToString()),
                                                new Ninject.Parameters.ConstructorArgument("StopLossValidDays", 0),
                                                new Ninject.Parameters.ConstructorArgument("StopLossAmount", 0m),
                                                new Ninject.Parameters.ConstructorArgument("StopLossPercentage", 0m)
                                      )
                                       };
            return ActualSellOrders.Select(i => i.Order).ToList();
        }

        private List<IOrder> GetSellOrdersByProfit(List<InstrumentRecommendation> TradingRecommendations)
        {
            List<IOrder> NewOrders = new List<IOrder>();
            //now, do an evalution of every holding and sell if profit above 1.5% and no recommendation exists
            foreach (IAccountInterface oA in m_oAccountHandler.Accounts().Where(a => a.GetAccountType().Equals(AccountType.Customer)))
            {
                CreateSystemNoficationEvent(string.Format("Calculation sell orders by profit from {0}", oA.AccountName));
                if (oA.GetHoldings().Count() == 0)
                {
                    continue;
                }
                var holdings = from h in oA.GetHoldings()
                               from tr in TradingRecommendations.Where(x => x.TradingRecommendations.Where(tr => tr.TradingForecastMethod != TradingForecastMethod.Undefined).Count() > 0).ToList().Where(t => t.InstrumentInfo.Symbol.Equals(h.Symbol)).DefaultIfEmpty()
                               where h.ChangePercent > 1.5m
                               select h;
                foreach (var holding in holdings.Where(h=>h.Symbol != null))
                {
                    CreateSystemNoficationEvent(string.Format("creating sell orders by profit from {0} for {1}", oA.AccountName, holding.Symbol));
                    m_oSA = ImperaturGlobal.Kernel.Get<ISecurityAnalysis>(new Ninject.Parameters.ConstructorArgument("Instrument", ImperaturGlobal.Instruments.Where(i => i.Symbol.Equals(holding.Symbol)).First()));
                    ITrigger NewTrigger = ImperaturGlobal.Kernel.Get<ITrigger>(
                                new Ninject.Parameters.ConstructorArgument("m_oOperator", TriggerOperator.EqualOrGreater),
                                new Ninject.Parameters.ConstructorArgument("m_oValueType", TriggerValueType.TradePrice),
                                new Ninject.Parameters.ConstructorArgument("m_oTradePriceValue", m_oSA.QuoteFromInstrument.LastTradePrice.Amount),
                                new Ninject.Parameters.ConstructorArgument("m_oPercentageValue", 0m)
                                );

                    NewOrders.Add(ImperaturGlobal.Kernel.Get<IOrder>(
                            new Ninject.Parameters.ConstructorArgument("Symbol", m_oSA.Instrument.Symbol),
                            new Ninject.Parameters.ConstructorArgument("Trigger", new List<ITrigger> { NewTrigger }),
                            new Ninject.Parameters.ConstructorArgument("AccountIdentifier", oA.Identifier),
                            new Ninject.Parameters.ConstructorArgument("Quantity", Convert.ToInt32(holding.Quantity)),
                            new Ninject.Parameters.ConstructorArgument("OrderType", OrderType.Sell),
                            new Ninject.Parameters.ConstructorArgument("ValidToDate", DateTime.Now.AddDays(2)),
                            new Ninject.Parameters.ConstructorArgument("ProcessCode", "SellProfit"),
                            new Ninject.Parameters.ConstructorArgument("StopLossValidDays", 0),
                            new Ninject.Parameters.ConstructorArgument("StopLossAmount", 0m),
                            new Ninject.Parameters.ConstructorArgument("StopLossPercentage", 0m)
                     ));
  
                }

            }
            return NewOrders;
        }
        public List<IOrder> RunTradeAutomation()
        {
            if (ImperaturGlobal.ExchangeStatus != ExchangeStatus.Open)
            {
                return new List<IOrder>();
            }
            List<IOrder> NewOrders = new List<IOrder>();
            try
            {
                m_oTradingOverview = GetRecommendationFromInstruments(ImperaturGlobal.Instruments);

                if (m_oTradingOverview.Where(x => x.TradingRecommendations.Where(tr => !tr.TradingForecastMethod.Equals(TradingForecastMethod.Undefined)).Count() == 0).Count() > 0)
                {
                    return NewOrders;
                }
                CreateSystemNoficationEvent("creating new buy orders");
                NewOrders.AddRange(GetBuyOrdersFromTradingRecommencation(m_oTradingOverview));
                CreateSystemNoficationEvent("creating new sell orders");
                NewOrders.AddRange(GetSellOrdersFromTradingRecommencation(m_oTradingOverview));
                CreateSystemNoficationEvent("creating new sell orders based on profit of holding");
                NewOrders.AddRange(GetSellOrdersByProfit(m_oTradingOverview));
                if (m_oTradingOverview.Count() > 0)
                {
                    CreateSystemNoficationEvent("updating trade recommendations on instruments");
                    ImperaturGlobal.TradingRecommendations = m_oTradingOverview.Where(x => x.TradingRecommendations.Where(tr => !tr.TradingForecastMethod.Equals(TradingForecastMethod.Undefined)).Count() > 0).Select(tr => tr.TradingRecommendations).SelectMany(x => x).Distinct().ToList();
                }
            }
            catch(Exception ex)
            {
                CreateSystemNoficationEvent("process went wrong");
                ImperaturGlobal.GetLog().Error("Trading automation process went wrong", ex);
            }
            return NewOrders;
        }
        private void CreateSystemNoficationEvent(string Message)
        {
            OnSystemNotification(new IMPSystemNotificationEventArg
            {
                Message = string.Format("{0} - {1}", PROCESSNAME, Message)
            });

        }
    }
}
