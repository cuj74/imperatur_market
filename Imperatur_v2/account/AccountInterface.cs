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
    public interface IAccountInterface 
    {
        Guid Identifier { get; }
        List<IMoney> GetCurrentAmount();
        List<IMoney> GetDepositedAmount();
        List<IMoney> GetDepositedAmount(List<ICurrency> FilterCurrency);
        bool AddTransaction(ITransactionInterface oTrans);
        List<Holding> GetHoldings();
        string[] GetSymbolInHoldings();
        List<IMoney> GetAvailableFunds();
        List<IMoney> GetTotalFunds();
        IMoney GetAverageAcquisitionCostFromHolding(string Symbol);
        List<IMoney> GetAvailableFunds(List<ICurrency> FilterCurrency);
        List<IMoney> GetTotalFunds(List<ICurrency> FilterCurrency);
        Customer GetCustomer();
        AccountType GetAccountType();
        List<Guid> GetBankAccountsFromCache();
        bool AddHoldingToAccount(int Quantity, string Symbol, ITradeHandlerInterface TradeHandler, string ProcessCode ="Manual");
        string GetLastErrorMessage { get; }
        string AccountName { get; }
        List<ITransactionInterface> Transactions { get; }
        event AccountHandler.SaveAccountEventHandler SaveAccountEvent;
        IMoney CalculateHoldingSell(int Quantity, string Ticker);
        bool SellHoldingFromAccount(int Quantity, string Ticker, ITradeHandlerInterface TradeHandler, string ProcessCode = "Manual");



    }
}
