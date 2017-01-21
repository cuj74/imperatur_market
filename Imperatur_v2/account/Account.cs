using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.monetary;
using Imperatur_v2.trade;
using Imperatur_v2.customer;
using Imperatur_v2.shared;
using Imperatur_v2.cache;
using Ninject;
using Imperatur_v2.securites;
using Imperatur_v2.handler;
using Imperatur_v2.events;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Imperatur_v2.account
{
    [DesignAttribute(true)]
    public class Account : IAccountInterface
    {
        private string LastErrorMessage;
        private Guid Identifier;
        private ObservableRangeCollection<ITransactionInterface> m_oTransactions;
        private AccountType AccountType;
        //private Guid AccountOwner;
        [DesignAttribute(true)]
        private string Name;
        [DesignAttribute(true, true)]
        private Customer Customer;

        [SerializeAttribute(SerializeAttributeType.DontSerialize)]
        public event AccountHandler.SaveAccountEventHandler SaveAccountEvent;

        #region constructor
        public Account(Customer Customer, AccountType AccountType, string AccountName)
        {
            PopulateNewAccount(Customer, AccountType, AccountName, null, Guid.NewGuid());
        }

        [JsonConstructor]
        public Account(Customer Customer, AccountType AccountType, string AccountName, ObservableRangeCollection<ITransactionInterface> m_oTransactions, Guid Identifier)
        {
            PopulateNewAccount(Customer, AccountType, AccountName, m_oTransactions, Identifier);
        }
        #endregion


        #region proctectedMethods
        protected virtual void OnSaveAccount(SaveAccountEventArg e)
        {
            if (SaveAccountEvent != null)
                SaveAccountEvent(this, e);
        }
        #endregion


        #region private
        private void Transactions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnSaveAccount(new SaveAccountEventArg()
            {
                Identifier = this.Identifier
            });
        }

        private void PopulateNewAccount(Customer Customer, AccountType AccountType, string AccountName, ObservableRangeCollection<ITransactionInterface> Transactions, Guid Identifier)
        {
            LastErrorMessage = "";
            this.Identifier = Identifier;
            this.Name = AccountName;
            this.AccountType = AccountType;
            this.Customer = Customer;
            if (Transactions != null && Transactions.Count > 0)
            {
                this.m_oTransactions = (ObservableRangeCollection<ITransactionInterface>)Transactions;
            }
            else
            {
                this.m_oTransactions = new ObservableRangeCollection<ITransactionInterface>();
                if (AccountType.Equals(AccountType.Customer))
                {
                    CreateZeroBalanceOnNewAccount();
                }

            }
            m_oTransactions.CollectionChanged -= Transactions_CollectionChanged;
            m_oTransactions.CollectionChanged += Transactions_CollectionChanged;
        }

        private void CreateZeroBalanceOnNewAccount()
        {
            IMoney ZeroBalance = ImperaturGlobal.Kernel.Get<IMoney>(
                new Ninject.Parameters.ConstructorArgument("m_oAmount", 0m),
                new Ninject.Parameters.ConstructorArgument("m_oCurrencyCode", ImperaturGlobal.GetSystemCurrency())
             );
            AddTransaction(
                CreateTransaction(ZeroBalance, GetBankAccountsFromCache().First(), Identifier, TransactionType.Transfer, null, "System")
                );
        }

        private ITransactionInterface CreateTransaction(IMoney NewTransaction, Guid DebitAccount, Guid CreditAccount, TransactionType TransactionType, ITradeInterface SecurtiesTrade, string ProcessCode = "Manual")
        {
            return ImperaturGlobal.Kernel.Get<ITransactionInterface>(
              new Ninject.Parameters.ConstructorArgument("_DebitAmount", NewTransaction),
              new Ninject.Parameters.ConstructorArgument("_CreditAmount", NewTransaction),
              new Ninject.Parameters.ConstructorArgument("_DebitAccount", DebitAccount),
              new Ninject.Parameters.ConstructorArgument("_CreditAccount", CreditAccount),
              new Ninject.Parameters.ConstructorArgument("_TransactionType", TransactionType),
              new Ninject.Parameters.ConstructorArgument("_SecurtiesTrade", SecurtiesTrade ?? (object)null),
              new Ninject.Parameters.ConstructorArgument("_ProcessCode", ProcessCode)

             );
        }

        private List<Guid> GetBusinessAccountsFromCache()
        {
            BusinessAccountCache oBA = (BusinessAccountCache)GlobalCachingProvider.Instance.GetItem(ImperaturGlobal.BusinessAccountCache);
            return oBA.GetCache().Select(b =>
                new Guid(b.Item1)).ToList();
        }

        #endregion

        #region public properties
        public string AccountName { get { return Name; } }

        Guid IAccountInterface.Identifier
        {
            get
            {
                return Identifier;
            }
        }

        public string GetLastErrorMessage
        {
            get
            {
                return LastErrorMessage;
            }
        }

        List<ITransactionInterface> IAccountInterface.Transactions
        {
            get
            {
                return new List<ITransactionInterface>((IEnumerable<ITransactionInterface>)this.m_oTransactions);
            }
        }



        public Customer GetCustomer()
        {
            return Customer;
        }

        #endregion


        #region public methods

        public bool AddTransaction(ITransactionInterface oTrans)
        {
            /*make sure the transaction is allowed
            * if withdrawal/transfer from customers account, the amount of availble funds must cover the amount of the transaction
            * if puchase of securites, the amount of availble funds must cover the amount of the transaction
            */
            try
            {
                List<IMoney> AvailableFunds = GetAvailableFunds().Where(t => t.CurrencyCode.Equals(oTrans.DebitAmount.CurrencyCode)).ToList();
                IMoney AvailableFundsCurrency = ImperaturGlobal.GetMoney(0m, oTrans.CreditAmount.CurrencyCode);

                if (AvailableFunds.Where(a => a.CurrencyCode.Equals(oTrans.CreditAmount.CurrencyCode)).Count() > 0)
                    AvailableFundsCurrency = AvailableFunds.Where(a => a.CurrencyCode.Equals(oTrans.CreditAmount.CurrencyCode)).First();

                if ((oTrans.TransactionType == TransactionType.Withdrawal || oTrans.TransactionType == TransactionType.Transfer || oTrans.TransactionType == TransactionType.Buy)
                   &&
                   oTrans.DebitAccount == this.Identifier
                   &&
                   oTrans.DebitAmount.Amount > AvailableFundsCurrency.Amount
                   )
                {
                    //abort transaction
                    LastErrorMessage = "Not enough available funds to cover this transaction!";
                    throw new Exception("Not enough available funds to cover this transaction!");
                }

                m_oTransactions.Add(oTrans);
            }
            catch (Exception ex)
            {
                ImperaturGlobal.GetLog().Error(string.Format("[Account.AddTransaction] Couldn't create transaction on account {0}", this.Identifier), ex);
                LastErrorMessage = ex.Message;
            }
            return true;
        }

        public List<Guid> GetBankAccountsFromCache()
        {
            return GetBusinessAccountsFromCache(AccountType.Bank);
        }

        public List<Guid> GetHouseAccountFromCache()
        {
            return GetBusinessAccountsFromCache(AccountType.House);
        }
        private List<Guid> GetBusinessAccountsFromCache(AccountType AccountTypeToFind)
        {
            BusinessAccountCache oBA = (BusinessAccountCache)GlobalCachingProvider.Instance.GetItem(ImperaturGlobal.BusinessAccountCache);
            return oBA.GetCache().Where(b => b.Item2.Equals(AccountTypeToFind.ToString())).Select(b =>
                  new Guid(b.Item1)).ToList();
        }

        /// <summary>
        /// How much amount is available for trade, per currency
        /// </summary>
        /// <returns>List of IMoney</returns>
        public List<IMoney> GetAvailableFunds()
        {

            List<TransactionType> TransferSell = new List<TransactionType>();
            TransferSell.Add(TransactionType.Sell);
            TransferSell.Add(TransactionType.Transfer);

            List<TransactionType> TransferBuyWithdraw = new List<TransactionType>();
            TransferBuyWithdraw.Add(TransactionType.Buy);
            TransferBuyWithdraw.Add(TransactionType.Transfer);
            TransferBuyWithdraw.Add(TransactionType.Withdrawal);

            //minus
            var DebitQuery =
                from t in m_oTransactions
                join hb in GetBusinessAccountsFromCache() on t.CreditAccount equals hb
                join tb in TransferBuyWithdraw on t.TransactionType equals tb
                select t;

            //plus
            var CreditQuery =
                from t in m_oTransactions
                join hb in GetBusinessAccountsFromCache() on t.DebitAccount equals hb
                join ts in TransferSell on t.TransactionType equals ts
                select t;

            List<IMoney> SumMoney = new List<IMoney>();
            SumMoney.AddRange(DebitQuery.Select(m => m.DebitAmount.SwitchSign));
            SumMoney.AddRange(CreditQuery.Select(m => m.CreditAmount));

            return (from p in SumMoney
                    group p.Amount by p.CurrencyCode into g
                    select
                      ImperaturGlobal.GetMoney(g.ToList().Sum(), g.Key)).ToList();
        }

        public List<IMoney> GetCurrentAmount()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// How much amount have been deposited, minus the withdrawal
        /// </summary>
        /// <returns>List of IMoney</returns>
        public List<IMoney> GetDepositedAmount()
        {
            List<TransactionType> TransferWithdraw = new List<TransactionType>();
            TransferWithdraw.Add(TransactionType.Transfer);
            TransferWithdraw.Add(TransactionType.Withdrawal);

            //minus
            var DebitQuery =
                from t in m_oTransactions
                where t.DebitAccount.Equals(this.Identifier)
                join tb in TransferWithdraw on t.TransactionType equals tb
                select t;

            //plus
            var CreditQuery =
                from t in m_oTransactions
                where t.CreditAccount.Equals(this.Identifier) && t.TransactionType.Equals(TransactionType.Transfer)
                select t;

            List<IMoney> SumMoney = new List<IMoney>();
            SumMoney.AddRange(DebitQuery.Select(m => m.DebitAmount.SwitchSign));
            SumMoney.AddRange(CreditQuery.Select(m => m.CreditAmount));


            return (List<IMoney>)(from p in SumMoney
                                  group p.Amount by p.CurrencyCode into g
                                  select
                                  ImperaturGlobal.GetMoney(g.ToList().Sum(), g.Key)).ToList();

        }

        public List<Holding> GetHoldings()
        {
            List<TransactionType> BuySell = new List<TransactionType>();
            BuySell.Add(TransactionType.Buy);
            BuySell.Add(TransactionType.Sell);

            //get all trade
            var HoldingQuery =
            from t in m_oTransactions
            join bs in BuySell on t.TransactionType equals bs
            select t;


            //get distinct tickers
            var Tickers = HoldingQuery.GroupBy(h => h.SecuritiesTrade.Security.Symbol)
                   .Select(grp => grp.First())
                   .ToList();

            List<Holding> Holdings = new List<Holding>();
            foreach (var Ticker in Tickers.Select(t => t.SecuritiesTrade.Security.Symbol))
            {

                Quote HoldingQuote = ImperaturGlobal.Quotes.Where(q => q.Symbol.Equals(Ticker)).First();

                Holding oH = new Holding();
                oH.Quantity = 0;
                oH.Name = Ticker;
                oH.Quantity += HoldingQuery.Where(h => h.SecuritiesTrade.Security.Symbol.Equals(Ticker) && h.TransactionType.Equals(TransactionType.Buy)).Sum(s => s.SecuritiesTrade.Quantity);
                oH.Quantity -= HoldingQuery.Where(h => h.SecuritiesTrade.Security.Symbol.Equals(Ticker) && h.TransactionType.Equals(TransactionType.Sell)).Sum(s => s.SecuritiesTrade.Quantity);

                oH.PurchaseAmount = GetGAAFromHolding(oH.Quantity, Ticker).Multiply(oH.Quantity);

                oH.CurrentAmount = HoldingQuote.LastTradePrice.Multiply(oH.Quantity);
                oH.Change = oH.CurrentAmount.Subtract(oH.PurchaseAmount);
                oH.ChangePercent = oH.PurchaseAmount.Amount != 0 ? (oH.CurrentAmount.Divide(oH.PurchaseAmount).Amount - 1) * 100 : 0;
                oH.Symbol = Ticker;
                Holdings.Add(oH);
            }

            return Holdings.Where(h => h.Quantity > 0).ToList();
        }

        public AccountType GetAccountType()
        {
            return AccountType;
        }


        public bool AddHoldingToAccount(int Quantity, string Symbol, ITradeHandlerInterface TradeHandler, string ProcessCode = "Manual")
        {
            ITradeInterface Trade = TradeHandler.GetTrade(Symbol, Quantity);
            try
            {
                AddTransaction(
                CreateTransaction(
                    Trade.TradeAmount,
                    this.Identifier,
                    GetHouseAccountFromCache().First(),
                    TransactionType.Buy,
                    Trade,
                    ProcessCode
                    )
                    );
            }
            catch (Exception ex)
            {
                ImperaturGlobal.GetLog().Error(string.Format("Couldn't add transaction to account {0}", this.Identifier), ex);
                LastErrorMessage = ex.Message;
                return false;
            }
            return true;
        }
        /// <summary>
        /// How much is the account worth(including deposit and the current value of holdings)
        /// </summary>
        /// <returns>List of IMoney</returns>
        public List<IMoney> GetTotalFunds()
        {
            List<IMoney> oM = GetAvailableFunds();
            oM.AddRange(GetHoldings().Select(h => h.CurrentAmount));

            return (List<IMoney>)(from p in oM
                                 group p.Amount by p.CurrencyCode into g
                                 select ImperaturGlobal.GetMoney(g.ToList().Sum(), g.Key)
                                 ).ToList();
        }

        private IMoney GetGAAFromHolding(decimal Quantity, string Ticker)
        {
            var Holdingtransactions = m_oTransactions.Where(t => t.DebitAccount.Equals(Identifier) && t.SecuritiesTrade.Security.Symbol.Equals(Ticker)).ToList();
            if (Holdingtransactions.Sum(h => h.GetQuantity()) < Quantity)
                return null;

            //calculate GAA
            return (from p in Holdingtransactions.Where(t => t.TransactionType.Equals(TransactionType.Buy))
                          group p.GetGAA().Amount by p.GetGAA().CurrencyCode into g
                          select ImperaturGlobal.GetMoney(g.ToList().Sum(), g.Key)).First().Divide(Convert.ToDecimal(Holdingtransactions.Where(t => t.TransactionType.Equals(TransactionType.Buy)).Count()));
        }

        private IMoney GetAACFromHoldingNoCheck(string Symbol)
        {
            var Holdingtransactions = m_oTransactions.Where(t => t.DebitAccount.Equals(Identifier) && t.SecuritiesTrade.Security.Symbol.Equals(Symbol)).ToList();
            //decimal g2 = Holdingtransactions.Sum(h => h.GetQuantity());
            if (Holdingtransactions.Sum(h => h.GetQuantity()) <= 0m)
                return null;

            //calculate GAA
            return (from p in Holdingtransactions.Where(t => t.TransactionType.Equals(TransactionType.Buy))
                    group p.GetGAA().Amount by p.GetGAA().CurrencyCode into g
                    select ImperaturGlobal.GetMoney(g.ToList().Sum(), g.Key)).First().Divide(Convert.ToDecimal(Holdingtransactions.Where(t => t.TransactionType.Equals(TransactionType.Buy)).Count()));
        }

        private IMoney GetRevenueFromHoldingSell(int Quantity, string Ticker)
        {
            IMoney GAA = GetGAAFromHolding(Quantity, Ticker);
            if (GAA == null)
            {
                throw new Exception("Not possible, not enough number of stocks");
            }


            //calculate revenue from the current ticker
            Quote HoldingQuote = ImperaturGlobal.Quotes.Where(q => q.Symbol.Equals(Ticker)).First();
            return HoldingQuote.LastTradePrice.Multiply(Quantity).Subtract(GAA.Multiply(Quantity));
        }

        public IMoney CalculateHoldingSell(int Quantity, string Ticker)
        {
            return GetRevenueFromHoldingSell(Quantity, Ticker);

        }
        public bool SellHoldingFromAccount(int Quantity, string Ticker, ITradeHandlerInterface TradeHandler, string ProcessCode = "Manual")
        {
            //first check that the quantity does not exceed the quantity of the security on the account
            IMoney GAA = GetGAAFromHolding(Quantity, Ticker);
            //calculate revenue from the current ticker
            Quote HoldingTicker = ImperaturGlobal.Quotes.Where(q => q.Symbol.Equals(Ticker)).First();
            IMoney oRevenue = GetRevenueFromHoldingSell(Quantity, Ticker);
            ITradeInterface Trade = TradeHandler.GetTrade(Ticker, Quantity, DateTime.Now, oRevenue); //minus for sell
            try
            {
                AddTransaction(
                CreateTransaction(
                    Trade.TradeAmount,
                    GetHouseAccountFromCache().First(), 
                    this.Identifier,
                    TransactionType.Sell,
                    Trade,
                    ProcessCode
                    )
                    );
            }
            catch (Exception ex)
            {
                ImperaturGlobal.GetLog().Error(string.Format("Couldn't create transaction for sell on account {0}", this.Identifier), ex);
                LastErrorMessage = ex.Message;
                return false;
            }
            return true;

        }

        public List<IMoney> GetDepositedAmount(List<ICurrency> FilterCurrency)
        {
          var DepositedAmount =
                   from t in GetDepositedAmount()
                    join fc in FilterCurrency on t.CurrencyCode equals fc
                    select t;
            return DepositedAmount.ToList();
        }

        public List<IMoney> GetAvailableFunds(List<ICurrency> FilterCurrency)
        {
            var AvailableFunds = 
                from t in GetAvailableFunds()
                join fc in FilterCurrency on t.CurrencyCode equals fc
                select t;
            return AvailableFunds.ToList();
        }

        public List<IMoney> GetTotalFunds(List<ICurrency> FilterCurrency)
        {
            var TotalFunds = 
                from t in GetTotalFunds()
                join fc in FilterCurrency on t.CurrencyCode equals fc
                select t;
            return TotalFunds.ToList();
        }

        public IMoney GetAverageAcquisitionCostFromHolding(string Symbol)
        {
            return GetAACFromHoldingNoCheck(Symbol);
        }

        #endregion
    }

    public enum AccountType
    {
        House,
        Customer,
        Bank
    }

    public class AccountCacheType
    {
        public Guid Identifier;
        public AccountType AccountType;
    }
}
