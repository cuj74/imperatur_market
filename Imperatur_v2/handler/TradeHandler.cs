using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.securites;
using Imperatur_v2.json;
using Imperatur_v2.shared;
using Imperatur_v2.cache;
using Newtonsoft.Json.Linq;
using Imperatur_v2.monetary;
using System.IO;
using Imperatur_v2.trade;
using Imperatur_v2.trade.analysis;
using Ninject;

namespace Imperatur_v2.handler
{
    public class TradeHandler : ITradeHandlerInterface
    {
        private ISecurityAnalysis m_oSecurityAnalysis;
        //TODO: move the quotes to an class by itself. Remember SOLID
        private List<Quote> m_oQuotes;


        public event ImperaturMarket.QuoteUpdateHandler QuoteUpdateEvent;


        #region proctectedMethods
        protected virtual void OnQuoteUpdate(EventArgs e)
        {
            if (QuoteUpdateEvent != null)
                QuoteUpdateEvent(this, e);
        }
        #endregion

        public Quote GetQuote(string Symbol)
        {
            return ReadQuotes().Where(q => q.Symbol.Equals(Symbol)).First();
        }


        public List<Quote> GetQuotes()
        {
            return ReadQuotes();
        }

        private void UpdateQuotesFromExternalSource()
        {
            if (m_oQuotes == null)
            {
                //first try to read the file
                try
                {
                    string FileToRead = "";
                    foreach (string f in Directory.EnumerateFiles(string.Format(@"{0}\{1}\{2}", ImperaturGlobal.SystemData.SystemDirectory, ImperaturGlobal.SystemData.QuoteDirectory, ImperaturGlobal.SystemData.DailyQuoteDirectory), string.Format("{0}*", ImperaturGlobal.SystemData.QuoteFile), SearchOption.TopDirectoryOnly))
                    {
                        string time = f.Substring(f.Length - 5).Replace(";", ":");
                        string date = f.Substring(f.Length - 15).Substring(0, 10);
                        if (Convert.ToDateTime(string.Format("{0} {1}", date, time)).CompareTo(DateTime.Now.AddMinutes(-15)) > 0)
                        {
                            FileToRead = f;
                            break;
                        }
                    }


                    if (FileToRead != "")
                    {
                        m_oQuotes = (List<Quote>)DeserializeJSON.DeserializeObjectFromFile(@FileToRead.ToString());
                    }
                    else
                    {
                        m_oQuotes = GetQuotesFromExternalSource(ImperaturGlobal.SystemData.ULR_Quotes).Where(x=>x != null).ToList();
                        if (m_oQuotes.Count() > 0)
                        {
                            SerializeJSONdata.SerializeObject(m_oQuotes, string.Format(@"{0}\{1}\{2}\{3}{4}{5}", ImperaturGlobal.SystemData.SystemDirectory, ImperaturGlobal.SystemData.QuoteDirectory, ImperaturGlobal.SystemData.DailyQuoteDirectory, ImperaturGlobal.SystemData.QuoteFile, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString().Replace(":", ";")));
                        }
                    }
                }
                catch (Exception ex)
                {
                    //read from external source
                    m_oQuotes = GetQuotesFromExternalSource(ImperaturGlobal.SystemData.ULR_Quotes).Where(x => x != null).ToList();
                    //save if results obtained
                }
            }
        }

        private List<Quote> ReadQuotes()
        {
            UpdateQuotesFromExternalSource();
            return m_oQuotes;
        }

        private List<Quote> GetQuotesFromExternalSource(string URL)
        {
            //gör om till parallella körning med en i varje, annars blir det fel eftersom vi inbland får tillbaka fel exchange...
            List<Quote> QuotesRet = new List<Quote>();
            string[] AllSymbolsToRetrieve = ImperaturGlobal.Instruments.Select(i => i.Symbol.Replace(" ", "-")).ToArray();
            URL = URL.Replace("{exchange}", ImperaturGlobal.SystemData.Exchange);
            Parallel.For(0, AllSymbolsToRetrieve.Length - 1, new ParallelOptions { MaxDegreeOfParallelism = 100 },
            i =>
            {
                QuotesRet.AddRange(GetQuotesFromExternalSource(URL, AllSymbolsToRetrieve[i]));
             });
            return QuotesRet;
        }



