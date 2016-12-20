using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.monetary;
using Imperatur_v2.trade;

namespace Imperatur_v2.account
{
    public class Account : IAccountInterface
    {
        public bool AddTransaction(Transaction oTrans)
        {
            throw new NotImplementedException();
        }

        public List<Money> GetAvailableFunds()
        {
            throw new NotImplementedException();
        }

        public List<Money> GetCurrentAmount()
        {
            throw new NotImplementedException();
        }

        public List<Money> GetDepositedAmount()
        {
            throw new NotImplementedException();
        }

        public List<Holding> GetHoldings()
        {
            throw new NotImplementedException();
        }
    }
}
