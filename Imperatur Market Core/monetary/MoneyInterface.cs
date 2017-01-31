using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;


namespace Imperatur_Market_Core.monetary
{
    public interface IMoney : IComparable<IMoney>, IEquatable<IMoney>, IComparable
    {
        Int16 ISOCode { get; set; }
        double InternalAmount { get; set; }
        decimal Amount { get; }
        decimal TruncatedAmount { get; }
        string CurrencyCode { get; }
        CurrencyCodes CurrencyCodes { get; }
        string CurrencySymbol { get; }
        string CurrencyName { get; }
        int DecimalDigits { get; }
        int GetHashCode();
        bool Equals(object obj);
        IMoney Copy();
        IMoney Clone();
        string ToString(bool genericFormatter);
        string ToString(string format = "C", bool genericFormatter = false);
        IMoney[] Allocate(int n);
        IMoney Convert(CurrencyCodes toCurrency);
        IMoney Convert(CurrencyCodes toCurrency, double rate);
    }
}
