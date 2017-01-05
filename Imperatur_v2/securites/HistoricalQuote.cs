using Imperatur_v2.monetary;
using Imperatur_v2.shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_v2.securites
{
    public class HistoricalQuote : IHistoricalQuote
    {
        private Exchange m_oExchange;
        private Instrument m_oInstrument;
        private List<HistoricalQuoteDetails> m_oHistoricalQuoteDetails;

        public HistoricalQuote(
            Exchange Exchange,
            Instrument Instrument,
            List<HistoricalQuoteDetails> HistoricalQuoteDetails
            )
        {
            m_oExchange = Exchange;
            m_oInstrument = Instrument;
            m_oHistoricalQuoteDetails = HistoricalQuoteDetails;
        }
        public List<HistoricalQuoteDetails> HistoricalQuoteDetails
        {
            get
            {
                return m_oHistoricalQuoteDetails;
            }
        }

        public Exchange Exchange
        {
            get
            {
                return m_oExchange;
            }
        }

        public Instrument Instrument
        {
            get
            {
                return m_oInstrument;
            }
        }
    }

    public interface IHistoricalQuote
    {
        List<HistoricalQuoteDetails> HistoricalQuoteDetails { get; }
        Exchange Exchange { get; }
        Instrument Instrument { get; }
    }

    public class HistoricalQuoteDetails
    {
        public DateTime Date;
        public decimal Close;
        public decimal High;
        public decimal Low;
        public decimal Open;
        public int Volume;
    }

    public class Exchange
    {
        public string Region;
        public string ExhangeCode;
        public string Name;

    }
    public enum QuoteSupplier
    {
        Google
    }

    public class GoogleHistoricalDataInterpreter
    {
        private const string COLUMNS = "COLUMNS=";
        private const string INTERVAL = "INTERVAL=";
        private const string INTERVALDELIMITER = "a";
        private const string TIMEZONE_OFFSET = "TIMEZONE_OFFSET";

        public HistoricalQuote GetHistoricalData(Instrument Instrument, Exchange Exchange, DateTime FromDate, bool UseDate)
        {
            string Symbol = Instrument.Symbol.Replace(" ", "-");
            string URL = "";
            if (UseDate)
            {
                TimeSpan epochTicks = new TimeSpan(new DateTime(1970, 1, 1).Ticks);
                TimeSpan unixTicks = new TimeSpan(FromDate.Ticks) - epochTicks;
                
                URL = string.Format("http://www.google.com/finance/getprices?q={0}&x={1}&i=86400&p=40Y&ts={2}&f=d,c,v,k,o,h,l", Symbol, Exchange.ExhangeCode, unixTicks.TotalSeconds.ToString());
            }
            else
            {
                URL = string.Format("http://www.google.com/finance/getprices?q={0}&x={1}&i=86400&p=40Y&f=d,c,v,k,o,h,l", Symbol, Exchange.ExhangeCode);
            }
            string ResponseData = "";
            
            using (WebClient wc = new WebClient())
            {
                ResponseData = wc.DownloadString(URL);
            }

            if (ResponseData != "")
            {
                List<string> oColumnOrder = new List<string>();
                int TimeZoneOffset = 0;

                //Find line that starts with "COLUMNS"
                foreach (string ResponseLine in new LineReader(() => new StringReader(ResponseData)))
                {
                    if (ResponseLine.StartsWith(COLUMNS, false, System.Globalization.CultureInfo.CurrentCulture))
                    {
                        oColumnOrder = ResponseLine.Substring(COLUMNS.Length).Split(',').ToList();
                        break;
                    }
                }

                if (oColumnOrder.Count() == 0)
                {
                    return null;
                }
                //Find the interval
                int IntervalInSec = -1;
                foreach (string ResponseLine in new LineReader(() => new StringReader(ResponseData)))
                {
                    if (ResponseLine.StartsWith(INTERVAL, false, System.Globalization.CultureInfo.CurrentCulture))
                    {
                        IntervalInSec = Convert.ToInt32(ResponseLine.Substring(INTERVAL.Length));
                        break;
                    }
                }
                if (IntervalInSec == -1)
                {
                    return null;
                }

                List<HistoricalQuoteDetails> oIntervalData = new List<HistoricalQuoteDetails>();



                DateTime ResponseDateTime = DateTime.Now;
                int IntervalMultiplier = -1;
                foreach (string ResponseLine in new LineReader(() => new StringReader(ResponseData)))
                {
                    string ResponseLineWorkingObject = ResponseLine;
                    if (ResponseLineWorkingObject.StartsWith(TIMEZONE_OFFSET, false, System.Globalization.CultureInfo.CurrentCulture))
                    {
                        TimeZoneOffset = Convert.ToInt32(ResponseLine.Substring(TIMEZONE_OFFSET.Length + 1));
                        continue;
                    }

                    if (ResponseLineWorkingObject.StartsWith(INTERVALDELIMITER, false, System.Globalization.CultureInfo.CurrentCulture))
                    {
                        DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                        ResponseDateTime = dtDateTime.AddSeconds(Convert.ToDouble(ResponseLine.Substring(1).Split(',')[0])).AddMinutes(TimeZoneOffset);
                        //remove the unixtimestamp
                        ResponseLineWorkingObject = ResponseLineWorkingObject.Remove(0, ResponseLine.Split(',')[0].Length);
                        ResponseLineWorkingObject = "0" + ResponseLineWorkingObject;
                        IntervalMultiplier = 0;

                    }
                    //TODO: currency exchange from USD to the instruments currency!!
                    //where in the data loop now
                    if (IntervalMultiplier >= 0)
                    {
                        HistoricalQuoteDetails oHd = new HistoricalQuoteDetails();
                        int i = 0;
                        foreach (string Column in oColumnOrder)
                        {
                            switch (Column)
                            {
                                case "DATE":
                                    IntervalMultiplier = int.Parse(ResponseLineWorkingObject.Split(',')[i]);
                                    oHd.Date = GetDateFromIntervalData(IntervalMultiplier, IntervalInSec, ResponseDateTime);
                                    break;
                                case "CLOSE":
                                    oHd.Close = CustomParse(ResponseLineWorkingObject.Split(',')[i]);
                                    break;
                                case "LOW":
                                    oHd.Low = CustomParse(ResponseLineWorkingObject.Split(',')[i]);
                                    break;
                                case "HIGH":
                                    oHd.High = CustomParse(ResponseLineWorkingObject.Split(',')[i]);
                                    break;
                                case "OPEN":
                                    oHd.Open = CustomParse(ResponseLineWorkingObject.Split(',')[i]);
                                    break;
                                case "VOLUME":
                                    oHd.Volume = int.Parse(ResponseLineWorkingObject.Split(',')[i]);
                                    break;

                                default:
                                    break;
                            }
                            i++;
                        }
                        oIntervalData.Add(oHd);

                    }

                }

                if (oIntervalData.Count == 0)
                {
                    return null;
                }
                return new HistoricalQuote(Exchange, Instrument, oIntervalData);
            }
            return null;
        }

        public HistoricalQuote GetHistoricalData(Instrument Instrument, Exchange Exchange)
        {
            return GetHistoricalData(Instrument, Exchange, new DateTime(), false);

        }

        public decimal CustomParse(string incomingValue)
        {
            decimal val;
            if (!decimal.TryParse(incomingValue.Replace(",", "").Replace(".", ""), NumberStyles.Number, CultureInfo.InvariantCulture, out val))
                return 0m;
            return val / 100;
        }

        private  DateTime GetDateFromIntervalData(int IntervalAmount, int IntervalSeconds, DateTime StartDateTime )
        {
            try
            {
                DateTime New = StartDateTime.AddSeconds(IntervalAmount * IntervalSeconds);
            }
            catch(Exception ex)
            {
                int gg = 0;
            }

            return StartDateTime.AddSeconds(IntervalAmount * IntervalSeconds);
        }


        public bool IsExchangeOpen()
        {
            return true;
        }
    }
}
