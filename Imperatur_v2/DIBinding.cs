using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;
using Imperatur_v2.shared;
using Imperatur_v2.monetary;
using Imperatur_v2.account;
using Imperatur_v2.handler;
using Imperatur_v2.trade;
using Imperatur_v2.securites;
using Imperatur_v2.trade.analysis;
using Imperatur_v2.trade.recommendation;
using Imperatur_v2.order;
using Imperatur_v2.trade.automation;

namespace Imperatur_v2
{
    public class DIBinding : NinjectModule
    {
        public override void Load()
        {
            Bind<ICurrency>().To<Currency>();
            Bind<IImperaturMarket>().To<ImperaturMarket>();
            Bind<ITransactionInterface>().To<Transaction>();
            Bind<IMoney>().To<Money>();
            Bind<IAccountInterface>().To<Account>();
            Bind<IAccountHandlerInterface>().To<AccountHandler>();
            Bind<ITradeInterface>().To<Trade>();
            Bind<ITradeHandlerInterface>().To<TradeHandler>();
            Bind<ISecurityAnalysis>().To<SecurityAnalysis>();
            Bind<IOrder>().To<Order>();
            Bind<IOrderQueue>().To<OrderQueue>();
            Bind<ITrigger>().To<Trigger>();
            Bind<ITradeAutomation>().To<TradeAutomation>();

            

        }
    }
}
