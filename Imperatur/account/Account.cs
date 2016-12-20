using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur.securities;
using Imperatur.monetary;
using Imperatur.trade;

namespace Imperatur.account
{
    public class Account : IAccountInterface
    {
        public Guid Identifier;
        public List<Transaction> Transactions;  //use ItransactionInterface later on!!
        public AccountType AccountType;
        public Guid AccountOwner;
        public string Name;
        public List<Money> GetCurrentAmount()
        {
            List<Money> oM = new List<Money>();
            

            //return the latest value of each purchased security
           // Transactions.Select(t=>t.GetCurrentValue)

            return oM;
        }
        public List<Money> GetDepositedAmount()
        {

            List<TransactionType> TransferWithdraw = new List<TransactionType>();
            TransferWithdraw.Add(TransactionType.Transfer);
            TransferWithdraw.Add(TransactionType.Withdrawal);

 /*           List<TransactionType> TransferSell = new List<TransactionType>();
            TransferSell.Add(TransactionType.Transfer);
            TransferSell.Add(TransactionType.Sell);*/

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

            List<Money> SumMoney = new List<Money>();
            SumMoney.AddRange(DebitQuery.Select(m => m.DebitAmount.SwitchSign()));
            SumMoney.AddRange(CreditQuery.Select(m => m.CreditAmount));


            return (List<Money>)(from p in SumMoney
                                 group p.Amount by p.CurrencyCode into g
                                 select new Money { CurrencyCode = g.Key, Amount = g.ToList().Sum() }).ToList();
        }
        public bool AddTransaction(Transaction oTrans, List<Guid> HouseOrBanks)
        {
            /*make sure the transaction is allowed
             * if withdrawal/transfer from customers account, the amount of availble funds must cover the amount of the transaction
             * if puchase of securites, the amount of availble funds must cover the amount of the transaction
             */
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

            Transactions.Add(oTrans);
            return true;
        }
        /// <summary>
        /// Holdings without the current value and change, use the method at the accounthandler level instead!
        /// </summary>
        /// <returns>List of holdings</returns>
        public List<Holding> GetHoldings()
        {
            List<TransactionType> BuySell = new List<TransactionType>();
            BuySell.Add(TransactionType.Buy);
            BuySell.Add(TransactionType.Sell);
            
            //get all trade
            var HoldingQuery =
            from t in Transactions
            join bs in BuySell on t.TransactionType equals bs
            select t;


            //get distinct tickers
            var Tickers = HoldingQuery.GroupBy(h => h._SecuritiesTrade.Security.Symbol)
                   .Select(grp => grp.First())
                   .ToList();

            List<Holding> Holdings = new List<Holding>();
            foreach (var Ticker in Tickers.Select(t => t._SecuritiesTrade.Security.Symbol))
            {
                Holding oH = new Holding();
                oH.Name = Ticker;
                oH.Quantity = HoldingQuery.Where(h => h._SecuritiesTrade.Security.Symbol.Equals(Ticker)).Sum(s => s._SecuritiesTrade.Quantity);
                oH.PurchaseAmount =
                    new Money
                    {
                        Amount = HoldingQuery.Where(h => h._SecuritiesTrade.Security.Symbol.Equals(Ticker)).Sum(s => s._SecuritiesTrade.TradeAmount.Amount),
                        CurrencyCode = HoldingQuery.Where(h => h._SecuritiesTrade.Security.Symbol.Equals(Ticker)).First().DebitAmount.CurrencyCode
                    };
                Holdings.Add(oH);
            }

            return Holdings.Where(h => h.Quantity > 0).ToList();
        }
     
        public List<Money> GetAvailableFunds(List<Guid> HouseOrBanks)
        {
            //later on the house or banks should be available from global cache!!!
            List<TransactionType> TransferSell = new List<TransactionType>();
            TransferSell.Add(TransactionType.Sell);
            TransferSell.Add(TransactionType.Transfer);

            List<TransactionType> TransferBuyWithdraw = new List<TransactionType>();
            TransferBuyWithdraw.Add(TransactionType.Buy);
            TransferBuyWithdraw.Add(TransactionType.Transfer);
            TransferBuyWithdraw.Add(TransactionType.Withdrawal);

            //minus
            var DebitQuery=
                from t in Transactions
                join hb in HouseOrBanks on t.CreditAccount equals hb
                join tb in TransferBuyWithdraw on t.TransactionType equals tb
                select t;

            //plus
            var CreditQuery =
                from t in Transactions
                join hb in HouseOrBanks on t.DebitAccount equals hb
                join ts in TransferSell on t.TransactionType equals ts
                select t;

            List<Money> SumMoney = new List<Money>();
            SumMoney.AddRange(DebitQuery.Select(m => m.DebitAmount.SwitchSign()));
            SumMoney.AddRange(CreditQuery.Select(m => m.CreditAmount));


            return (List<Money>)(from p in SumMoney
                     group p.Amount by p.CurrencyCode into g
                          select new Money { CurrencyCode = g.Key, Amount = g.ToList().Sum() }).ToList();

        

        }
        public Account(string AccountName, AccountType AccountType)
        {
            Identifier = Guid.NewGuid();
            this.Name = AccountName;
            this.AccountType = AccountType;
            Transactions = new List<Transaction>(); //use ItransactionInterface later on!!
        }
        

    }
    public enum AccountType
    {
        House,
        Customer,
        Bank
    }
}
