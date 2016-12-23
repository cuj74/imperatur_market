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
            /*
            Bind<IBFSDataHandler>().To<BFSDataHandler>();
            Bind<IFundAdministration>().To<FundAdministration>();
            Bind<IUserHandler>().To<UserHandler>();
            Bind<IFundCompanyHandler>().To<FundCompanyHandler>();
            Bind<IFundEntityHandler>().To<FundEntityHandler>();
            Bind<IPowerOfAttorney>().To<PowerOfAttorneyHandler>();
            Bind<IInstrument>().To<Instrument>();
            Bind<IFundEntity>().To<FundEntity>();
            Bind<IFundCompany>().To<FundCompany>();
            Bind<IFundInstrument>().To<FundInstrument>();
            Bind<shared.ICurrencyExhangeHandler>().To<shared.CurrencyExhangeHandler>();
            Bind<ICurrency>().To<Currency>();
            Bind<ISQL>().To<SQLHandler>();
            Bind<IDbConnection>().To<SqlConnection>();
            Bind<IDbCommand>().To<SqlCommand>();
            */

        }
    }
}
