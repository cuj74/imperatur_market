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


namespace Imperatur_v2.account
{
    [DesignAttribute(true)]
    public class Account : IAccountInterface
    {
        private Guid Identifier;
        private List<ITransactionInterface> Transactions;
        private AccountType AccountType;
        private Guid AccountOwner;
        [DesignAttribute(true)]
        private string Name;
        [DesignAttribute(true, true)]
        private Customer Customer;

        Guid IAccountInterface.Identifier
        {
            get
            {
                return Identifier;
            }
        }

        public Account(Customer Customer, AccountType AccountType, string AccountName)
        {
            Identifier = Guid.NewGuid();
            this.Name = AccountName;
            this.AccountType = AccountType;
            this.Customer = Customer;
            Transactions = new List<ITransactionInterface>(); //use ItransactionInterface later on!!
        }

        public Customer GetCustomer()
        {
            return Customer;
        }

        public bool AddTransaction(ITransactionInterface oTrans)
        {
            /*make sure the transaction is allowed
            * if withdrawal/transfer from customers account, the amount of availble funds must cover the amount of the transaction
            * if puchase of securites, the amount of availble funds must cover the amount of the transaction
            */
           // BusinessAccountCache oBA = (BusinessAccountCache)GlobalCachingProvider.Instance.GetItem(ImperaturGlobal.BusinessAccountCache);
            /*

            if (!oC.GetCache().Exists(c => c.Item1.Equals(CurrencyToTest)))
            {
                Exception ex = new Exception(string.Format("Value {0} is not an valid ISO currency code.", CurrencyToTest));
                Logger.Instance.Info(string.Format("AssertCurrency", ex));
                throw ex;
            }
            */
            //List<Money> AvailableFunds = GetAvailableFunds().Where(t => t.CurrencyCode.Equals(oTrans.DebitAmount.CurrencyCode)).ToList();
            /*
            List<Money> AvailableFunds = GetAvailableFunds(HouseOrBanks).Where(t => t.CurrencyCode.Equals(oTrans.DebitAmount.CurrencyCode)).ToList();
            Money AvailableFundsCurrency = new Money(0, oTrans.CreditAmount.CurrencyCode);
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
                throw new Exception("Not enough available funds to cover this transaction!");
            }
            */
            Transactions.Add(oTrans);
            return true;
        }

        private List<Guid> GetBusinessAccountsFromCache()
        {
            BusinessAccountCache oBA = (BusinessAccountCache)GlobalCachingProvider.Instance.GetItem(ImperaturGlobal.BusinessAccountCache);
            return oBA.GetCache().Select(b=>
                new Guid(b.Item1)).ToList();
        }

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
                from t in Transactions
                join hb in GetBusinessAccountsFromCache() on t.CreditAccount equals hb
                join tb in TransferBuyWithdraw on t.TransactionType equals tb
                select t;

            //plus
            var CreditQuery =
                from t in Transactions
                join hb in GetBusinessAccountsFromCache() on t.DebitAccount equals hb
                join ts in TransferSell on t.TransactionType equals ts
                select t;

            List<Money> SumMoney = new List<Money>();
            SumMoney.AddRange(DebitQuery.Select(m => m.DebitAmount.SwitchSign()));
            SumMoney.AddRange(CreditQuery.Select(m => m.CreditAmount));

            //här ska vi egentligen använda ninject kernel för att få tillbaka rätt typ av Money
            return (List<IMoney>)(from p in SumMoney
                                 group p.Amount by p.CurrencyCode into g
                                 select (IMoney)new Money(g.ToList().Sum(), new Currency(g.Key.ToString()))).ToList();


        }

        public List<IMoney> GetCurrentAmount()
        {
            throw new NotImplementedException();
        }

        public List<IMoney> GetDepositedAmount()
        {
            throw new NotImplementedException();
        }

        public List<Holding> GetHoldings()
        {
            throw new NotImplementedException();
        }

        public AccountType GetAccountType()
        {
            return AccountType;
        }
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
