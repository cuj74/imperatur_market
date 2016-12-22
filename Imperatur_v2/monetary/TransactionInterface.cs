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
        Money DebitAmount { get; }
        Guid CreditAccount { get; }
        Money CreditAmount { get; }

        Money GetGAA();
        Decimal GetQuantity();
        Money GetCurrentValue();
        Money GetTradeAmount();
    }
}
