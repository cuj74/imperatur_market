using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_Market_Core.monetary
{
    public interface ILogicalTransaction : IDisposable
    {
        bool AddTransaction(ITransaction Transaction);
        bool RemoveTransaction(ITransaction Transaction);
        ICollection<ITransaction> Transactions { get; }
        DateTime LogDateTime { get; }
        string LogicalTransactionType { get; }
        bool FinalizeLogicalTransaction();
    }
}
