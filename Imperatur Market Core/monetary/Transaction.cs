using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_Market_Core.entity;
using LiteDB;
using Imperatur_Market_Core.account;

namespace Imperatur_Market_Core.monetary
{
    public class Transaction
    {
        [BsonRef("Account")]
        public Account Account { get; set; }
        public IMoney Monetary  { get; set; }
        public string TransactionType  { get; set; }
    }
}
