using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_Market_Core.entity;

namespace Imperatur_Market_Core.monetary
{
    public class Transaction : ITransaction
    {
        public IEntity InterEntity
        { get; set; }
        public IMoney Monetary
        { get; set; }
        public IEntity SuperEntity
        { get; set; }
        public string TransactionType
        { get; set; }
    }
}
