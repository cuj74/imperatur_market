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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Newtonsoft.Json;
using System.IO;
using Imperatur_v2.customer;

namespace Imperatur_v2.handler
{
    public class AccountHandler : IAccountHandlerInterface
    {
        private ObservableRangeCollection<IAccountInterface> m_oAccounts;
        private string LastErrorMessage;
        private ObjectMapping m_oObjectMapping;
        private ObjectTextSearch m_oSearchObjects;
        private bool TryLoadFromStorage;
        
        public delegate void SaveAccountEventHandler(object sender, events.SaveAccountEventArg e);

        public AccountHandler()
        {
            TryLoadFromStorage = false;
            LastErrorMessage = "";
            m_oObjectMapping = new ObjectMapping();
            m_oSearchObjects = new ObjectTextSearch();
            m_oAccounts = new ObservableRangeCollection<IAccountInterface>();
            
        }
        

        public List<IAccountInterface> SearchAccount(string Search, AccountType AccountTypeToSearch)
        {
            if (Search.Equals("*"))
            {
                return Accounts().Where(a => a.GetAccountType().Equals(AccountTypeToSearch)).ToList();
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

        public IMoney CalculateHoldingSell(Guid Identifier, int Quantity, string Ticker)
        {
            return m_oAccounts.Single(a => a.Identifier.Equals(Identifier)).CalculateHoldingSell(Quantity, Ticker);
        }

        public bool CreateAccount(List<IAccountInterface> oAccountData)
        {
            m_oAccounts.AddRange(oAccountData);
            m_oAccounts.CollectionChanged -= M_oAccounts_CollectionChanged;
            m_oAccounts.CollectionChanged += M_oAccounts_CollectionChanged;
            return true;
        }

        private void M_oAccounts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (IAccountInterface item in m_oAccounts)
            {
                //item.SaveAccountEvent -= AccountPropertyChanged;
                item.SaveAccountEvent -= Item_SaveAccountEvent;
                item.SaveAccountEvent += Item_SaveAccountEvent;
            }

            SaveAccounts();
        }

        private void Item_SaveAccountEvent(object sender, events.SaveAccountEventArg e)
        {
            SaveAccount(e.Identifier);
        }

        public bool DepositAmount(Guid Identifier, IMoney Deposit)
        {
            IAccountInterface oA = m_oAccounts.Single(a => a.Identifier.Equals(Identifier));
            try
            {
                oA.AddTransaction(
                          ImperaturGlobal.Kernel.Get<ITransactionInterface>(
                                  new Ninject.Parameters.ConstructorArgument("_DebitAmount", Deposit),
                                  new Ninject.Parameters.ConstructorArgument("_CreditAmount", Deposit),
                                  new Ninject.Parameters.ConstructorArgument("_DebitAccount", oA.GetBankAccountsFromCache().First()),
                                  new Ninject.Parameters.ConstructorArgument("_CreditAccount", oA.Identifier),
                                  new Ninject.Parameters.ConstructorArgument("_TransactionType", TransactionType.Transfer),
                                  new Ninject.Parameters.ConstructorArgument("_SecurtiesTrade", (object)null)
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
            foreach (IAccountInterface oA in m_oAccounts )
            {
                SaveSingleAccount(oA);
            }
            /*
            json.SerializeJSONdata.SerializeObject(Accounts(),
                string.Format(@"{0}\{1}\{2}", ImperaturGlobal.SystemData.SystemDirectory, ImperaturGlobal.SystemData.AcccountDirectory, ImperaturGlobal.SystemData.AccountFile));
            */        
            return true;
        }

        private bool SaveSingleAccount(IAccountInterface oA)
        {

            //ImperaturGlobal.Kernel

            //GetMappingRowToObjects(typeof(IAccountInterface), new object[] { oA })
            //(IAccountInterface)oA.MemberwiseClone()

            //Customer Customer, AccountType AccountType, string AccountName, ObservableRangeCollection<ITransactionInterface> Transactions, Guid Identifier)
            //IAccountInterface Copy = ImperaturGlobal.Kernel.Get<IAccountInterface>(
            //    new Ninject.Parameters.ConstructorArgument("Customer", oA.GetCustomer()),
            //    new Ninject.Parameters.ConstructorArgument("AccountType", oA.GetAccountType()),
            //    new Ninject.Parameters.ConstructorArgument("AccountName", oA.AccountName),
            //    new Ninject.Parameters.ConstructorArgument("Transactions", oA.Transactions),
            //    new Ninject.Parameters.ConstructorArgument("Identifier", oA.Identifier)
            // );

            json.SerializeJSONdata.SerializeObject((Account)oA,
              string.Format(@"{0}\{1}\{2}.json", ImperaturGlobal.SystemData.SystemDirectory, ImperaturGlobal.SystemData.AcccountDirectory, oA.Identifier));
            return true;
        }

        public bool SaveAccount(Guid Identifier)
        {
            foreach (IAccountInterface oA in m_oAccounts.Where(a=>a.Identifier.Equals(Identifier)))
            {
                SaveSingleAccount(oA);
            }
            return true;
        }

        public bool SellHoldingFromAccount(Guid Identifier, int Quantity, string Ticker)
        {
            throw new NotImplementedException();
        }

        public List<IAccountInterface> Accounts()
        {
            //TODO lägg till en try
            if (TryLoadFromStorage == false)
            {
                try
                {
                    m_oAccounts = LoadAccounts();// (ObservableRangeCollection<IAccountInterface>)json.DeserializeJSON.DeserializeObjectFromFile(@ImperaturGlobal.SystemData.AccountFile);
                    m_oAccounts.CollectionChanged -= M_oAccounts_CollectionChanged;
                    m_oAccounts.CollectionChanged += M_oAccounts_CollectionChanged;

                    foreach (IAccountInterface item in m_oAccounts)
                    {
                        item.SaveAccountEvent -= Item_SaveAccountEvent;
                        item.SaveAccountEvent += Item_SaveAccountEvent;
                    }
                }
                catch(Exception ex)
                {
                    m_oAccounts = new ObservableRangeCollection<IAccountInterface>();

                }
            }
            TryLoadFromStorage = true;

            return new List<IAccountInterface>((IEnumerable<IAccountInterface>)m_oAccounts); 
        }

        private ObservableRangeCollection<IAccountInterface> LoadAccounts()
        {
            ObservableRangeCollection<IAccountInterface> AccountFromFiles = new ObservableRangeCollection<IAccountInterface>();

            string[] files = Directory.GetFiles(string.Format(@"{0}\{1}\", ImperaturGlobal.SystemData.SystemDirectory, ImperaturGlobal.SystemData.AcccountDirectory), "*.json", SearchOption.TopDirectoryOnly);

            foreach(string Fa in files)
            {
                //Account a = JsonConvert.DeserializeObject<Account>(json.DeserializeJSON.DeserializeObjectFromFile(Fa).ToString());

                //IAccountInterface a2 = JsonConvert.DeserializeObject<Account>(json.DeserializeJSON.DeserializeObjectFromFile(Fa).ToString());

                //int gg = 0;
                //object o = json.DeserializeJSON.DeserializeObjectFromFile(Fa);
                AccountFromFiles.Add((IAccountInterface)json.DeserializeJSON.DeserializeObjectFromFile(Fa));
                //var settings = new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new json.AllFieldsContractResolver(), TypeNameHandling = TypeNameHandling.All };
                //IAccountInterface oa =JsonConvert.DeserializeObject<IAccountInterface>(o.ToString(), settings);

                //AccountFromFiles.Add((IAccountInterface)o);
                //IAccountInterface oa = (IAccountInterface)o;

                //AccountFromFiles.Add(JsonConvert.DeserializeObject<IAccountInterface>(json.DeserializeJSON.DeserializeObjectFromFile(Fa).ToString()));
                /*
                AccountFromFiles.Add(
                    (IAccountInterface)Newtonsoft.Json.JsonConvert.DeserializeObject(
                    json.DeserializeJSON.DeserializeObjectFromFile(Fa).ToString()
                    ));
                    */
            }
            return AccountFromFiles;


        }


        private List<object> GetMappingRowToObjects(Type TypeOfObject, object[] oR)
        {
            return (
                from r in oR
                select m_oObjectMapping.GetMappingToObject(Activator.CreateInstance(TypeOfObject), r)
                ).ToList();
        }

        public bool CreateAccount(Customer customer, AccountType accountType, string AccountName)
        {
            try
            {
                int g = GetBankAccountsFromCache().Count();
            }
            catch
            (Exception ex)
            {
                int fdfd = 0;
            }
            List<IAccountInterface> oNewList = new List<IAccountInterface>();
            oNewList.Add(
            ImperaturGlobal.Kernel.Get<IAccountInterface>(
                        new Ninject.Parameters.ConstructorArgument("Customer", customer ?? (object)null),
                        new Ninject.Parameters.ConstructorArgument("AccountType", accountType),
                        new Ninject.Parameters.ConstructorArgument("AccountName", AccountName)
                    ));
            CreateAccount(oNewList);
            return true;
        
        }
    }
}
