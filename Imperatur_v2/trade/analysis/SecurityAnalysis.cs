using Imperatur_v2.securites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.shared;

namespace Imperatur_v2.trade.analysis
{
    public class SecurityAnalysis : ISecurityAnalysis
    {
        HistoricalQuote m_oH;
        public SecurityAnalysis(Instrument Instrument)
        {
            m_oH = ImperaturGlobal.HistoricalQuote(Instrument);
        }

        public decimal StandardDeviation
        {
            get
            {
                Double g = m_oH.HistoricalQuoteDetails.Select(h => Convert.ToDouble(h.Close)).StdDev();
                return Convert.ToDecimal(m_oH.HistoricalQuoteDetails.Select(h => Convert.ToDouble(h.Close)).StdDev());
            }
        }

        public decimal ChangeSince(DateTime From)
        {
            //error, fix
            return Convert.ToDecimal(m_oH.HistoricalQuoteDetails
                .Where(h=>h.Date >= From)
                .Select(h => Convert.ToDouble(h.Close)).StdDev());
        }

        public decimal StandardDeviationForRange(DateTime Start, DateTime End)
        {
            return Convert.ToDecimal(m_oH.HistoricalQuoteDetails
                .Where(h => h.Date >= Start && h.Date <= End)
                .Select(h => Convert.ToDouble(h.Close)).StdDev());
        }
    }
}
