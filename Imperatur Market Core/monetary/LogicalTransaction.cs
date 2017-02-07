using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_Market_Core.monetary
{
    public class LogicalTransaction : ILogicalTransaction
    {
        public DateTime LogDateTime  {get; set;}

        public string LogicalTransactionType{get; set;}


        public ICollection<ITransaction> Transactions{get; set;
        }


        public bool AddTransaction(ITransaction Transaction)
        {
            Transactions.Add(Transaction);
            return true;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool FinalizeLogicalTransaction()
        {
            throw new NotImplementedException();
        }

        public bool RemoveTransaction(ITransaction Transaction)
        {
            Transactions.Remove(Transaction);
            return true;
        }
    }
}
