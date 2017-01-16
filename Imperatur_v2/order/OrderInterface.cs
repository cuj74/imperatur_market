using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.handler;

namespace Imperatur_v2.order
{
    public interface IOrder
    {
        bool EvaluateTriggerOnOrder();
        bool ExecuteOrder(IAccountHandlerInterface AccountHandler, ITradeHandlerInterface TradeHandler, out IOrder StopLossOrder);
        Guid AccountIdentifier { get; }
        string Symbol { get; }
        string LastErrorMessage { get; }
        Guid Identifier { get; }
        DateTime ValidToDate { get; }
        //event OrderQueue.SaveOrderHandler SaveOrderEvent;
        
    }
}
