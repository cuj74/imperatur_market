using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.monetary;
using Imperatur_v2.account;
using Imperatur_v2.trade;
using Imperatur_v2.shared;
using Ninject;
using System.Reflection;
using Imperatur_v2.cache;


namespace Imperatur_v2.handler
{
    public class AccountHandler : IAccountHandlerInterface
    {
        private List<IAccountInterface> m_oAccounts;
        private string LastErrorMessage;
        private ObjectMapping m_oObjectMapping;
        public List<ITransactionInterface> m_oTransactions;
        //private readonly object Identifier;
        private ObjectTextSearch m_oSearchObjects;


        public AccountHandler()
        {
            LastErrorMessage = "";
            m_oObjectMapping = new ObjectMapping();
            m_oSearchObjects = new ObjectTextSearch();
        }
        

        public List<IAccountInterface> SearchAccount(string Search, AccountType AccountTypeToSearch)
        {
            if (Search.Equals("*"))
            {
                return Accounts().Where(a=>a.GetAccountType().Equals(AccountTypeToSearch)).ToList();
            }
            return Accounts().Where(a => a.GetAccountType().Equals(AccountTypeToSearch)).Select(a =>
                new
                {
                    Hit = m_oSearchObjects.FindTextInObject(Search, a),
                    Account = a
                }
                ).Where(a=>a.Hit).Select(a=>a.Account).ToList();

/*
            return Accounts().Where(
                a => m_oSearchObjects FindTextInObject(Search, a.GetCustomer().FirstName)
                ||
                a.GetCustomer().LastName.Contains(Search)
                ).ToList();*/
        }

        public bool AddHoldingToAccount(Guid Identifier, int Quantity, string Ticker)
        {
            throw new NotImplementedException();
        }

        public bool AddTransactionToAccount(Guid Identifier, ITransactionInterface NewTransaction)
        {
            m_oAccounts.Single(a => a.Identifier.Equals(Identifier)).AddTransaction(NewTransaction);
            return true;
        }

        public Money CalculateHoldingSell(Guid Identifier, int Quantity, string Ticker)
        {
            throw new NotImplementedException();
        }

        public bool CreateAccount(List<IAccountInterface> oAccountData)
        {
            if (m_oAccounts == null)
                m_oAccounts = new List<IAccountInterface>();

            //for each Account add a zero balance transfer to get available funds

            IMoney ZeroBalance = ImperaturGlobal.Kernel.Get<IMoney>(
                new Ninject.Parameters.ConstructorArgument("Amount", 0m),
                new Ninject.Parameters.ConstructorArgument("Currency", ImperaturGlobal.GetSystemCurrency())
             );

            try
            {
                foreach (var account in oAccountData.Where(a => a.GetAccountType().Equals(AccountType.Customer)).ToList())
                {
                    //ITradeInterface oTrade = ImperaturGlobal.Kernel.Get<ITradeInterface>();
                    account.AddTransaction(
                              ImperaturGlobal.Kernel.Get<ITransactionInterface>(
                              new Ninject.Parameters.ConstructorArgument("DebitAmount", ZeroBalance),
                              new Ninject.Parameters.ConstructorArgument("CreditAmount", ZeroBalance),
                              new Ninject.Parameters.ConstructorArgument("DebitAccount", account.GetBankAccountsFromCache().First()),
                              new Ninject.Parameters.ConstructorArgument("CreditAccount", account.Identifier),
                              new Ninject.Parameters.ConstructorArgument("TransactionType", TransactionType.Transfer),
                              new Ninject.Parameters.ConstructorArgument("SecurtiesTrade", (object)null) //trade before
                             ));
                }

            }
            catch(Exception ex)
            {
                int gsd = 0;
            } 
            m_oAccounts.AddRange(oAccountData);

            return true;
        }

        public bool DepositAmount(Guid Identifier, IMoney Deposit)
        {
            IAccountInterface oA = m_oAccounts.Single(a => a.Identifier.Equals(Identifier));
            try
            {
                oA.AddTransaction(
                          ImperaturGlobal.Kernel.Get<ITransactionInterface>(
                                  new Ninject.Parameters.ConstructorArgument("DebitAmount", Deposit),
                                  new Ninject.Parameters.ConstructorArgument("CreditAmount", Deposit),
                                  new Ninject.Parameters.ConstructorArgument("DebitAccount", oA.GetBankAccountsFromCache().First()),
                                  new Ninject.Parameters.ConstructorArgument("CreditAccount", oA.Identifier),
                                  new Ninject.Parameters.ConstructorArgument("TransactionType", TransactionType.Transfer),
                                  new Ninject.Parameters.ConstructorArgument("SecurtiesTrade", (object)null)
                                                                            )
                                    );
            }
            catch(Exception ex)
            {
                LastErrorMessage = ex.Message;
                return false;
            }
            return true;
        }

