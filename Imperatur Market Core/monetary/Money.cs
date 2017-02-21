using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using System.Globalization;

namespace Imperatur_Market_Core.monetary
{
    public class Money : IMoney
    {
        private CurrencyCodes currencyCode;
        private double amountAsDouble;

        #region Constructors

        public Money() : this(0d, LocalCurrencyCode) { }
        public Money(string currencyCode) : this((CurrencyCodes)Enum.Parse(typeof(CurrencyCodes), currencyCode)) { }
        public Money(CurrencyCodes currencyCode) : this(0d, currencyCode) { }
        public Money(long amount) : this(amount, LocalCurrencyCode) { }
        [Inject]
        public Money(decimal amount) : this(amount, LocalCurrencyCode) { }
        public Money(double amount) : this(amount, LocalCurrencyCode) { }
        public Money(long amount, string currencyCode) : this(amount, (CurrencyCodes)Enum.Parse(typeof(CurrencyCodes), currencyCode)) { }
        public Money(decimal amount, string currencyCode) : this(amount, (CurrencyCodes)Enum.Parse(typeof(CurrencyCodes), currencyCode)) { }
        public Money(double amount, string currencyCode) : this(amount, (CurrencyCodes)Enum.Parse(typeof(CurrencyCodes), currencyCode)) { }
        public Money(long amount, CurrencyCodes currencyCode) : this((double)amount, currencyCode) { }
        public Money(decimal amount, CurrencyCodes currencyCode) : this((double)amount, currencyCode) { }
        public Money(double amount, CurrencyCodes currencyCode)
        {
            this.currencyCode = currencyCode;
            this.amountAsDouble = amount;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Represents the ISO code for the currency
        /// </summary>
        /// <returns>An Int16 with the ISO code for the current currency</returns>
        public Int16 ISOCode
        {
            get { return (Int16)currencyCode; }
            set { currencyCode = (CurrencyCodes)value; }
        }

        /// <summary>
        /// Accesses the internal representation of the value of the Money
        /// </summary>
        /// <returns>A decimal with the internal amount stored for this Money.</returns>
        public double InternalAmount
        {
            get { return amountAsDouble; }
            set { amountAsDouble = value; }
        }

        /// <summary>
        /// Rounds the amount to the number of significant decimal digits
        /// of the associated currency using MidpointRounding.AwayFromZero.
        /// </summary>
        /// <returns>A decimal with the amount rounded to the significant number of decimal digits.</returns>
        public decimal Amount
        {
            get
            {
                return Decimal.Round((Decimal)amountAsDouble, this.DecimalDigits, MidpointRounding.AwayFromZero);
            }
        }

        /// <summary>
        /// Truncates the amount to the number of significant decimal digits
        /// of the associated currency.
        /// </summary>
        /// <returns>A decimal with the amount truncated to the significant number of decimal digits.</returns>
        public decimal TruncatedAmount
        {
            get
            {
                return (decimal)((long)Math.Truncate(amountAsDouble * this.DecimalDigits)) / this.DecimalDigits;
            }
        }
        public static ICurrencyConverter Converter { get; set; }

        public string CurrencyCode
        {
            //here we should use Ninject toget ICurrency
            get { return Currency.Get(this.currencyCode).Code; }
        }

        public string CurrencySymbol
        {
            //here we should use Ninject toget ICurrency
            get { return Currency.Get(this.currencyCode).Symbol; }
        }

        public string CurrencyName
        {
            //here we should use Ninject toget ICurrency
            get { return Currency.Get(this.currencyCode).EnglishName; }
        }

        /// <summary>
        /// Gets the number of decimal digits for the associated currency.
        /// </summary>
        /// <returns>An int containing the number of decimal digits.</returns>
        public int DecimalDigits
        {
            //here we should use Ninject to get ICurrency...or should we??
            get { return Currency.Get(this.currencyCode).NumberFormat.CurrencyDecimalDigits; }
        }

        /// <summary>
        /// Gets the CurrentCulture from the CultureInfo object and creates a CurrencyCodes enum object.
        /// </summary>
        /// <returns>The CurrencyCodes enum of the current locale.</returns>
        public static CurrencyCodes LocalCurrencyCode
        {
            get { return (CurrencyCodes)Enum.Parse(typeof(CurrencyCodes), new RegionInfo(CultureInfo.CurrentCulture.LCID).ISOCurrencySymbol); }
        }

        public CurrencyCodes CurrencyCodes
        {
            get { return this.currencyCode; }
        }
        // public static ICurrencyConverter Converter { get; set; }

        public static bool AllowImplicitConversion = false;

        #endregion

        #region Money Operators

        public override int GetHashCode()
        {
            return Amount.GetHashCode() ^ currencyCode.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return (obj is IMoney) && Equals((IMoney)obj);
        }

        public bool Equals(IMoney other)
        {
            if (object.ReferenceEquals(other, null)) return false;
            if (AllowImplicitConversion)
                return Amount == other.Convert(currencyCode).Amount
                  && other.Amount == Convert(other.CurrencyCodes).Amount;
            else
                return ((CurrencySymbol == other.CurrencySymbol) && (Amount == other.Amount));
        }

        public static bool operator ==(Money first, Money second)
        {
            if (object.ReferenceEquals(first, second)) return true;
            if (object.ReferenceEquals(first, null) || object.ReferenceEquals(second, null)) return false;
            return first.Amount == second.ConvertOrCheck(first.currencyCode).Amount
              && second.Amount == first.ConvertOrCheck(second.currencyCode).Amount;
        }

        public static bool operator !=(Money first, Money second)
        {
            return !first.Equals(second);
        }

        public static bool operator >(Money first, Money second)
        {
            return first.Amount > second.ConvertOrCheck(first.currencyCode).Amount
              && second.Amount < first.Convert(second.currencyCode).Amount;
        }

        public static bool operator >=(Money first, Money second)
        {
            return first.Amount >= second.ConvertOrCheck(first.currencyCode).Amount
              && second.Amount <= first.Convert(second.currencyCode).Amount;
        }

        public static bool operator <=(Money first, Money second)
        {
            return first.Amount <= second.ConvertOrCheck(first.currencyCode).Amount
              && second.Amount >= first.Convert(second.currencyCode).Amount;
        }

        public static bool operator <(Money first, Money second)
        {
            return first.Amount < second.ConvertOrCheck(first.currencyCode).Amount
              && second.Amount > first.Convert(second.currencyCode).Amount;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;
            if (!(obj is IMoney))
                throw new ArgumentException("Argument must be Money");
            return CompareTo((IMoney)obj);
        }

        public int CompareTo(IMoney other)
        {
            if (this.Amount < other.Amount)
                return -1;
            if (this.Amount > other.Amount)
                return 1;
            return 0;
        }

        public static IMoney operator +(Money first, Money second)
        {
            //use ninject!
            return new Money(first.amountAsDouble + second.ConvertOrCheck(first.currencyCode).InternalAmount, first.currencyCode);
        }

        public static Money operator -(Money first, Money second)
        {
            return new Money(first.amountAsDouble - second.ConvertOrCheck(first.currencyCode).InternalAmount, first.currencyCode);
        }

        public static Money operator *(Money first, Money second)
        {
            return new Money(first.amountAsDouble * second.ConvertOrCheck(first.currencyCode).InternalAmount, first.currencyCode);
        }

        public static Money operator /(Money first, Money second)
        {
            return new Money(first.amountAsDouble / second.ConvertOrCheck(first.currencyCode).InternalAmount, first.currencyCode);
        }

        #endregion

        #region Cast Operators

        public static implicit operator Money(long amount)
        {
            return new Money(amount, LocalCurrencyCode);
        }

        public static implicit operator Money(decimal amount)
        {
            return new Money(amount, LocalCurrencyCode);
        }

        public static implicit operator Money(double amount)
        {
            return new Money(amount, LocalCurrencyCode);
        }

        public static bool operator ==(Money money, long value)
        {
            if (object.ReferenceEquals(money, null) || object.ReferenceEquals(value, null)) return false;
            return (money.Amount == (decimal)value);
        }
        public static bool operator !=(Money money, long value)
        {
            return !(money == value);
        }

        public static bool operator ==(Money money, decimal value)
        {
            if (object.ReferenceEquals(money, null) || object.ReferenceEquals(value, null)) return false;
            return (money.Amount == value);
        }
        public static bool operator !=(Money money, decimal value)
        {
            return !(money == value);
        }

        public static bool operator ==(Money money, double value)
        {
            if (object.ReferenceEquals(money, null) || object.ReferenceEquals(value, null)) return false;
            return (money.Amount == (decimal)value);
        }
        public static bool operator !=(Money money, double value)
        {
            return !(money == value);
        }

        public static Money operator +(Money money, long value)
        {
            return money + (double)value;
        }
        public static Money operator +(Money money, decimal value)
        {
            return money + (double)value;
        }
        public static Money operator +(Money money, double value)
        {
            if (money == null) throw new ArgumentNullException("money");
            return new Money(money.amountAsDouble + value, money.currencyCode);
        }

        public static Money operator -(Money money, long value)
        {
            return money - (double)value;
        }
        public static Money operator -(Money money, decimal value)
        {
            return money - (double)value;
        }
        public static Money operator -(Money money, double value)
        {
            if (money == null) throw new ArgumentNullException("money");
            return new Money(money.amountAsDouble - value, money.currencyCode);
        }

        public static Money operator *(Money money, long value)
        {
            return money * (double)value;
        }
        public static Money operator *(Money money, decimal value)
        {
            return money * (double)value;
        }
        public static Money operator *(Money money, double value)
        {
            if (money == null) throw new ArgumentNullException("money");
            return new Money(money.amountAsDouble * value, money.currencyCode);
        }

        public static Money operator /(Money money, long value)
        {
            return money / (double)value;
        }
        public static Money operator /(Money money, decimal value)
        {
            return money / (double)value;
        }
        public static Money operator /(Money money, double value)
        {
            if (money == null) throw new ArgumentNullException("money");
            return new Money(money.amountAsDouble / value, money.currencyCode);
        }

        #endregion

        #region Functions

        public IMoney Copy()
        {
            //here we should use Ninjet
            return new Money(Amount, currencyCode);
        }

        public IMoney Clone()
        {
            //here we should use Ninjet or not since we actually are within the money class we are working on!!
            return new Money(currencyCode);
        }

        public string ToString(bool genericFormatter)
        {
            return ToString("C", genericFormatter);
        }

        public string ToString(string format = "C", bool genericFormatter = false)
        {
            if (genericFormatter)
            {
                var formatter = (NumberFormatInfo)Currency.Get(LocalCurrencyCode).NumberFormat.Clone();
                formatter.CurrencySymbol = this.CurrencyCode;
                return Amount.ToString(format, formatter);
            }
            else
                return Amount.ToString(format, Currency.Get(this.currencyCode).NumberFormat);
        }

        /// <summary>
        /// Evenly distributes the amount over n parts, resolving remainders that occur due to rounding 
        /// errors, thereby garuanteeing the postcondition: result->sum(r|r.amount) = this.amount and
        /// x elements in result are greater than at least one of the other elements, where x = amount mod n.
        /// </summary>
        /// <param name="n">Number of parts over which the amount is to be distibuted.</param>
        /// <returns>Array with distributed Money amounts.</returns>
        public IMoney[] Allocate(int n)
        {
            double cents = Math.Pow(10, this.DecimalDigits);
            double lowResult = ((long)Math.Truncate((double)amountAsDouble / n * cents)) / cents;
            double highResult = lowResult + 1.0d / cents;

            //here we should use Ninject!!
            Money[] results = new Money[n];

            int remainder = (int)(((double)amountAsDouble * cents) % n);
            for (int i = 0; i < remainder; i++)
                results[i] = new Money((Decimal)highResult, currencyCode);
            for (int i = remainder; i < n; i++)
                results[i] = new Money((Decimal)lowResult, currencyCode);
            return results;
        }

        public IMoney Convert(CurrencyCodes toCurrency)
        {
            if (currencyCode == toCurrency)
                return this;
            if (Converter == null)
                throw new Exception("You need to assign an ICurrencyconverter to Money.Converter to automatically convert different currencies.");
            return Convert(toCurrency, Converter.GetRate(this.currencyCode, toCurrency, DateTime.Now));
        }

        public IMoney Convert(CurrencyCodes toCurrency, double rate)
        {
            return new Money(amountAsDouble * rate, toCurrency);
        }

        private IMoney ConvertOrCheck(CurrencyCodes toCurrency)
        {
            if (this.currencyCode == toCurrency)
                return this;
            else
                if (AllowImplicitConversion)
                return this.Convert(toCurrency);
            else
                throw new InvalidOperationException("Money type mismatch");
        }

        #endregion
    }

}

