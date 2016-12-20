using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.cache;

namespace Imperatur_v2.monetary
{

    public interface ICurrencyExhangeHandler
    {
        CurrencyExchange GetExchangedAmount(decimal AmountFrom, Currency FromCurrency, Currency ToCurrency, DateTime ExchangeDate);
        CurrencyExchange GetExchangedAmount(CurrencyExchange ObjectToExchange);

    }

    //This class can, since it depends on decimal type be constructed without instantiate the decimal part - this is intended so it can be used for calculating 
    //the rate between two currencies
    public class CurrencyExhangeHandler : ICurrencyExhangeHandler
    {
        private List<CurrencyInfo> m_oCurrencyInfo;


        public CurrencyExhangeHandler(List<CurrencyInfo> CurrencyInfoList)
        {
            m_oCurrencyInfo = CurrencyInfoList;

        }
        public CurrencyExchange GetExchangedAmount(decimal AmountFrom, Currency FromCurrency, Currency ToCurrency, DateTime ExchangeDate)
        {
            return CurrencyExchangeInternal(new CurrencyExchange()
            {
                FromAmount = AmountFrom,
                FromCurrency = FromCurrency,
                ToCurrency = ToCurrency,
                ExchangeDate = ExchangeDate
            });
        }
        public CurrencyExchange GetExchangedAmount(CurrencyExchange ObjectToExchange)
        {
            return CurrencyExchangeInternal(ObjectToExchange);
        }

        private CurrencyExchange CurrencyExchangeInternal(CurrencyExchange ToExchange)
        {
            List<CurrencyExchange> oCurrencyToExchange = new List<CurrencyExchange>();
            oCurrencyToExchange.Add(ToExchange);
            var cur = (
                from t in oCurrencyToExchange
                join cn in m_oCurrencyInfo on new { CurrencyCode = t.FromCurrency.CurrencyCode, PriceDate = t.ExchangeDate } equals new { cn.Currency.CurrencyCode, PriceDate = cn.Date }
                join cf in m_oCurrencyInfo on new { CurrencyCode = t.ToCurrency.CurrencyCode, PriceDate = t.ExchangeDate } equals new { cf.Currency.CurrencyCode, PriceDate = cf.Date }
                select new
                {
                    ToAmount = t.FromAmount * (cn.Price * cf.Price),
                    ExhangeRate = (cn.Price * cf.Price)
                }
                ).ToArray();
            ToExchange.ToAmount = cur.First().ToAmount;
            ToExchange.ExchangeRate = cur.First().ExhangeRate;
            return ToExchange;
        }
    }

    public class CurrencyInfo
    {
        public decimal Price;
        public Currency Currency;
        public DateTime Date;
        //public Guid BFSid;
    }
    /// <summary>
    /// Used for creating the currency cachce
    /// </summary>
    public class CurrencyInfoCache
    {
        public string Currency;
        //public Guid BFSid;
    }




    public class CurrencyExchange
    {
        public decimal ToAmount;
        public decimal FromAmount;
        public decimal ExchangeRate;
        public Currency FromCurrency;
        public Currency ToCurrency;
        public DateTime ExchangeDate;
    }



    public class CurrencyExchangeCache : ICache
    {

        private List<Tuple<string, string, string>> m_oCurrencyExhange;

        public CurrencyExchangeCache(CurrencyInfo[] ListOfCurrencyExchange)
        {
            m_oCurrencyExhange = new List<Tuple<string, string, string>>();
            m_oCurrencyExhange.AddRange(
                ListOfCurrencyExchange.Select(x =>
            Tuple.Create(
                                     x.Currency.CurrencyCode,
                                     x.Date.ToString("yyyy-MM-dd"),
                                     x.Price.ToString()
                                    )
            ).ToArray()
            );
        }

        public List<Tuple<string, string, string>> GetCache()
        {
            return m_oCurrencyExhange;
        }
    }
}
