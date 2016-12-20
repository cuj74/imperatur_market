using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_Test
{
    public class AccounTradingSettings
    {
        public Guid AccountIdentifier;
        public int HistoricalAnalysis;
        public int StandardDeviation3M;
        public int StandardDeviation12M;
        public int InternetSearch;
        public int RSSSearch;
        public int TwitterSearch;
        public int Getpercentage(string Variable)
        {
            int total = HistoricalAnalysis+ StandardDeviation3M+ StandardDeviation12M+ InternetSearch+ RSSSearch+ TwitterSearch;
            switch (Variable)
            {
                case "HistoricalAnalysis":
                    {
                        return Convert.ToInt32((Convert.ToDecimal(HistoricalAnalysis) / total) * 100);
                    }
                case "StandardDeviation3M":
                    {
                        return Convert.ToInt32((Convert.ToDecimal(StandardDeviation3M) / total) * 100);
                    }
                case "StandardDeviation12M":
                    {
                        return Convert.ToInt32((Convert.ToDecimal(StandardDeviation12M) / total) * 100);
                    }
                case "InternetSearch":
                    {
                        return Convert.ToInt32((Convert.ToDecimal(InternetSearch) / total) * 100);
                     }
                case "RSSSearch":
                    {
                        return Convert.ToInt32((Convert.ToDecimal(RSSSearch) / total) * 100);
                     }
                case "TwitterSearch":
                    {
                        return Convert.ToInt32((Convert.ToDecimal(TwitterSearch) / total) * 100);
                    }
            }
            return 0;
        }
    }
}
