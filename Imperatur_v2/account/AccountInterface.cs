using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.monetary;
using Imperatur_v2.trade;

namespace Imperatur_v2.account
{
    public interface IAccountInterface
    {
        List<Money> GetCurrentAmount();
        List<Money> GetDepositedAmount();
        bool AddTransaction(Transaction oTrans);
        List<Holding> GetHoldings();
        List<Money> GetAvailableFunds();

    }
}
