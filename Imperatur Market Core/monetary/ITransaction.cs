using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_Market_Core.entity;

namespace Imperatur_Market_Core.monetary
{
    public interface ITransaction
    {
        IEntity SuperEntity { get; }
        IEntity InterEntity { get; }
        string TransactionType { get; }
        IMoney Monetary { get; }
    }
}
