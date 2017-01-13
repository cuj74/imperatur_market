using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.securites;
using Imperatur_v2.shared;

namespace Imperatur_v2.order
{
    public enum TriggerOperator
    {
        EqualOrGreater,
        EqualOrless
    }
    public enum TriggerValueType
    {
        TradePrice,
        Percentage
    }

    public class Trigger : ITrigger
    {
        private TriggerOperator m_oOperator;
        private TriggerValueType m_oValueType;
        private decimal m_oTradePriceValue;
        private decimal m_oPercentageValue;

        public Trigger(TriggerOperator Operator, TriggerValueType ValueType, decimal TradePriceValue, decimal PercentageValue)
        {
            m_oOperator = Operator;
            m_oValueType = ValueType;
            m_oTradePriceValue = TradePriceValue;
            m_oPercentageValue = PercentageValue;
            if (m_oTradePriceValue == m_oPercentageValue && m_oPercentageValue == 0)
            {
                throw new Exception("TradePriceValue and PercentageValue cant both be zero!");
            }

        }

        public bool Evaluate(Instrument Instrument)
        {
            decimal TradePriceValueEval = -1;
            switch (m_oValueType)
            {
                case TriggerValueType.TradePrice:
                    TradePriceValueEval = ImperaturGlobal.Quotes.Where(q => q.Symbol.Equals(Instrument.Symbol)).First().LastTradePrice.Amount;
                    switch (m_oOperator)
                    {
                        case TriggerOperator.EqualOrGreater:
                            return TradePriceValueEval >= m_oTradePriceValue;
                        case TriggerOperator.EqualOrless:
                            return TradePriceValueEval <= m_oTradePriceValue;
                        default:
                            break;
                    }
                    break;
                case TriggerValueType.Percentage:
                    TradePriceValueEval = ImperaturGlobal.Quotes.Where(q => q.Symbol.Equals(Instrument.Symbol)).First().LastTradePrice.Amount;
                    switch (m_oOperator)
                    {
                        case TriggerOperator.EqualOrGreater:
                            return (TradePriceValueEval/m_oTradePriceValue)*100 >= m_oPercentageValue;
                        case TriggerOperator.EqualOrless:
                            return (TradePriceValueEval / m_oTradePriceValue) * 100 <= m_oPercentageValue;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            return true;
        }
    }
}
