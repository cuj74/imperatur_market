using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_v2.monetary
{
    public interface IMoney
    {
        IMoney SwitchSign();
        string ToString();
        string ToString(bool WithSign, bool WithCurrencyCode);
  //      Money(decimal Amount, string CurrencyCode);
        IMoney Multiply(Decimal Multiplier);
        IMoney Divide(Decimal Divider);
        IMoney Divide(IMoney Divider);
        IMoney Add(Decimal Add);
        IMoney Add(IMoney Add);
        IMoney Subtract(Decimal Subtract);
        IMoney Subtract(IMoney Subtract);
        ICurrency CurrencyCode();
        decimal Amount();
    }
}
