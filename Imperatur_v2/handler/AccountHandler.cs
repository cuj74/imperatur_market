using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.monetary;
using Imperatur_v2.account;
using Imperatur_v2.trade;

namespace Imperatur_v2.handler
{
    public class AccountHandler : IAccountHandlerInterface
    {
        private List<IAccountInterface> _Accounts;
        private string LastErrorMessage;

        public AccountHandler()
        {
            LastErrorMessage = "";
        }

        public bool AddHoldingToAccount(Guid Identifier, int Quantity, string Ticker)
        {
            throw new NotImplementedException();
        }

        public bool AddTransactionToAccount(Guid Identifier, Transaction NewTransaction)
        {
            throw new NotImplementedException();
        }

        public Money CalculateHoldingSell(Guid Identifier, int Quantity, string Ticker)
        {
            throw new NotImplementedException();
        }

        public bool CreateAccount(List<Account> oAccountsData)
        {
            throw new NotImplementedException();
        }

        public bool DepositAmount(Guid Identifier, Money Deposit)
        {
            throw new NotImplementedException();
        }

        public Account GetAccount(Guid Identifier)
        {
            throw new NotImplementedException();
        }

        public List<Holding> GetAccountHoldings(Guid Identifier)
        {
            throw new NotImplementedException();
        }

        public List<Account> GetBankAccounts()
        {
            throw new NotImplementedException();
        }

        public List<Money> GetDepositedAmountOnAccount(Guid Identifier)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public bool SellHoldingFromAccount(Guid Identifier, int Quantity, string Ticker)
        {
            throw new NotImplementedException();
        }

        public List<IAccountInterface> Accounts()
        {

            throw new NotImplementedException();
        }
    }
}
