using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur.account;
using Imperatur.monetary;
using Imperatur.orm;
using Imperatur.cache;
using Imperatur.trade;


namespace Imperatur.handler
{
    public class AccountHandler : AccountHandlerInterface
    {
        private List<Account> _Accounts;
        private accountORM m_oAccountORM;
        private List<Quote> _Quotes;
        private string LastErrorMessage;


        public AccountHandler(List<Quote> Quotes)
        {
            _Quotes = Quotes;
            m_oAccountORM = new accountORM();
            LastErrorMessage = "";
        }

        public string GetLastErrorMessage
        {
            get
            {
                if (LastErrorMessage == null)
                    return "No error encountered";
                else
                    return LastErrorMessage;
            }
        }
        public List<Account> Accounts
        {
            get {
                if (_Accounts == null)
                    _Accounts = m_oAccountORM.ReadAccounts();

                return _Accounts;
            }
        }

        public List<Money> GetDepositedAmountOnAccount(Guid Identifier)
        {
            List<Money> oM = _Accounts.Single(a => a.Identifier.Equals(Identifier)).GetDepositedAmount();
            if (oM.Count == 0)
            {
                return null;
            }
            //ugly and wrong but works right now!
            return oM;// new Money(oM.Sum(m => m.Amount), oM[0].CurrencyCode);
        }

        public bool DepositAmount(Guid Identifier, Money Deposit)
        {
            Account oA = _Accounts.Single(a => a.Identifier.Equals(Identifier));
            Transaction oD = new Transaction(
                Deposit,
                Deposit,
                BankAccounts[0].Identifier,
                oA.Identifier,
                TransactionType.Transfer,
                null
                );
            oA.AddTransaction(oD, HouseAndBankAccountsGuid);
            return true;
        }

        public List<Holding> GetAccountHoldings(Guid Identifier)
        {
            List<Holding> oH =_Accounts.Single(a => a.Identifier.Equals(Identifier)).GetHoldings();
            foreach (Holding h in oH)
            {
                Quote oHoldingTicker = _Quotes.Where(q => q.Symbol.Equals(h.Name)).First();
                h.CurrentAmount = new Money(h.Quantity * oHoldingTicker.Dividend.Amount, oHoldingTicker.Dividend.CurrencyCode);
                h.Change = new Money(h.CurrentAmount.Amount - h.PurchaseAmount.Amount, h.CurrentAmount.CurrencyCode);
                h.ChangePercent = ((h.CurrentAmount.Amount / h.PurchaseAmount.Amount) - 1) * 100;
             }
            return oH;
        }

        public Account GetAccount(Guid Identifier)
        {
            return _Accounts.Single(a => a.Identifier.Equals(Identifier));
        }

