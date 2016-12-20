using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_v2.monetary
{
    public interface IMoney
    {
        Money SwitchSign();
        string ToString();
        string ToString(bool WithSign, bool WithCurrencyCode);
  //      Money(decimal Amount, string CurrencyCode);
        Money Multiply(Decimal Multiplier);
        Money Divide(Decimal Divider);
        Money Divide(Money Divider);
        Money Add(Decimal Add);
        Money Add(Money Add);
        Money Subtract(Decimal Subtract);
        Money Subtract(Money Subtract);
    }
}
