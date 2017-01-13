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
        public TriggerOperator Operator;
        public TriggerValueType ValueType;
        public decimal TradePriceValue;
        public decimal PercentageValue;

        public bool Evaluate(Instrument Instrument)
        {
            decimal TradePriceValueEval = -1;
            switch (ValueType)
            {
                case TriggerValueType.TradePrice:
                    TradePriceValueEval = ImperaturGlobal.Quotes.Where(q => q.Symbol.Equals(Instrument.Symbol)).First().LastTradePrice.Amount;
                    switch (Operator)
                    {
                        case TriggerOperator.EqualOrGreater:
                            return TradePriceValueEval >= TradePriceValue;
                        case TriggerOperator.EqualOrless:
                            return TradePriceValueEval <= TradePriceValue;
                        default:
                            break;
                    }
                    break;
                case TriggerValueType.Percentage:
                    TradePriceValueEval = ImperaturGlobal.Quotes.Where(q => q.Symbol.Equals(Instrument.Symbol)).First().LastTradePrice.Amount;
                    switch (Operator)
                    {
                        case TriggerOperator.EqualOrGreater:
                            return (TradePriceValueEval/TradePriceValue)*100 >= PercentageValue;
                        case TriggerOperator.EqualOrless:
                            return (TradePriceValueEval / TradePriceValue) * 100 <= PercentageValue;
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
