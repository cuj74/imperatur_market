using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_Market_Core.monetary
{
    public class LogicalTransaction
    {
        public int Id { get; set; }
        public DateTime LogDateTime  {get; set;}
        public string LogicalTransactionType{get; set;}
        public List<Transaction> Transactions{get; set;}
    }
}
