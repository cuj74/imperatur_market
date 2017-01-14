using Imperatur_v2.securites;
using Imperatur_v2.trade.recommendation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_v2.trade.analysis
{
    public interface ISecurityAnalysis
    {
        decimal StandardDeviationForRange(DateTime Start, DateTime End);
        decimal StandardDeviation { get; }
        Instrument Instrument { get; }
        decimal ChangeSince(DateTime From);
        bool RangeConvergeWithElliotForBuy(int IntervalInDays, out TradingRecommendation TradingRecommendation);
        bool RangeConvergeWithElliotForSell(int IntervalInDays);
        bool HasValue { get; }
        List<double> MovingAverageForRange(DateTime Start, DateTime End);
        List<List<double>> StandardBollingerForRange(DateTime Start, DateTime End, int Period = 20, double Multiply = 2);
        List<HistoricalQuoteDetails> GetDataForRange(DateTime Start, DateTime End);
        List<Tuple<DateTime, VolumeIndicator>> GetRangeOfVolumeIndicator(DateTime Start, DateTime End);
        List<TradingRecommendation> GetTradingRecommendations();
    }
}
