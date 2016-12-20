using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_Test
{
    public class InstrumentRating
    {
        public DateTime Logdate;
        public int HistoricalAnalysis; //the forecast of rising or fallin (negative means falling)
        public decimal StandardDeviation3M_high; //high value
        public decimal StandardDeviation3M_low; 
        public decimal StandardDeviation12M_high; //high value
        public decimal StandardDeviation12M_low;
        public int InternetSearch;
        public int RSSSearch;
        public int TwitterSearch;
    }
}