        public Money CalculateHoldingSell(Guid Identifier, int Quantity, string Ticker)
        {
            Account oA = GetAccount(Identifier);
            var Holdingtransactions = oA.Transactions.Where(t => t.DebitAccount.Equals(oA.Identifier) && t._SecuritiesTrade.Security.Symbol.Equals(Ticker)).ToList();
            if (Holdingtransactions.Sum(h => h.GetQuantity()) < Quantity)
                return null;

            //calculate GAA
            Money GAA = (from p in Holdingtransactions.Where(t => t.TransactionType.Equals(TransactionType.Buy))
                         group p.GetGAA().Amount by p.GetGAA().CurrencyCode into g
                         select new Money { CurrencyCode = g.Key, Amount = g.ToList().Sum() }).First().Divide(Holdingtransactions.Where(t => t.TransactionType.Equals(TransactionType.Buy)).Count()); ;

            //calculate revenue from the current ticker
            Quote oHoldingTicker = _Quotes.Where(q => q.Symbol.Equals(Ticker)).First();
            return oHoldingTicker.Dividend.Multiply(Quantity).Subtract(GAA.Multiply(Quantity));

        }
        public bool SellHoldingFromAccount(Guid Identifier, int Quantity, string Ticker)
        {
            //first check that the quantity does not exceed the quantity of the security on the account
            Account oA = GetAccount(Identifier);
            var Holdingtransactions = oA.Transactions.Where(t => t.DebitAccount.Equals(oA.Identifier) && t._SecuritiesTrade.Security.Symbol.Equals(Ticker)).ToList();
            if (Holdingtransactions.Sum(h => h.GetQuantity()) < Quantity)
                return false;

            //calculate GAA
            Money GAA = (from p in Holdingtransactions.Where(t => t.TransactionType.Equals(TransactionType.Buy))
                          group p.GetGAA().Amount by p.GetGAA().CurrencyCode into g
                          select new Money { CurrencyCode = g.Key, Amount = g.ToList().Sum()}).First().Divide(Holdingtransactions.Where(t => t.TransactionType.Equals(TransactionType.Buy)).Count()); ;
            
            //calculate revenue from the current ticker
            Quote oHoldingTicker = _Quotes.Where(q => q.Symbol.Equals(Ticker)).First();
            Trade oSellTrade = new Imperatur.trade.Trade
            {
                TradeAmount = oHoldingTicker.Dividend.Multiply(Quantity).SwitchSign(),
                Quantity = -Quantity,
                Revenue = oHoldingTicker.Dividend.Multiply(Quantity).Subtract(GAA.Multiply(Quantity)),
                Security = (securities.Securities)oHoldingTicker,
                TradeDateTime = DateTime.Now
            };
            Transaction oT = new Transaction(
               oHoldingTicker.Dividend.Multiply(Quantity),
               oHoldingTicker.Dividend.Multiply(Quantity),
               HouseAccounts[0].Identifier,
               Identifier,
               TransactionType.Sell,
               oSellTrade
               );
            try
            {
                AddTransactionToAccount(Identifier, oT);
            }
            catch (Exception ex)
            {
                LastErrorMessage = ex.Message;
                return false;
            }
            return true;



            //GetAccount(Identifier).Transactions.Where(t=>t.CreditAccount.Equals(Identifier) && t._SecuritiesTrade.Quantity)
        }


        public bool AddHoldingToAccount(Guid Identifier, int Quantity, string Ticker)
        {
            Quote oHoldingTicker = _Quotes.Where(q => q.Symbol.Equals(Ticker)).First();
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
            }
            return true;
        }

        public bool AddTransactionToAccount(Guid Identifier, monetary.Transaction NewTransaction)
        {
            _Accounts.Single(a => a.Identifier.Equals(Identifier)).AddTransaction(NewTransaction, HouseAndBankAccountsGuid);
            return true;
        }


        public List<Account> BankAccounts
        {
            get { return _Accounts.Where(a => a.AccountType.Equals(AccountType.Bank)).ToList(); }
        }
        public List<Account> HouseAccounts
        {
            get { return _Accounts.Where(a => a.AccountType.Equals(AccountType.House)).ToList(); }
        }

        public bool CreateAccount(List<Account> oAccountsData)
        {
            if (_Accounts == null)
                _Accounts = new List<Account>();

            _Accounts.AddRange(oAccountsData);
            return true;
        }

        public bool SaveAccounts()
        {
            m_oAccountORM.SaveAllAccounts(_Accounts);
            return true;
        }
        public List<Money> GetTotalFundsOfAccount(Guid Identifier)
        {
            Account oA = GetAccount(Identifier);
            List<Money> oM = oA.GetAvailableFunds(HouseAndBankAccountsGuid);
            oM.AddRange(GetAccountHoldings(Identifier).Select(h => h.CurrentAmount));

            return (List<Money>)(from p in oM
                                 group p.Amount by p.CurrencyCode into g
                                 select new Money { CurrencyCode = g.Key, Amount = g.ToList().Sum() }).ToList();


        }

        public List<Guid> HouseAndBankAccountsGuid
        {

            get
            {
                List<Guid> HouseAndBankAccounts = new List<Guid>();
                HouseAndBankAccounts.AddRange(this.BankAccounts.Select(b => b.Identifier).ToList());
                HouseAndBankAccounts.AddRange(this.HouseAccounts.Select(b => b.Identifier).ToList());

                return HouseAndBankAccounts;
            }
        }

    }
}
