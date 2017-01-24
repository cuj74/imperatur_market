using Imperatur_v2.handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_v2.order
{
    public interface IOrderQueue
    {
        bool EvaluateOrdersInQueue();
        bool AddOrder(IOrder Order);
        bool AddOrders(List<IOrder> Orders);
        List<IOrder> GetOrdersForAccount(Guid AccountIdentifier);
        bool QueueMaintence(IAccountHandlerInterface AccountHandler);
    }
}
