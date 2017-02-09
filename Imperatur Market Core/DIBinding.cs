using Ninject.Modules;
using Imperatur_Market_Core.database;
using Imperatur_Market_Core.account;
using Imperatur_Market_Core.user;
using Imperatur_Market_Core.trade;
using Imperatur_Market_Core.securities;
using Imperatur_Market_Core.system;
using Imperatur_Market_Core.monetary;

namespace Imperatur_v2
{
    public class DIBinding : NinjectModule
    {
        public override void Load()
        {
            Bind<IDatabaseHandler>().To<DatabaseHandler>();
            Bind<IAccountHandler>().To<AccountHandler>();
            Bind<IUserHandler>().To<UserHandler>();
            Bind<ITradeHandler>().To<TradeHandler>();
            Bind<ISecurityHandler>().To<SecurityHandler>();
            Bind<ISystemHandler>().To<SystemHandler>();
            Bind<IAccount>().To<Account>();
            Bind<ILogicalTransactionHandler>().To<LogicalTransactionHandler>();
            

            //Bind<>().To<>();
            /*
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
            Bind<IHistoricalPriceCacheBuilder>().To<HistoricalPriceCacheBuilder>();*/
        }
    }
}
