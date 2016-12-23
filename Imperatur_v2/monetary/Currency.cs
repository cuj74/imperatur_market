using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.cache;
using System.Globalization;
using Imperatur_v2.shared;

namespace Imperatur_v2.monetary
{

    public interface ICurrency
    {
        /// <summary>
        /// Make sure the currency code applies to ISO standard and exists in the list
        /// </summary>
        /// <param name="CurrencyToTest"></param>
        void AssertCurrency(string CurrencyToTest);
        /// <summary>
        /// Get the underlying data of the currency
        /// </summary>
        /// <returns>object of type Currency</returns>
        //ICurrency GetData();
        string GetCurrencyString();
    }


    [Serializable]
    public struct Currency : IEquatable<Currency>, IFormatProvider, ICurrency
    {
        public string CurrencyCode
        {
            get
            {
                return _CurrencyCode;
            }
        }

        private readonly string _CurrencyCode;

        public Currency(string CurrencyCode)
        {
            CurrencyCode = CurrencyCode.ToUpper();
            _CurrencyCode = CurrencyCode;
            AssertCurrency(CurrencyCode);

        }
      
        //public ICurrency GetData()
        //{
        //    return this;
        //}
        public string GetCurrencyString()
        {
            return _CurrencyCode;
        }

        public void AssertCurrency(string CurrencyToTest)
        {
            if (CurrencyToTest.Length > 3)
            {
                Exception ex = new Exception(string.Format("CurrencyCode is restrained to {0} characters. '{1}' is not a valid currency code.", 3, CurrencyToTest));
                //Logger.Instance.Info(string.Format("AssertCurrency", ex));
                throw ex;
            }

            CurrencyCodeCache oC = (CurrencyCodeCache)GlobalCachingProvider.Instance.GetItem(ImperaturGlobal.CurrencyCodeCache);
            if (!oC.GetCache().Exists(c => c.Item1.Equals(CurrencyToTest)))
            {
                Exception ex = new Exception(string.Format("Value {0} is not an valid ISO currency code.", CurrencyToTest));
                //Logger.Instance.Info(string.Format("AssertCurrency", ex));
                throw ex;
            }
        }


        public override Int32 GetHashCode()
        {
            unchecked
            {
                return (397 * CurrencyCode.GetHashCode()) ^ _CurrencyCode.GetHashCode();
            }
        }
        public static Boolean TryParse(String s, out Currency currency)
        {
            currency = new Currency();
            try
            {
                currency = new Currency(s);
                return true;
            }
            catch
            {
                return false;
            }

        }

        public override Boolean Equals(Object obj)
        {
            if (!(obj is Currency))
            {
                return false;
            }

            var other = (Currency)obj;
            return Equals(other);
        }
        /// <summary>
        ///     Compares two currency values for equality.
        /// </summary>
        /// <param name="left">The left side to compare.</param>
        /// <param name="right">The right side to compare.</param>
        /// <returns>
        ///     <see langword="true" /> if they are equal; <see langword="false" /> otherwise.
        /// </returns>
        public static Boolean operator ==(Currency left, Currency right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Compares two currency values for inequality.
        /// </summary>
        /// <param name="left">The left side to compare.</param>
        /// <param name="right">The right side to compare.</param>
        /// <returns>
        ///     <see langword="true" /> if they are not equal; <see langword="false" /> otherwise.
        /// </returns>
        public static Boolean operator !=(Currency left, Currency right)
        {
            return !left.Equals(right);
        }

        #region IEquatable<Currency> Members

        public Boolean Equals(Currency other)
        {
            return _CurrencyCode == other._CurrencyCode;
        }

        #endregion

        #region IFormatProvider Members

        public Object GetFormat(Type formatType)
        {
            return null;
        }


        #endregion
    }

    public static class CurrencyTools
    {
        private static IDictionary<string, string> map;
        static CurrencyTools()
        {
            map = CultureInfo
                .GetCultures(CultureTypes.AllCultures)
                .Where(c => !c.IsNeutralCulture)
                .Select(culture => {
                    try
                    {
                        return new RegionInfo(culture.LCID);
                    }
                    catch
                    {
                        return null;
                    }
                })
                .Where(ri => ri != null)
                .GroupBy(ri => ri.ISOCurrencySymbol)
                .ToDictionary(x => x.Key, x => x.First().CurrencySymbol);
        }

        public static List<string> GetCurrencies()
        {
            return map.Select(m => m.Key).ToList();
        }

        public static bool TryGetCurrencySymbol(
                              string ISOCurrencySymbol,
                              out string symbol)
        {
            return map.TryGetValue(ISOCurrencySymbol, out symbol);
        }
    }
}

