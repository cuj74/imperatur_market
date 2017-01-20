using System.Collections.Generic;
using Imperatur_v2.order;
using Imperatur_v2.events;


namespace Imperatur_v2.trade.automation
{
    public interface ITradeAutomation
    {
        List<IOrder> RunTradeAutomation();
        event ImperaturMarket.SystemNotificationHandler SystemNotificationEvent;
    }
}
