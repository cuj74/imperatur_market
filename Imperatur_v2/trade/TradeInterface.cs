using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.monetary;
using Imperatur_v2.securites;

namespace Imperatur_v2.trade
{
    public interface ITradeInterface
    {
        #region properties
        IMoney TradeAmount { get; }
        Decimal Quantity { get; }
        IMoney AverageAcquisitionValue { get; }
        DateTime TradeDateTime { get; }
        Security Security { get; }
        IMoney Revenue { get; }
        #endregion

        IMoney GetGAA();
        Decimal GetQuantity();
        IMoney GetTradeAmount();

    }
}
