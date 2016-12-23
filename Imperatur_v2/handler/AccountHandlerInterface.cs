using Imperatur_v2.account;
using Imperatur_v2.monetary;
using Imperatur_v2.trade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_v2.handler
{
    public interface IAccountHandlerInterface
    {
        string GetLastErrorMessage();
        List<IMoney> GetDepositedAmountOnAccount(Guid Identifier);
        bool DepositAmount(Guid Identifier, Money Deposit);
        List<Holding> GetAccountHoldings(Guid Identifier);
        IAccountInterface GetAccount(Guid Identifier);
        Money CalculateHoldingSell(Guid Identifier, int Quantity, string Ticker);
        bool SellHoldingFromAccount(Guid Identifier, int Quantity, string Ticker);
        bool AddHoldingToAccount(Guid Identifier, int Quantity, string Ticker);
        bool AddTransactionToAccount(Guid Identifier, monetary.Transaction NewTransaction);
        List<Account> GetBankAccounts();
        List<Account> GetHouseAccounts();
        bool CreateAccount(List<IAccountInterface> oAccountData);
        bool SaveAccounts();
        List<Money> GetTotalFundsOfAccount(Guid Identifier);
        List<Guid> GetHouseAndBankAccountsGuid();
        List<IAccountInterface> Accounts();
        List<IAccountInterface> SearchAccount(string Search);
    }
}
