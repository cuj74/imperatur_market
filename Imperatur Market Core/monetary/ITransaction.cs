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
    public interface ITransaction
    {
        IAccount Account { get; set; }
        string TransactionType { get; set; }
        IMoney Monetary { get; set; }
    }
}
