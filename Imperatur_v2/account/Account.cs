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
          
            List<IMoney> AvailableFunds = GetAvailableFunds().Where(t => t.CurrencyCode().Equals(oTrans.DebitAmount.CurrencyCode())).ToList();
            IMoney AvailableFundsCurrency = ImperaturGlobal.GetMoney(0m, oTrans.CreditAmount.CurrencyCode());

            if (AvailableFunds.Where(a => a.CurrencyCode().Equals(oTrans.CreditAmount.CurrencyCode())).Count() > 0)
                AvailableFundsCurrency = AvailableFunds.Where(a => a.CurrencyCode().Equals(oTrans.CreditAmount.CurrencyCode())).First();

            if ((oTrans.TransactionType == TransactionType.Withdrawal || oTrans.TransactionType == TransactionType.Transfer || oTrans.TransactionType == TransactionType.Buy)
               &&
               oTrans.DebitAccount == this.Identifier
               &&
               oTrans.DebitAmount.Amount() > AvailableFundsCurrency.Amount()
               )
            {
                //abort transaction
                throw new Exception("Not enough available funds to cover this transaction!");
            }

            Transactions.Add(oTrans);
            return true;
        }

        private List<Guid> GetBusinessAccountsFromCache()
        {
            BusinessAccountCache oBA = (BusinessAccountCache)GlobalCachingProvider.Instance.GetItem(ImperaturGlobal.BusinessAccountCache);
            return oBA.GetCache().Select(b=>
                new Guid(b.Item1)).ToList();
        }
        public List<Guid> GetBankAccountsFromCache()
        {
            BusinessAccountCache oBA = (BusinessAccountCache)GlobalCachingProvider.Instance.GetItem(ImperaturGlobal.BusinessAccountCache);
            return oBA.GetCache().Where(b=>b.Item2.Equals(AccountType.Bank.ToString())).Select(b =>
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

            List<IMoney> SumMoney = new List<IMoney>();
            SumMoney.AddRange(DebitQuery.Select(m => m.DebitAmount.SwitchSign()));
            SumMoney.AddRange(CreditQuery.Select(m => m.CreditAmount));

            return (from p in SumMoney
                                  group p.Amount() by p.CurrencyCode() into g
                                  select
                                    ImperaturGlobal.GetMoney(g.ToList().Sum(), g.Key)).ToList();
        }

        public List<IMoney> GetCurrentAmount()
        {
            throw new NotImplementedException();
        }

        public List<IMoney> GetDepositedAmount()
        {
            List<TransactionType> TransferWithdraw = new List<TransactionType>();
            TransferWithdraw.Add(TransactionType.Transfer);
            TransferWithdraw.Add(TransactionType.Withdrawal);

            //minus
            var DebitQuery =
                from t in Transactions
                where t.DebitAccount.Equals(this.Identifier)
                join tb in TransferWithdraw on t.TransactionType equals tb
                select t;

            //plus
            var CreditQuery =
                from t in Transactions
                where t.CreditAccount.Equals(this.Identifier) && t.TransactionType.Equals(TransactionType.Transfer)
                select t;

            List<IMoney> SumMoney = new List<IMoney>();
            SumMoney.AddRange(DebitQuery.Select(m => m.DebitAmount.SwitchSign()));
            SumMoney.AddRange(CreditQuery.Select(m => m.CreditAmount));


            return (List<IMoney>)(from p in SumMoney
                                 group p.Amount() by p.CurrencyCode() into g
                                 select 
                                 ImperaturGlobal.GetMoney(g.ToList().Sum(), g.Key)).ToList();
  
        }

        public List<Holding> GetHoldings()
        {
            throw new NotImplementedException();
        }

        public AccountType GetAccountType()
        {
            return AccountType;
        }

        public bool AddHoldingToAccount(int Quantity, string Symbol)
        {
            // Quote oHoldingTicker = _Quotes.Where(q => q.Symbol.Equals(Ticker)).First();
            /*
            Trade oNewTrade = new Imperatur.trade.Trade
            {
                TradeAmount = oHoldingTicker.Dividend.Multiply(Quantity),
                Quantity = Quantity,
                AverageAcquisitionValue = oHoldingTicker.Dividend,
                Security = (securities.Securities)oHoldingTicker,
                TradeDateTime = DateTime.Now
            };

            Transaction oT = new Transaction(
                oHoldingTicker.Dividend.Multiply(Quantity),
                oHoldingTicker.Dividend.Multiply(Quantity),
                Identifier,
                HouseAccounts[0].Identifier,
                TransactionType.Buy,
                oNewTrade
                );
            try
            {
                AddTransactionToAccount(Identifier, oT);
            }
            catch (Exception ex)
            {
                LastErrorMessage = ex.Message;
                return false;
            }*/
            return true;
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
