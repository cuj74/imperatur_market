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

namespace Imperatur_v2.handler
{
    public class TradeHandler : ITradeHandlerInterface
    {
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
            if (m_oQuotes != null)
            {
                //first try to read the file
                try {
                    m_oQuotes = (List<Quote>)DeserializeJSON.DeserializeObjectFromFile(string.Format(@"{0}\{1}\{2}{3}", ImperaturGlobal.SystemData.SystemDirectory, ImperaturGlobal.SystemData.QuoteDirectory, ImperaturGlobal.SystemData.QuoteFile, DateTime.Now.ToShortDateString()));
                }
                catch
                {
                    //read from external source
                    m_oQuotes = GetQuotesFromExternalSource(ImperaturGlobal.SystemData.ULR_Quotes);
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
                                ImperaturGlobal.Instruments.Select(i=>i.Symbol).ToArray()
                                ));

            //Google adds a comment before the json for some unknown reason, so we need to remove it
            json = json.Replace("//", "");

            var v = JArray.Parse(json);

            foreach (var i in v)
            {
                if (i.SelectToken("t") != null && Convert.ToDecimal(i.SelectToken("l")) != 0)
                {
                    QuotesRet.Add(new Quote
                    {
                        //Need to adjust to get the new variables, high, low and so on---
                        Change = (Decimal)i.SelectToken("cp_fix"),
                        ChangePercent = (Decimal)i.SelectToken("cp"),
                        DividendYield = new Money(1, new Currency(i.SelectToken("l_cur").ToString().Substring(0, 3))),
                        Loggedat = DateTime.Now,
                        Symbol = i.SelectToken("t").ToString(),
                    });
                }
            }


            return QuotesRet;
        }

        public bool ForceUpdate()
        {
            m_oQuotes = null; //next read will update the quote list
            return true;
        }
    }
    
}
