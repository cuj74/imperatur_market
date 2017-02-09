using Imperatur_Market_Core.shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_Market_Core.monetary
{
    public class LogicalTransactionHandler : ILogicalTransactionHandler
    {
        public int AddLogicalTransaction(LogicalTransaction logicalTransaction)
        {
            if (logicalTransaction.Transactions.Sum(x=>x.Monetary.Amount) == 0)
                return GetUserCollection().Insert(logicalTransaction);

            return -1;
        }
        private LiteDB.LiteCollection<LogicalTransaction> GetUserCollection()
        {
            return ImperaturGlobal.DatabaseHandler.GetCollectionFromDataBase<LogicalTransaction>();
        }
    }
}
