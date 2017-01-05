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
        decimal ChangeSince(DateTime From);
        bool RangeConvergeWithElliotForBuy(int IntervalInDays, out decimal SaleValue);
        bool RangeConvergeWithElliotForSell(int IntervalInDays);


    }
}
