using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.order;
using Imperatur_v2.handler;


namespace Imperatur_v2.trade.automation
{
    public interface ITradeAutomation
    {
        List<IOrder> RunTradeAutomation();
    }
}
