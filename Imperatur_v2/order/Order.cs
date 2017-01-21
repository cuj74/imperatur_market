using Imperatur_v2.securites;
using System;
using System.Collections.Generic;
using System.Linq;
using Imperatur_v2.shared;
using Imperatur_v2.account;
using Imperatur_v2.handler;
using Ninject;
namespace Imperatur_v2.order
{
    public enum OrderType
    {
        Buy,
        Sell,
        StopLoss
    }
    public class Order : IOrder
    {
        private string m_oSymbol;
        private List<ITrigger> m_oTriggers;
        private Guid m_oAccountIdentifier;
        private string m_oLastErrorMessage;
        private int m_oQuantity;
        private OrderType m_oOrderType;
        private decimal m_oStopLossAmount;
        private decimal m_oStopLossPercentage;
       
        private DateTime m_oValidToDate;
        private int m_oStopLossDaysValid;
        private Guid m_oIdentifier;
        private string m_oProcessCode;

        public string ProcessCode
        {
            get
            {
                return m_oProcessCode;
            }

        }

        public string Symbol
        {
            get
            {
                return m_oSymbol;
            }

        }

        public Guid AccountIdentifier
        {
            get
            {
                return m_oAccountIdentifier;
            }

        }

        public string LastErrorMessage
        {
            get
            {
                return m_oLastErrorMessage;
            }

        }

        public DateTime ValidToDate
        {
            get
            {
                return m_oValidToDate;
            }

            set
            {
                m_oValidToDate = value;
            }
        }

        public Guid Identifier
        {
            get
            {
                return m_oIdentifier;
            }


        }

        public OrderType OrderType
        {
            get
            {
                return m_oOrderType;
            }
        }

        public int Quantity
        {
            get
            {
                return m_oQuantity;
            }
        }

        public Order(string Symbol, List<ITrigger> Trigger, Guid AccountIdentifier, int Quantity, OrderType OrderType, DateTime ValidToDate, int StopLossValidDays = 0, decimal StopLossAmount = 0, decimal StopLossPercentage = 0)
        {
            m_oIdentifier = Guid.NewGuid();
            m_oSymbol = Symbol;
            m_oTriggers = Trigger;
            m_oAccountIdentifier = AccountIdentifier;
            m_oLastErrorMessage = "";
            m_oQuantity = Quantity;
            m_oOrderType = OrderType;
            m_oValidToDate = ValidToDate;
            if (m_oOrderType.Equals(OrderType.StopLoss))
            {
                if (StopLossAmount == StopLossPercentage && StopLossPercentage == 0 || StopLossValidDays == 0 || StopLossValidDays < 0)
                {
                    throw new Exception("For stoploss either a StopLossPercentage or StopLossAmount must be provided and above zero! Also StopLossValidDays must be greater than zero.");
                }
                m_oStopLossAmount = StopLossAmount;
                m_oStopLossPercentage = StopLossPercentage;
                m_oStopLossDaysValid = StopLossValidDays;
            }
            /*
            OnSaveOrder(new SaveOrderEventArg()
            {
                Identifier = this.Identifier
            });*/

        }

        private Order CreateStopLossOrder(decimal TradePrice)
        {
            List<ITrigger> StopLossTriggers = new List<ITrigger>();
            if (m_oStopLossPercentage > 0)
            {
                StopLossTriggers.Add(
                    ImperaturGlobal.Kernel.Get<ITrigger>(
                    new Ninject.Parameters.ConstructorArgument("m_oOperator", TriggerOperator.EqualOrless),
                    new Ninject.Parameters.ConstructorArgument("m_oValueType", TriggerValueType.Percentage),
                    new Ninject.Parameters.ConstructorArgument("m_oTradePriceValue", TradePrice),
                    new Ninject.Parameters.ConstructorArgument("m_oPercentageValue", m_oStopLossPercentage)
                    )
                );
             }
            if (m_oStopLossAmount > 0)
            {
                StopLossTriggers.Add(
                  ImperaturGlobal.Kernel.Get<ITrigger>(
                    new Ninject.Parameters.ConstructorArgument("m_oOperator", TriggerOperator.EqualOrless),
                    new Ninject.Parameters.ConstructorArgument("m_oValueType", TriggerValueType.TradePrice),
                    new Ninject.Parameters.ConstructorArgument("m_oTradePriceValue", m_oStopLossAmount),
                    new Ninject.Parameters.ConstructorArgument("m_oPercentageValue", 0)
                    )
                );
            }
            return new Order(m_oSymbol, StopLossTriggers, m_oAccountIdentifier, m_oQuantity, OrderType.Sell, DateTime.Now.AddDays(m_oStopLossDaysValid).Date);
        }

        public bool EvaluateTriggerOnOrder()
        {
            try
            {
                Instrument InstrumentToEvaluate = ImperaturGlobal.Instruments.Where(i => i.Symbol.Equals(m_oSymbol)).First();
                return (m_oTriggers.Select(t => t.Evaluate(InstrumentToEvaluate)).Where(t2 => t2.Equals(true)).Count() > 0);
            }
            catch(Exception ex)
            {
                throw new Exception("Trigger or Instrument error: " + ex.Message);
            }
        }

        public bool ExecuteOrder(IAccountHandlerInterface AccountHandler, ITradeHandlerInterface TradeHandler, out IOrder StopLossOrder)
        {
            StopLossOrder = this;
            if (EvaluateTriggerOnOrder())
            {
                try
                {
                    IAccountInterface oA = AccountHandler.GetAccount(m_oAccountIdentifier);
                    switch (m_oOrderType)
                    {
                        case OrderType.Buy:
                            oA.AddHoldingToAccount(m_oQuantity, m_oSymbol, TradeHandler);
                            StopLossOrder = this;
                            return true;
                        case OrderType.Sell:
                            oA.SellHoldingFromAccount(m_oQuantity, m_oSymbol, TradeHandler);
                            return true;
                        case OrderType.StopLoss:
                            oA.AddHoldingToAccount(m_oQuantity, m_oSymbol, TradeHandler);
                            StopLossOrder = CreateStopLossOrder(ImperaturGlobal.Quotes.Where(q => q.Symbol.Equals(m_oSymbol)).First().LastTradePrice.Amount);
                            return true;
                        default:
                            break;
                    }

                }
                catch (Exception ex)
                {
                    m_oLastErrorMessage = "OrderExecute error: " + ex.Message;
                    return false;
                }

            }
            return false;
        }
    }
}
