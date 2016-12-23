using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_v2.monetary
{
    public interface ITransactionInterface
    {
        Guid DebitAccount { get; }
        TransactionType TransactionType { get; }
        IMoney DebitAmount { get; }
        Guid CreditAccount { get; }
        IMoney CreditAmount { get; }

        IMoney GetGAA();
        Decimal GetQuantity();
        IMoney GetCurrentValue();
        IMoney GetTradeAmount();
    }
}