        public IAccountInterface GetAccount(Guid Identifier)
        {
            return Accounts().Where(a => a.Identifier.Equals(Identifier)).FirstOrDefault();
        }

        public List<Holding> GetAccountHoldings(Guid Identifier)
        {
            throw new NotImplementedException();
        }

        public List<Account> GetBankAccounts()
        {
            throw new NotImplementedException();
        }

        private List<Guid> GetBusinessAccountsFromCache()
        {
            BusinessAccountCache oBA = (BusinessAccountCache)GlobalCachingProvider.Instance.GetItem(ImperaturGlobal.BusinessAccountCache);
            return oBA.GetCache().Select(b =>
                new Guid(b.Item1)).ToList();
        }
        public List<Guid> GetBankAccountsFromCache()
        {
            BusinessAccountCache oBA = (BusinessAccountCache)GlobalCachingProvider.Instance.GetItem(ImperaturGlobal.BusinessAccountCache);
            return oBA.GetCache().Where(b => b.Item2.Equals(AccountType.Bank.ToString())).Select(b =>
                  new Guid(b.Item1)).ToList();
        }

        public List<IMoney> GetDepositedAmountOnAccount(Guid Identifier)
        {
            List<IMoney> oM = m_oAccounts.Single(a => a.Identifier.Equals(Identifier)).GetDepositedAmount();
            if (oM.Count == 0)
            {
                return null;
            }
            //ugly and wrong but works right now!
            return oM;

            /*
            List<TransactionType> TransferWithdraw = new List<TransactionType>();
            TransferWithdraw.Add(TransactionType.Transfer);
            TransferWithdraw.Add(TransactionType.Withdrawal);

            var Transactions = GetMappingRowToObjects(typeof(Transaction), m_oTransactions.ToArray()).Select(
             item =>
             GetKernel().Get<ITransactionInterface>()
             ).ToList();

            //minus
            var DebitQuery =
                from t in Transactions
                where t.DebitAccount.Equals(this.Identifier)
                join tb in TransferWithdraw on t.TransactionType equals tb
                select t;

            //plus
            var CreditQuery =
                from t in m_oTransactions
                where t.CreditAccount.Equals(this.Identifier) && t.TransactionType.Equals(TransactionType.Transfer)
                select t;

            List<IMoney> SumMoney = new List<IMoney>();
            SumMoney.AddRange(DebitQuery.Select(m => m.DebitAmount.SwitchSign()));
            SumMoney.AddRange(CreditQuery.Select(m => m.CreditAmount));


            return (List<IMoney>)(from p in SumMoney
                                 group p.Amount() by p.CurrencyCode() into g
                                 select 
                                 ImperaturGlobal.GetMoney(g.ToList().Sum(), g.Key.ToString())
                                 ).ToList();
*/
        }

        public List<Account> GetHouseAccounts()
        {
            throw new NotImplementedException();
        }

        public List<Guid> GetHouseAndBankAccountsGuid()
        {
            throw new NotImplementedException();
        }

        public string GetLastErrorMessage()
        {
            throw new NotImplementedException();
        }

        public List<Money> GetTotalFundsOfAccount(Guid Identifier)
        {
            throw new NotImplementedException();
        }

        public bool SaveAccounts()
        {
            json.SerializeJSONdata.SerializeObject(Accounts(),
                string.Format(@"{0}\{1}\{2}", ImperaturGlobal.SystemData.SystemDirectory, ImperaturGlobal.SystemData.AcccountDirectory, ImperaturGlobal.SystemData.AccountFile));
            return true;
        }

        public bool SellHoldingFromAccount(Guid Identifier, int Quantity, string Ticker)
        {
            throw new NotImplementedException();
        }

        public List<IAccountInterface> Accounts()
        {
            //TODO lägg till en try
            if (m_oAccounts == null)
            {
                try
                {
                    
                    CurrencyCodeCache oC = (CurrencyCodeCache)GlobalCachingProvider.Instance.GetItem(ImperaturGlobal.CurrencyCodeCache);
                    string dfd = oC.GetCache().Select(i => i.Item1).First();
                    string ff = ImperaturGlobal.SystemData.AccountFile;
                    //BusinessAccountCache oBA = (BusinessAccountCache)GlobalCachingProvider.Instance.GetItem(ImperaturGlobal.BusinessAccountCache);
                    m_oAccounts = (List<IAccountInterface>)json.DeserializeJSON.DeserializeObjectFromFile(@ImperaturGlobal.SystemData.AccountFile);
                }
                catch(Exception ex)
                {
                    m_oAccounts = new List<IAccountInterface>();
                }
            }
            return m_oAccounts;
        }

        private StandardKernel GetKernel()
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            return kernel;
        }

        private List<object> GetMappingRowToObjects(Type TypeOfObject, object[] oR)
        {
            return (
                from r in oR
                select m_oObjectMapping.GetMappingToObject(Activator.CreateInstance(TypeOfObject), r)
                ).ToList();
        }

    }
}
