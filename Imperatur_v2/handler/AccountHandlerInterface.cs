using Imperatur_v2.account;
using Imperatur_v2.customer;
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
        bool DepositAmount(Guid Identifier, IMoney Deposit);
        List<Holding> GetAccountHoldings(Guid Identifier);
        IAccountInterface GetAccount(Guid Identifier);
        IMoney CalculateHoldingSell(Guid Identifier, int Quantity, string Ticker);
        bool SellHoldingFromAccount(Guid Identifier, int Quantity, string Ticker);
        bool AddHoldingToAccount(Guid Identifier, int Quantity, string Ticker);
        bool AddTransactionToAccount(Guid Identifier, ITransactionInterface NewTransaction);
        //List<Account> GetBankAccounts();
        //List<Account> GetHouseAccounts();
        bool CreateAccount(List<IAccountInterface> oAccountData);
        bool CreateAccount(Customer customer, AccountType accountType, string AccountName);
        bool SaveAccounts();
        List<Money> GetTotalFundsOfAccount(Guid Identifier);
        List<Tuple<string, decimal>> GetProfitPerForecast();
        //List<Guid> GetHouseAndBankAccountsGuid();
        List<IAccountInterface> Accounts();
        List<IAccountInterface> SearchAccount(string Search, AccountType AccountTypeToSearch);
    }
}
