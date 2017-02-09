using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_Market_Core.monetary
{
    public interface ILogicalTransactionHandler
    {
        int AddLogicalTransaction(LogicalTransaction logicalTransaction);
    }
}