        private List<Quote> GetQuoteInfoFromString(string QuoteInfoInJson)
        {
            List<Quote> QuotesRet = new List<Quote>();
            var v = JArray.Parse(QuoteInfoInJson);

            foreach (var i in v)
            {
                if (i.SelectToken("t") != null && Convert.ToDecimal(i.SelectToken("l")) != 0)
                {
                    try
                    {
                        if (i.SelectToken("t") != null
                            &&
                            ImperaturGlobal.Instruments.Where(ins => ins.Symbol.Replace(" ", "-").Equals(i.SelectToken("t").ToString())).Count() > 0
                            &&
                            i.SelectToken("e").ToString().Equals(ImperaturGlobal.SystemData.Exchange) //Only the correct Exchange
                            )
                        {
                            //todo, fix the conversion of decimals to be more stable!
                            QuotesRet.Add(new Quote
                            {
                                Change = ImperaturGlobal.GetMoney(
                                Convert.ToDecimal(Convert.ToDecimal(i.SelectToken("c").ToString().Replace(".", ","))), i.SelectToken("l_cur").ToString().Substring(0, 3)
                                ),
                                ChangePercent = Convert.ToDecimal(i.SelectToken("cp").ToString().Replace(".", ",")),
                                InternalLoggedat = DateTime.Now,
                                Symbol = i.SelectToken("t").ToString().Replace("-", " "),
                                Exchange = i.SelectToken("e").ToString(),
                                LastTradeDateTime = Convert.ToDateTime(i.SelectToken("lt_dts").ToString()),
                                LastTradePrice = ImperaturGlobal.GetMoney(
                                Convert.ToDecimal(i.SelectToken("l")), i.SelectToken("l_cur").ToString().Substring(0, 3)
                                ),
                                LastTradeSize = Convert.ToInt32(i.SelectToken("s").ToString()),
                                PreviousClosePrice = ImperaturGlobal.GetMoney(
                                Convert.ToDecimal(i.SelectToken("pcls_fix")), i.SelectToken("l_cur").ToString().Substring(0, 3)

                               )
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        ImperaturGlobal.GetLog().Error(string.Format("Error when retreiving quote data in GetQuotesFromExternalSource"), ex);
                    }
                }
            }
            return QuotesRet;
        }
        //string.Join(",", SymbolsToRetrieve.ToArray()));

        private List<Quote> GetQuotesFromExternalSource(string URL, string SymbolsToRetrieve)
        {
            rest.Rest oG = new rest.Rest();
            string json = oG.GetResultFromURL(URL + SymbolsToRetrieve);
            if (json == "")
            {
                return new List<Quote>();
            }
            json = json.Replace("//", "");
            return GetQuoteInfoFromString(json);
            
        }

        public bool ForceUpdate()
        {
            m_oQuotes = null; //next read will update the quote list
            OnQuoteUpdate(new EventArgs());
            return true;
        }

        public ITradeInterface GetTrade(string Symbol, decimal Quantity)
        {
            return GetTrade(Symbol, Quantity, DateTime.Now, null);
        }

        public ITradeInterface GetTrade(string Symbol, decimal Quantity, DateTime TradeDateTime, IMoney Revenue)
        {
            Quote oHoldingTicker = GetQuote(Symbol);
            IMoney TradeAmount = oHoldingTicker.LastTradePrice.Multiply(Quantity);
            Security NewSecurity = new Security()
            {
                Price = oHoldingTicker.LastTradePrice,
                Symbol = Symbol
            };

            ITradeInterface oNewTrade = ImperaturGlobal.Kernel.Get<ITradeInterface>(
             new Ninject.Parameters.ConstructorArgument("m_oQuantity", Quantity),
             new Ninject.Parameters.ConstructorArgument("m_oSecurity", NewSecurity),
             new Ninject.Parameters.ConstructorArgument("m_oTradeAmount", TradeAmount),
             new Ninject.Parameters.ConstructorArgument("m_oTradeDateTime", TradeDateTime),
             new Ninject.Parameters.ConstructorArgument("m_oRevenue", Revenue)
             );

            return oNewTrade;
        }

        public void CacheQuotes()
        {
            UpdateQuotesFromExternalSource();
        }

        public ISecurityAnalysis GetSecurityAnalysis(string Symbol)
        {
            if (ImperaturGlobal.Instruments.Where(i => i.Symbol.Equals(Symbol)).Count() > 0)
            {
                return GetSecurityAnalysis(ImperaturGlobal.Instruments.Where(i => i.Symbol.Equals(Symbol)).First());
            }
            else
            {
                throw new Exception("Instrument symbol doesn't exists");
            }
            
        }

        public ISecurityAnalysis GetSecurityAnalysis(Instrument Instrument)
        {
            if (m_oSecurityAnalysis != null && m_oSecurityAnalysis.Instrument.Equals(Instrument))
            {
                return m_oSecurityAnalysis;
            }
            else
            m_oSecurityAnalysis = ImperaturGlobal.Kernel.Get<ISecurityAnalysis>(
              new Ninject.Parameters.ConstructorArgument("Instrument", Instrument)
              );
            return m_oSecurityAnalysis;

        }
    }
}
