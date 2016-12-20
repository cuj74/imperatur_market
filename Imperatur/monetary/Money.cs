using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Imperatur.monetary
{
    public class Money
    {
        public string CurrencyCode;
        public Decimal Amount;


        public Money SwitchSign()
        {
            return new Money(-this.Amount, this.CurrencyCode);
        }

        public override string ToString()
        {
            return ToString(false, true);
        }
        public string ToString(bool WithSign, bool WithCurrencyCode)
        {
            //no need add a minussign!
            if (WithSign)
                return string.Format("{0}{1} {2}", Amount == 0 ? "" : Amount > 0? "+" : "", Math.Round(Amount, 2, MidpointRounding.AwayFromZero).ToString("#,0.00", System.Globalization.CultureInfo.GetCultureInfo("sv-SE")), WithCurrencyCode ? CurrencyCode.ToUpper().Trim(): "");
            else
                return string.Format("{0} {1}", Math.Round(Amount, 2, MidpointRounding.AwayFromZero).ToString("#,0.00", System.Globalization.CultureInfo.GetCultureInfo("sv-SE")), WithCurrencyCode ? CurrencyCode.ToUpper().Trim() : "");
        }

        public Money(decimal Amount, string CurrencyCode)
        {
            this.Amount = Amount;
            this.CurrencyCode = CurrencyCode;
        }

        public Money Multiply(Decimal Multiplier)
        {
            return new Money(this.Amount * Multiplier, this.CurrencyCode);
        }
        public Money Divide(Decimal Divider)
        {
            return this.Divide(new Money(Divider, this.CurrencyCode));
            
        }

        public Money Divide(Money Divider)
        {
            if (this.CurrencyCode != Divider.CurrencyCode) 
                throw new Exception("Can't divide two money objects with different currency");
            if (Divider.Amount == 0)
                throw new Exception("Can't divide by zero");

            return new Money(this.Amount / Divider.Amount, this.CurrencyCode);
        }
        public Money Add(Decimal Add)
        {
            return this.Add(new Money(Add, this.CurrencyCode));
            
        }
        public Money Add(Money Add)
        {
            if (this.CurrencyCode != Add.CurrencyCode)
                throw new Exception("Can't add two money objects with different currency");

            if (Add.Amount.Equals(0))
                return this;

            return new Money(this.Amount + Add.Amount, this.CurrencyCode);
        }

        public Money Subtract(Decimal Subtract)
        {
            return this.Subtract(new Money(Subtract, this.CurrencyCode));
        }
        public Money Subtract(Money Subtract)
        {
            if (this.CurrencyCode != Subtract.CurrencyCode)
                throw new Exception("Can't add two money objects with different currency");
            if (Subtract.Amount.Equals(0))
                return this;

            return new Money(this.Amount - Subtract.Amount, this.CurrencyCode);
        }

        public Money()
        {
        }
    }
}
