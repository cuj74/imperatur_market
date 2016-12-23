using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.monetary;
using Imperatur_v2.trade;
using Imperatur_v2.customer;

namespace Imperatur_v2.account
{
    public interface IAccountInterface
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

    }
}
