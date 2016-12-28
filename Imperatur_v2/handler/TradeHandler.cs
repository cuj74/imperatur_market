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
using Ninject;

namespace Imperatur_v2.handler
{
    public class TradeHandler : ITradeHandlerInterface
    {

        //TODO: move the quotes to an class by itself. Remember SOLID
        private List<Quote> m_oQuotes;

        public Quote GetQuote(string Symbol)
        {
            return ReadQuotes().Where(q => q.Symbol.Equals(Symbol)).First();
        }


        public List<Quote> GetQuotes()
        {
            return ReadQuotes();
        }

        private List<Quote> ReadQuotes()
        {
            if (m_oQuotes == null)
            {
                //first try to read the file
                try
                {
                    //get file younger than 15 min
                    var files = from file in Directory.EnumerateFiles(string.Format(@"{0}\{1}\", ImperaturGlobal.SystemData.SystemDirectory, ImperaturGlobal.SystemData.QuoteDirectory), string.Format("{0}.*", ImperaturGlobal.SystemData.QuoteFile), SearchOption.TopDirectoryOnly)
                                from f in file
                                where Convert.ToDateTime(f.ToString().Replace(ImperaturGlobal.SystemData.QuoteFile, "")) > DateTime.Now.AddMinutes(-15)
                                select f;

                    if (files.Count() > 0)
                    {
                        m_oQuotes = (List<Quote>)DeserializeJSON.DeserializeObjectFromFile(@files.First().ToString());
                    }
                    else
                    {
                        m_oQuotes = GetQuotesFromExternalSource(ImperaturGlobal.SystemData.ULR_Quotes);
                        if (m_oQuotes.Count() > 0)
                        {
                            SerializeJSONdata.SerializeObject(m_oQuotes, string.Format(@"{0}\{1}\{2}{3}{4}", ImperaturGlobal.SystemData.SystemDirectory, ImperaturGlobal.SystemData.QuoteDirectory, ImperaturGlobal.SystemData.QuoteFile, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString().Replace(":", ";")));
                        }
                    }
                    //m_oQuotes = (List<Quote>)DeserializeJSON.DeserializeObjectFromFile(string.Format(@"{0}\{1}\{2}{3}", ImperaturGlobal.SystemData.SystemDirectory, ImperaturGlobal.SystemData.QuoteDirectory, ImperaturGlobal.SystemData.QuoteFile, DateTime.Now.ToShortDateString()));
                }
                catch (Exception ex)
                {
                    //read from external source
                    m_oQuotes = GetQuotesFromExternalSource(ImperaturGlobal.SystemData.ULR_Quotes);
                    //save if results obtained

                }

            }
            return m_oQuotes;
        }

        private List<Quote> GetQuotesFromExternalSource(string URL)
        {
            List<Quote> QuotesRet = new List<Quote>();
            string json;
            rest.Rest oG = new rest.Rest();
            json = oG.GetResultFromURL(URL + string.Join(",",
                                ImperaturGlobal.Instruments.Select(i => i.Symbol.Replace(" ", "-")).ToArray()
                                ));

            //Google adds a comment before the json for some unknown reason, so we need to remove it
            json = json.Replace("//", "");

            var v = JArray.Parse(json);

            foreach (var i in v)
            {
                if (i.SelectToken("t") != null && Convert.ToDecimal(i.SelectToken("l")) != 0)
                {
                    try
                    {
                        if (i.SelectToken("t") != null &&
                            ImperaturGlobal.Instruments.Where(ins => ins.Symbol.Replace(" ", "-").Equals(i.SelectToken("t").ToString())).Count() > 0)
                        {
                            QuotesRet.Add(new Quote
                            {
                                InternalLoggedat = DateTime.Now,
                                Symbol = i.SelectToken("t").ToString(),
                                Index = i.SelectToken("e").ToString(),
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
                        int gg = 0;
                    }
                }
            }



            return QuotesRet;
        }

        public bool ForceUpdate()
        {
            m_oQuotes = null; //next read will update the quote list
            return true;
        }

        public ITradeInterface GetTrade(string Symbol, decimal Quantity)
        {
            Quote oHoldingTicker = GetQuote(Symbol);
            IMoney TradeAmount = oHoldingTicker.LastTradePrice.Multiply(Quantity);
            Security NewSecurity = new Security()
            {
                Price = oHoldingTicker.LastTradePrice,
                Symbol = Symbol
            };

            ITradeInterface oNewTrade = ImperaturGlobal.Kernel.Get<ITradeInterface>(
             new Ninject.Parameters.ConstructorArgument("Quantity", Quantity),
             new Ninject.Parameters.ConstructorArgument("Security", NewSecurity),
             new Ninject.Parameters.ConstructorArgument("TradeAmount", TradeAmount)
             );

            return oNewTrade;
        }
    }
}
