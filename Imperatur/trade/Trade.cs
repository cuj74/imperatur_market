using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur.monetary;
using Imperatur.securities;
namespace Imperatur.trade
{
    public class Trade : ITradeInterface
    {
        public Money TradeAmount;
        public Decimal Quantity;
        public Money AverageAcquisitionValue;
        public DateTime TradeDateTime;
        public Securities Security;
        public Money Revenue;
        public Money GetGAA()
        {
            return TradeAmount.Divide(Quantity);
            //return AverageAcquisitionValue;
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
