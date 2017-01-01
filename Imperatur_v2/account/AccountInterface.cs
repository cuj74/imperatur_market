using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.monetary;
using Imperatur_v2.trade;
using Imperatur_v2.customer;
using Imperatur_v2.handler;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Imperatur_v2.account
{
    public interface IAccountInterface //: INotifyPropertyChanged
    {
        Guid Identifier { get; }
        List<IMoney> GetCurrentAmount();
        List<IMoney> GetDepositedAmount();
        bool AddTransaction(ITransactionInterface oTrans);
        List<Holding> GetHoldings();
        List<IMoney> GetAvailableFunds();
        Customer GetCustomer();
        AccountType GetAccountType();
        List<Guid> GetBankAccountsFromCache();
        bool AddHoldingToAccount(int Quantity, string Symbol, ITradeHandlerInterface TradeHandler);
        string GetLastErrorMessage { get; }
        string AccountName { get; }
        List<ITransactionInterface> Transactions { get; }
        event AccountHandler.SaveAccountEventHandler SaveAccountEvent;

    }
}
