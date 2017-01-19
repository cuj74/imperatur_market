using Imperatur_v2.securites;
using Imperatur_v2.trade.analysis;
using Imperatur_v2.trade.recommendation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_v2.trade.automation
{
    public class InstrumentRecommendation
    {
        public Instrument InstrumentInfo;
        public List<TradingRecommendation> TradingRecommendations;
        public VolumeIndicator VolumeIndication;
        public int ExternalSearchHits;
    }
}
