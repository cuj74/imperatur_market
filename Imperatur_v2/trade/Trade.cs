using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.monetary;
using Imperatur_v2.securites;

namespace Imperatur_v2.trade
{
    public class Trade : ITradeInterface
    {
        public Money TradeAmount;
        public Decimal Quantity;
        public Money AverageAcquisitionValue;
        public DateTime TradeDateTime;
        public Security Security;
        public Money Revenue;
        public Money GetGAA()
        {
            return TradeAmount.Divide(Quantity);
        }
        public Decimal GetQuantity()
        {
            return Quantity;
        }
        public Money GetTradeAmount()
        {
            return TradeAmount;
        }
    }
}
