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
    }
}
