using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Imperatur_Test
{
    public class HistoricalStock
    {
        public DateTime Date;
        public double Open;
        public double High;
        public double Low;
        public double Close;
        public double Volume;
        public double AdjClose;
    }

    public class HistoricalStockDownloader
    {

        public static List<HistoricalStock> DownloadData(string ticker, int Monthsback)
        {
            List<HistoricalStock> retval = new List<HistoricalStock>();

            using (WebClient web = new WebClient())
            {

                /*
                
                http://ichart.finance.yahoo.com/table.csv?s=YHOO&d=0&e=28&f=2010&g=d&a=3&b=12&c=2009&ignore=.csv
                 
sn =Ticker symbol (YHOO in the example)
a = The "from month" - 1
b = The "from day" (two digits)
c = The "from year"
d = The "to month" - 1
e = The "to day" (two digits)
f = The "to year"
g = d for day, m for month, y for yearly

                */

                DateTime From =  DateTime.Now.AddMonths(-Monthsback);
                string URL = string.Format("http://ichart.finance.yahoo.com/table.csv?s={0}&d={1}&e={2}&f={3}&g=d&a={4}&b={5}&c={6}&ignore=.csv",
                    ticker,
                    DateTime.Now.Year,
                    DateTime.Now.Day,
                    DateTime.Now.Year,
                    From.Month,
                    From.Day,
                    From.Year);




                string data = web.DownloadString(URL);

                data = data.Replace("r", "");

                string[] rows = data.Split('n');

                //must fix this to get the right data!!
                //First row is headers so Ignore it
                for (int i = 1; i < rows.Length; i++)
                {
                    if (rows[i].Replace("n", "").Trim() == "") continue;

                    string[] cols = rows[i].Split(',');

                    HistoricalStock hs = new HistoricalStock();
                    hs.Date = Convert.ToDateTime(cols[0]);
                    hs.Open = Convert.ToDouble(cols[1]);
                    hs.High = Convert.ToDouble(cols[2]);
                    hs.Low = Convert.ToDouble(cols[3]);
                    hs.Close = Convert.ToDouble(cols[4]);
                    hs.Volume = Convert.ToDouble(cols[5]);
                    hs.AdjClose = Convert.ToDouble(cols[6]);

                    retval.Add(hs);
                }

                return retval;
            }
        }
    }
}
