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
           decimal trt = logicalTransaction.Transactions.Sum(x => x.Monetary.Amount);
            try
            {
                if (logicalTransaction.Transactions.Sum(x => x.Monetary.Amount) == 0)
                    return GetCollection().Insert(logicalTransaction);
            }
            catch (Exception ex)
            {
                int gg = 0;
            }

           return -1;
        }
        private LiteDB.LiteCollection<LogicalTransaction> GetCollection()
        {
            return ImperaturGlobal.DatabaseHandler.GetCollectionFromDataBase<LogicalTransaction>();
        }
    }
}
