﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.handler;
using Imperatur_v2.monetary;
using Imperatur_v2.cache;
using Imperatur_v2.shared;
using Ninject;
using System.Reflection;
using System.IO;
using Imperatur_v2.json;
using Newtonsoft.Json.Linq;
using Imperatur_v2.account;
using Imperatur_v2.securites;
using Imperatur_v2.trade.analysis;
using Imperatur_v2.trade.recommendation;
using Imperatur_v2.order;
using Imperatur_v2.trade.automation;

namespace Imperatur_v2
{
    public static class ImperaturContainer
    {
        /// <summary>
        /// Build the Imperatur Market Container
        /// </summary>
        /// <param name="SystemLocation">The directory of the system</param>
        /// <returns></returns>
        public static IImperaturMarket BuildImperaturContainer(string SystemLocation)
        {
            //Ninject bindings
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());

            var ImpertaturContainer = kernel.Get<IImperaturMarket>(
                new Ninject.Parameters.ConstructorArgument("SystemLocation", SystemLocation)
                );

            return ImpertaturContainer;
        }
        public static IImperaturMarket BuildImperaturContainer(ImperaturData NewSystemData)
        {
            //Ninject bindings
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());

            var ImpertaturContainer = kernel.Get<IImperaturMarket>(
                new Ninject.Parameters.ConstructorArgument("SystemData", NewSystemData)
                );

