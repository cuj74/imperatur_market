using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.cache;
using Imperatur_v2.SweaWebService;
using Imperatur_v2.shared;
using System.ServiceModel;

namespace Imperatur_v2.monetary
{

    public interface ICurrencyExhangeHandler
    {
        CurrencyExchange GetExchangedAmount(decimal AmountFrom, ICurrency FromCurrency, ICurrency ToCurrency, DateTime ExchangeDate);
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
        public CurrencyExchange GetExchangedAmount(decimal AmountFrom, ICurrency FromCurrency, ICurrency ToCurrency, DateTime ExchangeDate)
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
        public ICurrency Currency;
        public DateTime Date;
    }
    /// <summary>
    /// Used for creating the currency cachce
    /// </summary>
    public class CurrencyInfoCache
    {
        public string Currency;
    }




    public class CurrencyExchange
    {
        public decimal ToAmount;
        public decimal FromAmount;
        public decimal ExchangeRate;
        public ICurrency FromCurrency;
        public ICurrency ToCurrency;
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

    public class CurrencyDataFromExternalSource
    {
        const string SEKTOEURSERIE = "SEKEURPMI";
        const string SEKTOUSDSERIE = "SEKUSDPMI";
        const int GroupId = 130;
        public CurrencyDataFromExternalSource()
        {

        }
        public List<CurrencyInfo> GetCurrentCurrencyExchangeRate()//ICurrency FromCurrency, ICurrency ToCurrency)
        {
            string[] series = new string[] { SEKTOEURSERIE, SEKTOUSDSERIE };
            List<CurrencyInfo> oCurrencyInfo = new List<CurrencyInfo>();
            try
            {
                string s_RemoteAddress = "https://swea.riksbank.se:443/sweaWS/services/SweaWebServiceHttpSoap12Endpoint";
                Uri oRemoteUri;
                try
                {
                    oRemoteUri = new Uri(s_RemoteAddress);
                }
                catch (System.Exception ex)
                {
                    throw new System.Exception(string.Format("Remoteaddress is not correct formatted {0} ({1})", s_RemoteAddress, ex.Message));
                }

                BasicHttpSecurityMode oSecuritymode = oRemoteUri.Scheme.Equals("https", StringComparison.InvariantCultureIgnoreCase) ? BasicHttpSecurityMode.Transport : BasicHttpSecurityMode.None;
                BasicHttpBinding oBinding = new BasicHttpBinding(oSecuritymode);
                oBinding.MaxReceivedMessageSize = int.MaxValue;
                oBinding.MaxBufferSize = int.MaxValue;
                SweaWebServicePortTypeClient oClient =  new SweaWebServicePortTypeClient(oBinding, new EndpointAddress(s_RemoteAddress));


                //SweaWebServicePortTypeClient oClient = new SweaWebServicePortTypeClient(System.ServiceModel.BasicHttpsBinding , "https://swea.riksbank.se:443/sweaWS/services/SweaWebServiceHttpSoap12Endpoint");
                Result oResult = oClient.getLatestInterestAndExchangeRates(LanguageType.en,
                    series
                    );



                foreach (var ResultGroup in oResult.groups)
                {
                    foreach (var ResultSerie in ResultGroup.series)
                    {
                        oCurrencyInfo.AddRange(ResultSerie.resultrows.Select(s =>
                             new CurrencyInfo
                             {
                                 Currency = ResultSerie.seriesid.Trim().Equals(SEKTOUSDSERIE) ? ImperaturGlobal.GetMoney(0, "USD").CurrencyCode : ImperaturGlobal.GetMoney(0, "EUR").CurrencyCode,
                                 Date = s.date.Value,
                                 Price = Convert.ToDecimal(s.value)
                             }
                            ).ToArray());
                    }

                }
            }
            catch(System.Exception ex)
            {
                int gg = 0;
            }

            return oCurrencyInfo;

        }


    }
}
