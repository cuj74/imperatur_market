using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;


namespace Imperatur.cache
{
    public class Quote : securities.Securities
    {
        public DateTime Loggedat;
        public string CompanyName;
    }

    public class TicketInfo
    {
        public string Ticket;
        public string CompanyName;
    }

    public static class Quotes
    {
        private static List<Quote> HistoricalQuotes;
        private static string _SystemDirectory;
        private static List<TicketInfo> TicketInfo;
 
        public static string SystemDirectory
        {
            get { return _SystemDirectory; }
            set { _SystemDirectory = value; }
        }
        public static List<Quote> GetQuotes
        {
            get { return ReadHistoricalQuotes(); }
        }

        private static List<TicketInfo> ReadTickets()
        {
            if (TicketInfo == null)
            {
                TicketInfo = new List<TicketInfo>();
                //load the stock quotes
                string line;

                // Read the file and display it line by line.
                using (StreamReader file = new StreamReader(SystemDirectory + @"\" + "ticks.txt"))
                {
                    int i = 0;
                    while ((line = file.ReadLine()) != null)
                    {
                        if (i > 0) //skip first line, is columnnames
                        {
                            char[] delimiters = new char[] { '\t' };
                            string[] parts = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length > 1)
                            {
                                TicketInfo.Add(new TicketInfo { Ticket = parts[1], CompanyName = parts[0] });
                                    //parts[1]);  //the ticket to fetch
                            }
                        }
                        i++;
                    }

                    file.Close();
                }
            }
            return TicketInfo;
        }
        private static void ReadQuotes(string QuoteFile)
        {
            using (StreamReader file = File.OpenText(string.Format(@QuoteFile, DateTime.Now.ToShortDateString())))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JArray o2 = (JArray)JToken.ReadFrom(reader);
                HistoricalQuotes = o2.ToObject<List<Quote>>();
            }
        }

        public static Quote GetQuote(string Ticker)
        {
            if (HistoricalQuotes == null)
            {
/*
 else
                {
                    HistoricalQuotes = ReadHistoricalQuotes();
                    using (FileStream fs = File.Open(string.Format(@QuoteFile, DateTime.Now.ToShortDateString()), FileMode.CreateNew))
                    using (StreamWriter sw = new StreamWriter(fs))
                    using (JsonTextWriter jw = new JsonTextWriter(sw))
                    {
                        jw.Formatting = Formatting.Indented;

                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Serialize(jw, HistoricalQuotes);
                    }
                }*/

                string QuoteFile;
                QuoteFile = string.Format(_SystemDirectory + @"\quotes\quotes.json{0}", DateTime.Now.ToShortDateString());
                if (File.Exists(@QuoteFile))
                {
                    ReadQuotes(QuoteFile);
                }
                else
                {
                    HistoricalQuotes = ReadHistoricalQuotes();
                    using (FileStream fs = File.Open(string.Format(@QuoteFile, DateTime.Now.ToShortDateString()), FileMode.CreateNew))
                    using (StreamWriter sw = new StreamWriter(fs))
                    using (JsonTextWriter jw = new JsonTextWriter(sw))
                    {
                        jw.Formatting = Formatting.Indented;

                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Serialize(jw, HistoricalQuotes);
                    }
                }
            }
            return HistoricalQuotes.Where(q => q.Symbol.Equals(Ticker)).Select(q => q).First();
        }
        private static List<Quote> ReadHistoricalQuotes()
        {
            List<Quote> HistoricalQuotesRet = new List<Quote>();

            //get the data for each ticket, this is were the Json kicks in
            string json;

            //kan även använda http://www.google.com/finance/info?q=NSE:AXIS,ABB
            rest.GenericRest oG = new rest.GenericRest();
            json = oG.GetResultFromURL("http://finance.google.com/finance/info?client=ig&q=NASDAQ%3A" + string.Join(",", ReadTickets().Select(q => q.Ticket)));

            //Google adds a comment before the json for some unknown reason, so we need to remove it
            json = json.Replace("//", "");

            var v = JArray.Parse(json);

            foreach (var i in v)
            {
                if (i.SelectToken("t") != null && Convert.ToDecimal(i.SelectToken("l")) != 0)
                {
                    HistoricalQuotesRet.Add(new Quote
                    {
                        Change = (Decimal)i.SelectToken("cp_fix"),
                        ChangePercent = (Decimal)i.SelectToken("cp"),
                        Dividend = new monetary.Money
                        {
                            Amount = Convert.ToDecimal(i.SelectToken("l")),
                            CurrencyCode = i.SelectToken("l_cur").ToString().Substring(0, 3)
                        },
                        DividendYield = new monetary.Money() { Amount = 1, CurrencyCode = "SEK" },
                        Exchange = "STO",
                        LastTradeDateTime = DateTime.Now,
                        Loggedat = DateTime.Now,
                        Symbol = i.SelectToken("t").ToString(),
                        CompanyName =
                        ReadTickets().Where(t => t.Ticket.Equals(i.SelectToken("t").ToString())).Count() > 0 ?
                        ReadTickets().Where(t => t.Ticket.Equals(i.SelectToken("t").ToString())).Select(t => t.CompanyName).First(): "N/A"
                    });
                }
            }


            return HistoricalQuotesRet;
        }

    }