            return ImpertaturContainer;
        }

    }
    /// <summary>
    /// IOC of Imperatur Market
    /// </summary>
    public interface IImperaturMarket
    {


        /// <summary>
        /// Returns the last known error message that has occurred
        /// </summary>
        /// <returns>string</returns>
        string GetLastErrorMessage();
        ImperaturData GetSystemData();
        IAccountHandlerInterface GetAccountHandler();
        ITradeHandlerInterface GetTradeHandler();
        IOrderQueue OrderQueue { get; }
        //IMoney GetMoney(decimal Amount, string CurrencyCode);
        event ImperaturMarket.QuoteUpdateHandler QuoteUpdateEvent;
        IMoney GetMoney(decimal Amount, string CurrencyCode);
        bool SetAutomaticTrading(bool Value);
        ExchangeStatus SystemExchangeStatus { get; }
        //List<TradingRecommendation> GetTradingRecommendation(string Symbol);


    }


    public class ImperaturMarket : IImperaturMarket
    {
        private IAccountHandlerInterface m_oAccountHandler;
        private ITradeHandlerInterface m_oTradeHandler;
        private ICurrency m_oDisplayCurrency;
        private IOrderQueue m_oOrderQueue;
        private string m_oLastErrorMessage;
        private ImperaturData m_oImperaturData;
        private readonly string SystemDataFile = "imperatursettings.json";
     
        private System.Timers.Timer m_oQuoteTimer;
        private TradeAutomation m_oTradeAutomation;


        //private List<Tuple<Instrument, List<TradingRecommendation>, VolumeIndicator, int>> m_oTradingOverview;

        public IOrderQueue OrderQueue
        {
            get
            {
                if (m_oOrderQueue == null)
                {
                    m_oOrderQueue = ImperaturGlobal.Kernel.Get<IOrderQueue>(
                         new Ninject.Parameters.ConstructorArgument("AccountHandler", GetAccountHandler()),
                         new Ninject.Parameters.ConstructorArgument("TradeHandler", GetTradeHandler())
                     );
                }
                return m_oOrderQueue;
            }
        }

        public ExchangeStatus SystemExchangeStatus
        {
            get
            {
                return ImperaturGlobal.ExchangeStatus;
            }
        }

        public delegate void QuoteUpdateHandler(object sender, EventArgs e);
        public event QuoteUpdateHandler QuoteUpdateEvent;

        #region proctectedMethods
        protected virtual void OnQuoteUpdate(EventArgs e)
        {
            if (QuoteUpdateEvent != null)
                QuoteUpdateEvent(this, e);
        }
        #endregion


        #region General
        public string GetLastErrorMessage()
        {
            return m_oLastErrorMessage ?? "";
        }
        #endregion


        #region constructor
        public ImperaturMarket(ImperaturData SystemData)
        {
            //create the system based on the data
            if (!Directory.Exists(SystemData.SystemDirectory))
            {
                CreateImperaturDataFromSystemData(SystemData);
            }
            CreateImperaturMarket(SystemData);

        }


        public ImperaturMarket(string SystemLocation)
        {
            CreateImperaturMarket(ReadImperaturDataFromSystemLocation(SystemLocation));
        }
        #endregion


        #region public methods
        public ImperaturData GetSystemData()
        {
            return m_oImperaturData;
        }

        public bool SetAutomaticTrading(bool Value)
        {
            bool SaveValue = false;
            if (m_oImperaturData.IsAutomaticMaintained != Value)
            {
                SaveValue = true;
            }
            m_oImperaturData.IsAutomaticMaintained = Value;
            if (SaveValue)
            {
                CreateSystemSettingsFile(m_oImperaturData);
            }
            
            return true;
        }

         public IAccountHandlerInterface GetAccountHandler()
        {
            if (m_oAccountHandler == null)
            {
                m_oAccountHandler = ImperaturGlobal.Kernel.Get<IAccountHandlerInterface>();
            }
            return m_oAccountHandler;
        }
        #endregion

        #region private methods

        private void M_oQuoteTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (ImperaturGlobal.ExchangeStatus != ExchangeStatus.Open)
            {
                return;
            }
            m_oTradeHandler.ForceUpdate();
            TradingRobotMain();
        }

        private ImperaturData ReadImperaturDataFromSystemLocation(string SystemLocation)
        {
            ImperaturData oD = new ImperaturData();
            try
            {
                oD = (ImperaturData)DeserializeJSON.DeserializeObjectFromFile(string.Format(@"{0}\{1}", SystemLocation, SystemDataFile));
            }
            catch (Exception ex)
            {
                int ff = 0;
            }
            return oD;
         }
        private bool CreateImperaturDataFromSystemData(ImperaturData Systemdata)
        {
            if (
                CreateDirectory(Systemdata.SystemDirectory)
                &&
                CreateDirectory(string.Format(@"{0}\{1}", Systemdata.SystemDirectory, Systemdata.AcccountDirectory))
                &&
                CreateDirectory(string.Format(@"{0}\{1}\{2}", Systemdata.SystemDirectory, Systemdata.QuoteDirectory, Systemdata.DailyQuoteDirectory))
                &&
                CreateDirectory(string.Format(@"{0}\{1}\{2}", Systemdata.SystemDirectory, Systemdata.QuoteDirectory, Systemdata.HistoricalQuoteDirectory))
                &&
                CreateDirectory(string.Format(@"{0}\{1}", Systemdata.SystemDirectory, Systemdata.OrderDirectory))
                &&
                CreateSystemSettingsFile(Systemdata)
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool CreateDirectory(String NewDirectoryToCreate)
        {
            try
            {
                Directory.CreateDirectory(NewDirectoryToCreate);
            }
            catch (Exception ex)
            {
                m_oLastErrorMessage = ex.Message;
                return false;
            }
            return true;
        }
        private bool CreateSystemSettingsFile(ImperaturData SystemData)
        {
            if (SerializeJSONdata.SerializeObject(SystemData, string.Format(@"{0}\{1}", SystemData.SystemDirectory, SystemDataFile)))
                return true;
            else
            {
                m_oLastErrorMessage = string.Format("Could not save settings file to {0}", SystemData.SystemDirectory);
                return false;
            }
        }

        private StandardKernel InitiateNinjectKernel()
        {
            try
            {
                StandardKernel kernel = new StandardKernel();
                kernel.Load(Assembly.GetExecutingAssembly());
                return kernel;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Could not initiate Imperatur Market application: {0}", ex.Message));
            }
        }
        private void CreateImperaturMarket(ImperaturData SystemData)
        {

            m_oImperaturData = SystemData;

            ImperaturGlobal.Initialize(m_oImperaturData, InitiateNinjectKernel(), null);
            List<account.AccountCacheType> BusinessAccounts = new List<account.AccountCacheType>();

            List<IAccountInterface> oLAB = new List<IAccountInterface>();
            if (GetAccountHandler().Accounts().Where(a => !a.GetAccountType().Equals(account.AccountType.Bank)).Count() == 0)
            {
                //create internalbankaccount for balancetransactions
                //start by create the bankaccount
                oLAB.Add(
                    ImperaturGlobal.Kernel.Get<IAccountInterface>(
                        new Ninject.Parameters.ConstructorArgument("Customer", (object)null),
                        new Ninject.Parameters.ConstructorArgument("AccountType", AccountType.Bank),
                        new Ninject.Parameters.ConstructorArgument("AccountName", "INTERNALBANK")
                    )
                    );
            }
            if (GetAccountHandler().Accounts().Where(a => !a.GetAccountType().Equals(account.AccountType.House)).Count() == 0)
            {
                //create internalbankaccount for balancetransactions
                //start by create the bankaccount
                oLAB.Add(
                    ImperaturGlobal.Kernel.Get<IAccountInterface>(
                        new Ninject.Parameters.ConstructorArgument("Customer", (object)null),
                        new Ninject.Parameters.ConstructorArgument("AccountType", AccountType.House),
                        new Ninject.Parameters.ConstructorArgument("AccountName", "INTERNALHOUSE")
                    )
                    );
            }
            GetAccountHandler().CreateAccount(oLAB);
            //add all business accounts to cache
            BusinessAccounts = GetAccountHandler().Accounts().Where(a => !a.GetAccountType().Equals(account.AccountType.Customer)).
                Select(b =>
                new account.AccountCacheType
                {
                    AccountType = b.GetAccountType(),
                    Identifier = b.Identifier
                }).ToList();

            ImperaturGlobal.InitializeBusinessAccount(BusinessAccounts);
            ImperaturGlobal.Quotes = GetTradeHandler().GetQuotes();

            m_oQuoteTimer = new System.Timers.Timer();
            m_oQuoteTimer.Elapsed += M_oQuoteTimer_Elapsed;
            m_oQuoteTimer.Interval = 1000 * 60 * 5; //Convert.ToInt32(m_oImperaturData.QuoteRefreshTime); //every 15 minutes
            m_oQuoteTimer.Enabled = true;

            m_oDisplayCurrency = ImperaturGlobal.Kernel.Get<ICurrency>(new Ninject.Parameters.ConstructorArgument("CurrencyCode", m_oImperaturData.SystemCurrency));
            m_oTradeHandler.QuoteUpdateEvent += M_oTradeHandler_QuoteUpdateEvent;

            m_oOrderQueue = null;
            m_oTradeAutomation = new TradeAutomation(GetAccountHandler());


            if (m_oImperaturData.IsAutomaticMaintained)
            {
                OrderQueue.EvaluateOrdersInQueue();
                TradingRobotMain();
              }

        }

        private void TradingRobotMain()
        {
            if (ImperaturGlobal.ExchangeStatus != ExchangeStatus.Open)
            {
                return;
            }

            foreach (IOrder oI in m_oTradeAutomation.RunTradeAutomation())
            {
                OrderQueue.AddOrder(oI);
            }
            OrderQueue.EvaluateOrdersInQueue();

            /*
            m_oTradingOverview = new List<Tuple<Instrument, List<TradingRecommendation>, VolumeIndicator,int>>();
            ISecurityAnalysis oS;
            RSSReader oR = new RSSReader();
            string[] SearchPlaces = new string[] {
                "http://investors.avanza.se/sv/feed/reports"
            };
            foreach (Instrument oI in ImperaturGlobal.Instruments)
            {
                oS = ImperaturGlobal.Kernel.Get<ISecurityAnalysis>(new Ninject.Parameters.ConstructorArgument("Instrument", oI));

                if (oS.HasValue)
                {
                    m_oTradingOverview.Add(new Tuple<Instrument, List<TradingRecommendation>, VolumeIndicator, int>(
                        oI,
                        oS.GetTradingRecommendations(),
                        oS.GetRangeOfVolumeIndicator(DateTime.Now.AddDays(-50), DateTime.Now).Last().Item2,
                        oR.GetOccurancesOfString(SearchPlaces, new string[] {oI.Name, oI.Symbol})
                        ));
                }
            }
            if (m_oTradingOverview.Count() == 0)
            {
                return;
            }

            //group the instruments by different values, like the search or the amount of buy or sell recommendations-
            var BuyRecommendations = m_oTradingOverview.Where(x => x.Item2.Where(tr => tr.TradingForecastMethod != TradingForecastMethod.Undefined && tr.BuyPrice.Amount > 0 && tr.SellPrice.Amount == 0).Count() > 0).ToList();
            var BuyCounts = BuyRecommendations.GroupBy(x => x.Item1.Symbol)
                      .Select(g => new { g.Key, Count = g.Count() });
            var SearchCounts = BuyRecommendations.GroupBy(x => x.Item1.Symbol)
                        .Select(g => new { g.Key, Sum = g.Sum(x=>x.Item4)});
            var VolumeIndicator = BuyRecommendations.Where(x=>!x.Item3.VolumeIndicatorType.Equals(VolumeIndicatorType.VolumeClimaxUp)).GroupBy(x => x.Item1.Symbol)
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


            var SellRecommendations = m_oTradingOverview.Where(x => x.Item2.Where(tr => tr.TradingForecastMethod != TradingForecastMethod.Undefined && tr.BuyPrice.Amount == 0 && tr.SellPrice.Amount > 0).Count() > 0).ToList();
            var SellCounts = SellRecommendations.GroupBy(x => x.Item1.Symbol)
                      .Select(g => new { g.Key, Count = g.Count() });
            var SellSearchCounts = SellRecommendations.GroupBy(x => x.Item1.Symbol)
                        .Select(g => new { g.Key, Sum = g.Sum(x => x.Item4) });
            var SellVolumeIndicator = SellRecommendations.Where(x => !x.Item3.VolumeIndicatorType.Equals(VolumeIndicatorType.VolumeClimaxDown)).GroupBy(x => x.Item1.Symbol)
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
                                        new Ninject.Parameters.ConstructorArgument("StopLossValidDays", 30),
                                        new Ninject.Parameters.ConstructorArgument("StopLossAmount", 0m),
                                        new Ninject.Parameters.ConstructorArgument("StopLossPercentage", 0.8m)
                                    )
                               };


            var ActualSellPositions = from b in SellDenseRanked
                                     join i in ImperaturGlobal.Instruments on b.Item.Key equals i.Symbol
                                     select new
                                     {
                                         SecAnalysis = ImperaturGlobal.Kernel.Get<ISecurityAnalysis>(new Ninject.Parameters.ConstructorArgument("Instrument", i)),
                                     };
            try
            {
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
                                                        new Ninject.Parameters.ConstructorArgument("m_oTradePriceValue", bp.SecAnalysis.QuoteFromInstrument.LastTradePrice),
                                                        new Ninject.Parameters.ConstructorArgument("m_oPercentageValue", 0m)
                                                                    )
                                                }),
                                                new Ninject.Parameters.ConstructorArgument("AccountIdentifier", ac.Identifier),
                                                new Ninject.Parameters.ConstructorArgument("Quantity",
                                                Convert.ToInt32(ac.GetHoldings().Where(h => h.Symbol.Equals(bp.SecAnalysis.Instrument.Symbol)).Sum(ho => ho.Quantity))
                                                ),
                                                new Ninject.Parameters.ConstructorArgument("OrderType", OrderType.Sell),
                                                new Ninject.Parameters.ConstructorArgument("ValidToDate", DateTime.Now.AddDays(2)),
                                                new Ninject.Parameters.ConstructorArgument("StopLossValidDays", 0),
                                                new Ninject.Parameters.ConstructorArgument("StopLossAmount", 0m),
                                                new Ninject.Parameters.ConstructorArgument("StopLossPercentage", 0m)
                                      )
                                       };

                if (ActualSellOrders != null && ActualSellOrders.Count() > 0)
                {
                    foreach (var o in ActualSellOrders)
                    {
                        OrderQueue.AddOrder(o.Order);
                    }
                }
                if (ActualBuyOrders != null && ActualBuyOrders.Count() > 0)
                {
                    foreach (var o in ActualBuyOrders)
                    {
                        OrderQueue.AddOrder(o.Order);
                    }
                }
            }
            catch (Exception ex)
            {
                int gg = 0;
            }
            /*
            foreach (IAccountInterface oA in m_oAccountHandler.Accounts().Where(a => a.GetAccountType().Equals(AccountType.Customer)))
            {
                //sell recommendations
                foreach (Tuple<Instrument, List<TradingRecommendation>, VolumeIndicator, int> oTR in m_oTradingOverview.Where(x => x.Item2.Where(tr => tr.TradingForecastMethod != TradingForecastMethod.Undefined && tr.SellPrice.Amount > 0 && tr.BuyPrice.Amount == 0).Count() > 0).ToList())
                {
                    //allow a loss of 2% from the original price!! Just to get the trading started!! Fix this later!
                    if (oA.GetHoldings().Count() > 0 && oA.GetHoldings().Where(h => h.Symbol != null && h.Symbol.Equals(oTR.Item1.Symbol)).Count() > 0 && oA.GetAverageAcquisitionCostFromHolding(oTR.Item1.Symbol).Multiply(1.02m).Amount >= oTR.Item2.Select(tr => tr.SellPrice.Amount).First())
                    {
                        if (oTR.Item3.VolumeIndicatorType == VolumeIndicatorType.LowVolume || oTR.Item3.VolumeIndicatorType == VolumeIndicatorType.Unknown
                            || oTR.Item3.VolumeIndicatorType == VolumeIndicatorType.VolumeClimaxUp)
                        {
                            oS = ImperaturGlobal.Kernel.Get<ISecurityAnalysis>(new Ninject.Parameters.ConstructorArgument("Instrument", oTR.Item1));
                            ITrigger NewTrigger = ImperaturGlobal.Kernel.Get<ITrigger>(
                                        new Ninject.Parameters.ConstructorArgument("m_oOperator", TriggerOperator.EqualOrGreater),
                                        new Ninject.Parameters.ConstructorArgument("m_oValueType", TriggerValueType.TradePrice),
                                        new Ninject.Parameters.ConstructorArgument("m_oTradePriceValue", oS.QuoteFromInstrument.LastTradePrice.Amount),
                                        new Ninject.Parameters.ConstructorArgument("m_oPercentageValue", 0m)
                                        );

                            IOrder NewOrder = ImperaturGlobal.Kernel.Get<IOrder>(
                                    new Ninject.Parameters.ConstructorArgument("Symbol", oS.Instrument.Symbol),
                                    new Ninject.Parameters.ConstructorArgument("Trigger", new List<ITrigger> { NewTrigger }),
                                    new Ninject.Parameters.ConstructorArgument("AccountIdentifier", oA.Identifier),
                                    new Ninject.Parameters.ConstructorArgument("Quantity", Convert.ToInt32(oA.GetHoldings().Where(h => h.Symbol.Equals(oTR.Item1.Symbol)).Sum(ho => ho.Quantity))),
                                    new Ninject.Parameters.ConstructorArgument("OrderType", OrderType.Sell),
                                    new Ninject.Parameters.ConstructorArgument("ValidToDate", DateTime.Now.AddDays(2)),
                                    new Ninject.Parameters.ConstructorArgument("StopLossValidDays", 0),
                                    new Ninject.Parameters.ConstructorArgument("StopLossAmount", 0m),
                                    new Ninject.Parameters.ConstructorArgument("StopLossPercentage", 0m)
                             );
                            OrderQueue.AddOrder(NewOrder);
                        }
                    }
                }




                //buy recommendations
                foreach (Tuple<Instrument, List<TradingRecommendation>, VolumeIndicator, int> oTR in m_oTradingOverview.Where(x => x.Item2.Where(tr => tr.TradingForecastMethod != TradingForecastMethod.Undefined && tr.BuyPrice.Amount > 0 && tr.SellPrice.Amount == 0).Count() > 0).ToList())
                {
                    oS = ImperaturGlobal.Kernel.Get<ISecurityAnalysis>(new Ninject.Parameters.ConstructorArgument("Instrument", oTR.Item1));
                    IMoney oAvailableFunds = oA.GetAvailableFunds(new List<ICurrency>() { ImperaturGlobal.GetMoney(0, oTR.Item1.CurrencyCode).CurrencyCode }).First();
                    if (oTR.Item2.Where(tr => tr.BuyPrice.Amount > 0 && tr.SellPrice.Amount == 0).Count() > 0 && (oTR.Item3.VolumeIndicatorType != VolumeIndicatorType.HighVolumeChurn || oTR.Item3.VolumeIndicatorType != VolumeIndicatorType.VolumeClimaxUp))
                    {
                        if (oAvailableFunds.Amount >= oS.QuoteFromInstrument.LastTradePrice.Amount)
                        {
                            int HighestQuantity = (int)(oAvailableFunds.Amount / oS.QuoteFromInstrument.LastTradePrice.Amount);
                            //add order
                            if (oTR.Item3.VolumeIndicatorType != VolumeIndicatorType.VolumeClimaxDown)
                            {
                                //add stoploss also

                                ITrigger NewTrigger = ImperaturGlobal.Kernel.Get<ITrigger>(
                                    new Ninject.Parameters.ConstructorArgument("m_oOperator", TriggerOperator.EqualOrless),
                                    new Ninject.Parameters.ConstructorArgument("m_oValueType", TriggerValueType.TradePrice),
                                    new Ninject.Parameters.ConstructorArgument("m_oTradePriceValue", oS.QuoteFromInstrument.LastTradePrice.Amount),
                                    new Ninject.Parameters.ConstructorArgument("m_oPercentageValue", 0m)
                                    );

                                IOrder NewOrder = ImperaturGlobal.Kernel.Get<IOrder>(
                                        new Ninject.Parameters.ConstructorArgument("Symbol", oS.Instrument.Symbol),
                                        new Ninject.Parameters.ConstructorArgument("Trigger", new List<ITrigger> { NewTrigger }),
                                        new Ninject.Parameters.ConstructorArgument("AccountIdentifier", oA.Identifier),
                                        new Ninject.Parameters.ConstructorArgument("Quantity", HighestQuantity),
                                        new Ninject.Parameters.ConstructorArgument("OrderType", OrderType.StopLoss),
                                        new Ninject.Parameters.ConstructorArgument("ValidToDate", DateTime.Now.AddDays(2)),
                                        new Ninject.Parameters.ConstructorArgument("StopLossValidDays", 30),
                                        new Ninject.Parameters.ConstructorArgument("StopLossAmount", 0m),
                                        new Ninject.Parameters.ConstructorArgument("StopLossPercentage", 0.8m)
                                 );
                                OrderQueue.AddOrder(NewOrder);
                            }
                            else
                            {
                                ITrigger NewTrigger = ImperaturGlobal.Kernel.Get<ITrigger>(
                                 new Ninject.Parameters.ConstructorArgument("m_oOperator", TriggerOperator.EqualOrless),
                                 new Ninject.Parameters.ConstructorArgument("m_oValueType", TriggerValueType.TradePrice),
                                 new Ninject.Parameters.ConstructorArgument("m_oTradePriceValue", oS.QuoteFromInstrument.LastTradePrice.Amount),
                                 new Ninject.Parameters.ConstructorArgument("m_oPercentageValue", 0m)
                                 );

                                IOrder NewOrder = ImperaturGlobal.Kernel.Get<IOrder>(
                                        new Ninject.Parameters.ConstructorArgument("Symbol", oS.Instrument.Symbol),
                                        new Ninject.Parameters.ConstructorArgument("Trigger", new List<ITrigger> { NewTrigger }),
                                        new Ninject.Parameters.ConstructorArgument("AccountIdentifier", oA.Identifier),
                                        new Ninject.Parameters.ConstructorArgument("Quantity", HighestQuantity),
                                        new Ninject.Parameters.ConstructorArgument("OrderType", OrderType.Buy),
                                        new Ninject.Parameters.ConstructorArgument("ValidToDate", DateTime.Now.AddDays(2)),
                                        new Ninject.Parameters.ConstructorArgument("StopLossValidDays", 0),
                                        new Ninject.Parameters.ConstructorArgument("StopLossAmount", 0m),
                                        new Ninject.Parameters.ConstructorArgument("StopLossPercentage", 0m)
                                 );
                                OrderQueue.AddOrder(NewOrder);
                            }
                        }
                    }
                }
            }
            if (m_oTradingOverview.Count() > 0)
            {
                ImperaturGlobal.TradingRecommendations = m_oTradingOverview.Select(tr => tr.Item2).SelectMany(x => x).Distinct().ToList();
            }



            //now, do an evalution of every holding and sell if profit above 1.5% and no recommendation exists
            foreach (IAccountInterface oA in m_oAccountHandler.Accounts().Where(a => a.GetAccountType().Equals(AccountType.Customer)))
            {
                var holdings = from h in oA.GetHoldings()
                               from tr in m_oTradingOverview.Where(x => x.Item2.Where(tr => tr.TradingForecastMethod != TradingForecastMethod.Undefined).Count() > 0).ToList().Where(t => t.Item1.Symbol.Equals(h.Symbol)).DefaultIfEmpty()
                               where h.ChangePercent > 1.5m
                               select h;
                foreach (var holding in holdings)
                {

                    oS = ImperaturGlobal.Kernel.Get<ISecurityAnalysis>(new Ninject.Parameters.ConstructorArgument("Instrument", ImperaturGlobal.Instruments.Where(i => i.Symbol.Equals(holding.Symbol)).First()));
                    ITrigger NewTrigger = ImperaturGlobal.Kernel.Get<ITrigger>(
                                new Ninject.Parameters.ConstructorArgument("m_oOperator", TriggerOperator.EqualOrGreater),
                                new Ninject.Parameters.ConstructorArgument("m_oValueType", TriggerValueType.TradePrice),
                                new Ninject.Parameters.ConstructorArgument("m_oTradePriceValue", oS.QuoteFromInstrument.LastTradePrice.Amount),
                                new Ninject.Parameters.ConstructorArgument("m_oPercentageValue", 0m)
                                );

                    IOrder NewOrder = ImperaturGlobal.Kernel.Get<IOrder>(
                            new Ninject.Parameters.ConstructorArgument("Symbol", oS.Instrument.Symbol),
                            new Ninject.Parameters.ConstructorArgument("Trigger", new List<ITrigger> { NewTrigger }),
                            new Ninject.Parameters.ConstructorArgument("AccountIdentifier", oA.Identifier),
                            new Ninject.Parameters.ConstructorArgument("Quantity", Convert.ToInt32(holding.Quantity)),
                            new Ninject.Parameters.ConstructorArgument("OrderType", OrderType.Sell),
                            new Ninject.Parameters.ConstructorArgument("ValidToDate", DateTime.Now.AddDays(2)),
                            new Ninject.Parameters.ConstructorArgument("StopLossValidDays", 0),
                            new Ninject.Parameters.ConstructorArgument("StopLossAmount", 0m),
                            new Ninject.Parameters.ConstructorArgument("StopLossPercentage", 0m)
                     );
                    OrderQueue.AddOrder(NewOrder);
                }

            }*/
            OrderQueue.EvaluateOrdersInQueue();
        }

        private void M_oTradeHandler_QuoteUpdateEvent(object sender, EventArgs e)
        {
            m_oTradeHandler.CacheQuotes();
            OnQuoteUpdate(e);
            ImperaturGlobal.Quotes = m_oTradeHandler.GetQuotes();
            //this is also the trading robot start. 
            if (m_oImperaturData.IsAutomaticMaintained)
            {
                OrderQueue.EvaluateOrdersInQueue();
                TradingRobotMain();
            }
        }

        public ITradeHandlerInterface GetTradeHandler()
        {
            if (m_oTradeHandler == null)
                m_oTradeHandler = ImperaturGlobal.Kernel.Get<ITradeHandlerInterface>();

            return m_oTradeHandler;
        }

        public IMoney GetMoney(decimal Amount, string CurrencyCode)
        {
            return ImperaturGlobal.GetMoney(Amount, CurrencyCode);
        }
/*
        public List<TradingRecommendation> GetTradingRecommendation(string Symbol)
        {
            if(m_oTradingOverview != null && m_oTradingOverview.Count() > 0 && m_oTradingOverview.Where(tr=>tr.Item1.Symbol.Equals(Symbol)).Count() > 0)
            {
                return m_oTradingOverview.Where(tr => tr.Item1.Symbol.Equals(Symbol)).Select(t=>t.Item2).First();
            }
            return null;
        }*/
        #endregion


    }
}
