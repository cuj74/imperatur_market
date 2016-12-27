using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.monetary;
using Imperatur_v2.securites;
using Imperatur_v2.shared;

namespace Imperatur_v2.trade
{
    public class Trade : ITradeInterface
    {
        private IMoney m_oTradeAmount;
        private Decimal m_oQuantity;
        private IMoney m_oAverageAcquisitionValue;
        private DateTime m_oTradeDateTime;
        private Security m_oSecurity;
        private IMoney m_oRevenue;

        public IMoney TradeAmount
        {
            get
            {
                return m_oTradeAmount;
            }
        }

        public decimal Quantity
        {
            get
            {
                return m_oQuantity;
            }
        }

        public IMoney AverageAcquisitionValue
        {
            get
            {
                return m_oAverageAcquisitionValue;
            }
        }

        public DateTime TradeDateTime
        {
            get
            {
                return m_oTradeDateTime;
            }
        }

        public Security Security
        {
            get
            {
                return m_oSecurity;
            }
        }

        public IMoney Revenue
        {
            get
            {
                return m_oRevenue;
            }
        }

        public IMoney GetGAA()
        {
            return m_oTradeAmount.Divide(m_oQuantity);
        }
        public Decimal GetQuantity()
        {
            return m_oQuantity;
        }
        public IMoney GetTradeAmount()
        {
            return m_oTradeAmount;
        }
        public Trade(IMoney TradeAmount, Decimal Quantity, Security Security)
        {
            this.m_oTradeAmount = TradeAmount;
            this.m_oQuantity = Quantity;
            this.m_oSecurity = Security;
            this.m_oAverageAcquisitionValue = Security.Price;
            this.m_oTradeDateTime = DateTime.Now;
        }
    }
}