    /*
    public class Quoteswww
    {
        private List<string> m_oTickets;
        public List<Quote> HistoricalQuotes;

        private List<Quote> ReadHistoricalQuotes()
        {
            List<Quote> HistoricalQuotesRet = new List<Quote>();
    
            //get the data for each ticket, this is were the Json kicks in
            string json;

            rest.GenericRest oG = new rest.GenericRest();
            json = oG.GetResultFromURL("http://finance.google.com/finance/info?client=ig&q=NASDAQ%3" + string.Join(",", m_oTickets.Select(q => q)));

            //Google adds a comment before the json for some unknown reason, so we need to remove it
            json = json.Replace("//", "");

            var v = JArray.Parse(json);

            foreach (var i in v)
            {
                if (i.SelectToken("t") != null && Convert.ToDecimal(i.SelectToken("l")) != 0)
                {
                    HistoricalQuotesRet.Add(new Quote
                    {
                        Change = (Decimal)i.SelectToken("cp_fix"),
                        ChangePercent = (Decimal)i.SelectToken("cp"),
                        Dividend = new monetary.Money
                        {
                            Amount = Convert.ToDecimal(i.SelectToken("l")),
                            CurrencyCode = i.SelectToken("l_cur").ToString().Substring(0, 3)
                        },
                        DividendYield = new monetary.Money() { Amount = 1, CurrencyCode = "SEK" },
                        Exchange = "STO",
                        LastTradeDateTime = DateTime.Now,
                        Loggedat = DateTime.Now,
                        Ticker = i.SelectToken("t").ToString()

                    });
                }


            }
            return HistoricalQuotesRet;
        }

        public Quotes(List<string> Tickets, string QuoteFile)
        {
            m_oTickets = Tickets;
            if (HistoricalQuotes == null)
            {
                if (File.Exists(string.Format(@QuoteFile, DateTime.Now.ToShortDateString())))
                {
                    ReadQuotes(QuoteFile);
                }
                else
                {
                    HistoricalQuotes = ReadHistoricalQuotes();
                    using (FileStream fs = File.Open(string.Format(@QuoteFile, DateTime.Now.ToShortDateString()), FileMode.CreateNew))
                    using (StreamWriter sw = new StreamWriter(fs))
                    using (JsonTextWriter jw = new JsonTextWriter(sw))
                    {
                        jw.Formatting = Formatting.Indented;

                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Serialize(jw, HistoricalQuotes);
                    }
                }
            }


        }

        private void ReadQuotes(string QuoteFile)
        {
            using (StreamReader file = File.OpenText(string.Format(@QuoteFile, DateTime.Now.ToShortDateString())))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JArray o2 = (JArray)JToken.ReadFrom(reader);
                HistoricalQuotes = o2.ToObject<List<Quote>>();
            }
        }
    }*/
}
