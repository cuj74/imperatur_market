using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_v2.monetary
{
    public class Money : IMoney
    {
        public Currency CurrencyCode;
        public Decimal Amount;

        public Money Add(Money Add)
        {
            throw new NotImplementedException();
        }

        public Money Add(decimal Add)
        {
            throw new NotImplementedException();
        }

        public Money Divide(Money Divider)
        {
            throw new NotImplementedException();
        }

        public Money Divide(decimal Divider)
        {
            throw new NotImplementedException();
        }

        public Money Multiply(decimal Multiplier)
        {
            throw new NotImplementedException();
        }

        public Money Subtract(Money Subtract)
        {
            throw new NotImplementedException();
        }

        public Money Subtract(decimal Subtract)
        {
            throw new NotImplementedException();
        }

        public Money SwitchSign()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return ToString(false, true);
        }

        public string ToString(bool WithSign, bool WithCurrencyCode)
        {
            throw new NotImplementedException();
        }

        public Money(decimal Amount, string CurrencyCode)
        {
            this.Amount = Amount;
            this.CurrencyCode = new Currency(CurrencyCode);
        }

        public Money(decimal Amount, Currency CurrencyCode)
        {
            this.Amount = Amount;
            this.CurrencyCode = CurrencyCode;
        }

    }
}
