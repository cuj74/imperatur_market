using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.cache;
using Imperatur_v2.shared;
using Imperatur_v2.monetary;
using Imperatur_v2.securites;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Ninject;
using Imperatur_v2.events;
using Imperatur_v2.json;
using Imperatur_v2.trade.recommendation;
using log4net;
using log4net.Config;


namespace Imperatur_v2.shared
{
    public enum ExchangeStatus
    {
        Closed,
        Open,
        Undefined
    }

    //TODO: Create an interface for this class
    public sealed class ImperaturGlobal
    {
        #region cache
        public static string CurrencyCodeCache = "CURRENCYCODECACHE";
        public static string CountryCache = "COUNTRYCACHE";
        public static string CurrencyExchangeCache = "CURRENCYEXCHANGECACHE";
        public static string InstrumentCache = "INSTRUMENTCACHE";
        public static string BusinessAccountCache = "BUSINESSACCOUNTCACHE";
        public static ImperaturData SystemData;
        public static List<Instrument> Instruments;
        public static List<TradingRecommendation> TradingRecommendations;
        public static List<Quote> Quotes;
        private static List<CurrencyInfo> m_oCurrencyRates;
        private static ExchangeStatus m_oExchangeStatus;
        private static Tuple<DateTime, DateTime> m_oOpeningHours;
        private readonly static string LogName = "log4net";

        private static StandardKernel m_oKernel;

        public static List<Tuple<int, int>> BankDays;
        public static List<Tuple<int, int>> HalfDay;

        #endregion


        #region public methods
        public static ILog GetLog()
        {
            return GetLog(null, ImperaturGlobal.SystemData.SystemDirectory);
        }

        public static ILog GetLogWithDirectory(string LogDirectory)
        {
            return GetLog(null, LogDirectory);
        }

        private static ILog GetLog(string logName, string LogDirectory)
        {
            if (logName == null)
            {
                logName = LogName;
            }
            //log4net.GlobalContext.Properties["LogFileName"] = string.Format("{0}\\{1}{2}", ImperaturGlobal.SystemData.SystemDirectory, "log\\imp", DateTime.Now.ToString("yyyy-mm-dd")); //log file path
            log4net.GlobalContext.Properties["LogFileName"] = string.Format("{0}\\{1}{2}", LogDirectory, "log\\imp", DateTime.Now.ToString("yyyy-MM-dd")); //log file path
            log4net.Config.XmlConfigurator.Configure();
            ILog log = LogManager.GetLogger(logName);
            return log;
        }
        #endregion

        #region Init

        static ICurrencyExhangeHandler _CurrencyExchangeHandler = null;
        public static StandardKernel Kernel
        {
            get
            {
                if (m_oKernel == null)
                {
                    m_oKernel = new StandardKernel();
                    m_oKernel.Load(Assembly.GetExecutingAssembly());
                }
                return m_oKernel;
            }
        }
        public static List<CurrencyInfo> CurrencyRates
        {
            get {
                if (m_oCurrencyRates == null)
                {
                    SetCurrencyExhangeRatesForToday();
                }
                return m_oCurrencyRates;
            }
        }

        public static List<TradingRecommendation> GetTradingRecommendation(string Symbol)
        {
            if (TradingRecommendations != null && TradingRecommendations.Count() > 0 && TradingRecommendations.Where(tr => tr.Instrument != null && tr.Instrument.Symbol.Equals(Symbol)).Count() > 0)
            {
                return TradingRecommendations.Where(tr => tr.Instrument != null && tr.Instrument.Symbol.Equals(Symbol)).ToList();
            }
            return null;
        }

        public static decimal GetPriceForCurrencyToday(ICurrency Currency)
        {
            return CurrencyRates.Where(c => c.Currency.Equals(Currency)).First().Price;
        }
        public static ICurrency GetSystemCurrency()
        {
            return m_oKernel.Get<ICurrency>(
                new Ninject.Parameters.ConstructorArgument("CurrencyCode", SystemData.SystemCurrency)
                );
        }

        public static IMoney GetMoney(decimal Amount, string CurrencyCode)
        {
            return m_oKernel.Get<IMoney>(
                                  new Ninject.Parameters.ConstructorArgument("m_oAmount", Amount),
                                  new Ninject.Parameters.ConstructorArgument("m_oCurrencyCode",
                                    m_oKernel.Get<ICurrency>(
                                    new Ninject.Parameters.ConstructorArgument("CurrencyCode", CurrencyCode))
                                    ));
        }
        public static IMoney GetMoney(decimal Amount, ICurrency Currency)
        {
            return GetMoney(Amount, Currency.CurrencyCode);
            /*
            string t = Currency.ToString();
            int g = 0;
            return ImperaturGlobal.Kernel.Get<IMoney>(
                                  new Ninject.Parameters.ConstructorArgument("Amount", Amount),
                                  new Ninject.Parameters.ConstructorArgument("Currency", Currency)
                                    );*/
        }

        internal static ICurrencyExhangeHandler CurrencyExchangeHandler
        {
            get
            {
                if (_CurrencyExchangeHandler == null)
                {

                }
                return _CurrencyExchangeHandler;
            }
        }

        public static ExchangeStatus ExchangeStatus
        {
            get
            {
                return CurrentExchangeStatus(new Exchange { ExhangeCode = SystemData.Exchange });
            }
        }

 

        internal static void Initialize(ImperaturData SystemDataToCache, StandardKernel NinjectKernel, List<account.AccountCacheType> BusinessAccounts)
        {
            if (SystemData == null)
                SystemData = SystemDataToCache;

            if (Instruments == null)
                Instruments = ReadInstrumentsFromAssembly();

            if (m_oKernel == null)
                m_oKernel = NinjectKernel;

            BuildCurrencyCodeCache();

            try
            {
                BuildHistoricalPriceCache();
            }
            catch (Exception ex)
            {
                GetLog().Error("Error ", ex);
            }

            InitializeBusinessAccount(BusinessAccounts);
            
            if (!GlobalCachingProvider.Instance.FindItem(ImperaturGlobal.CountryCache))
                GlobalCachingProvider.Instance.AddItem(ImperaturGlobal.CountryCache, new CountryCache());

            m_oCurrencyRates = new CurrencyDataFromExternalSource().GetCurrentCurrencyExchangeRate();

            //month, day
            BankDays = new List<Tuple<int, int>>();
            HalfDay = new List<Tuple<int, int>>();

            BankDays.Add(new Tuple<int, int>(1, 1));
            BankDays.Add(new Tuple<int, int>(1, 6));
            BankDays.Add(new Tuple<int, int>(3, 28));
            BankDays.Add(new Tuple<int, int>(5, 5));
            BankDays.Add(new Tuple<int, int>(6, 6));
            BankDays.Add(new Tuple<int, int>(6, 24));
            BankDays.Add(new Tuple<int, int>(6, 26));

            //to 13:00
            HalfDay.Add(new Tuple<int, int>(1, 5));
            HalfDay.Add(new Tuple<int, int>(3, 24));
            HalfDay.Add(new Tuple<int, int>(5, 4));
            HalfDay.Add(new Tuple<int, int>(11, 4));

        }

        private static void SetCurrencyExhangeRatesForToday()
        {
            m_oCurrencyRates = new CurrencyDataFromExternalSource().GetCurrentCurrencyExchangeRate();
        }

        internal static void InitializeBusinessAccount(List<account.AccountCacheType> BusinessAccounts)
        {
            if (BusinessAccounts != null)
            {
                if (!GlobalCachingProvider.Instance.FindItem(ImperaturGlobal.BusinessAccountCache))
                    GlobalCachingProvider.Instance.AddItem(ImperaturGlobal.BusinessAccountCache, new BusinessAccountCache(
                        BusinessAccounts.ToArray()
                        ));
            }
        }
        private static List<Instrument> ReadInstrumentsFromAssembly()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Imperatur_v2.cache.instruments.json";
            List<Instrument> oInstruments;
            string JsonInstrumentdata;
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                JsonInstrumentdata = reader.ReadToEnd();
            }
            JArray oJsonData = JArray.Parse(JsonInstrumentdata);
            oInstruments = oJsonData.ToObject<List<Instrument>>();
            return oInstruments;
        }

        private static void BuildInstrumentCache()
        {
            if (!GlobalCachingProvider.Instance.FindItem(ImperaturGlobal.InstrumentCache))
            {
                GlobalCachingProvider.Instance.AddItem(ImperaturGlobal.InstrumentCache, new InstrumentCache(ReadInstrumentsFromAssembly().ToArray()));
            }
        }


        private static void BuildCurrencyCodeCache()
        {
            if (!GlobalCachingProvider.Instance.FindItem(ImperaturGlobal.CurrencyCodeCache))
                GlobalCachingProvider.Instance.AddItem(ImperaturGlobal.CurrencyCodeCache, new CurrencyCodeCache(
                    CurrencyTools.GetCurrencies().Select(
                        c => new CurrencyInfoCache()
                        {
                            Currency = c
                        }).ToArray()
                    ));
        }



        private static void BuildCurrencyExchangeCache()
        {
            /*
            if (!GlobalCachingProvider.Instance.FindItem(ImperaturGlobal.CurrencyCodeCache))
                BuildCurrencyCodeCache();

            CurrencyCodeCache oCurrencyCache = (CurrencyCodeCache)GlobalCachingProvider.Instance.GetItem(ImperaturGlobal.CurrencyCodeCache);

            if (!GlobalCachingProvider.Instance.FindItem(ImperaturGlobal.CurrencyExchangeCache))
                GlobalCachingProvider.Instance.AddItem(ImperaturGlobal.CurrencyExchangeCache, new CurrencyExchangeCache(
                     BFS.GetCurrencyInfo(oCurrencyCache.GetCache().Select(t => new Guid(t.Item3)).ToArray()).ToArray()
                    )
                );
                */
        }

        //add to another class
        private static void BuildHistoricalPriceCache()
        {

            //return;
            foreach (Instrument i in Instruments)
            {
                HistoricalQuote oH = new HistoricalQuote(null, null, null);
                DateTime oDataFromNeeded = DateTime.Now;
                bool bReadMore = false;
                bool bAllHistoricalDataNeeded = true;

                string FullPath = GetFullPathOfHistoricalDataForInstrument(i);
                if (File.Exists(FullPath))
                {
                    oH = (HistoricalQuote)DeserializeJSON.DeserializeObjectFromFile(FullPath);
                    if (oH == null)
                    {
                        continue;
                    }

                    //get the latest date to see if we need to add
                    if (oH.HistoricalQuoteDetails != null && oH.HistoricalQuoteDetails.Count > 0 && (DateTime.Now.Date - oH.HistoricalQuoteDetails.Max(h => h.Date).Date.AddDays(1).Date).Days >= 1)
                    {
                        bReadMore = true;
                        bAllHistoricalDataNeeded = false;
                        oDataFromNeeded = oH.HistoricalQuoteDetails.Max(h => h.Date).Date.AddDays(1).Date;
                    }
                    else
                    {
                        //all is up to date!
                        bAllHistoricalDataNeeded = false;
                    }
                }

                if (!bReadMore && bAllHistoricalDataNeeded)
                {
                    try
                    {
                        oH = GetHistoricalQuoteOnline(i, new Exchange { ExhangeCode = SystemData.Exchange });
                        SerializeJSONdata.SerializeObject(oH, FullPath);
                    }
                    catch (Exception ex)
                    {
                        int lg = 0;
                    }
                }
                else if (bReadMore)
                {
                    try
                    {
                        HistoricalQuote oHnew = GetHistoricalQuoteOnline(i, new Exchange { ExhangeCode = SystemData.Exchange }, oDataFromNeeded);

                        oH.HistoricalQuoteDetails.AddRange(oHnew.HistoricalQuoteDetails.Where(h=>h.Date.Date > oDataFromNeeded.Date).ToList());
                        SerializeJSONdata.SerializeObject(oH, FullPath);
                    }
                    catch (Exception ex)
                    {
                        int geg = 0;
                    }
                }
            }
            int gg = 0;
        }



        /// <summary>
        /// Calculates number of business days, taking into account:
        ///  - weekends (Saturdays and Sundays)
        ///  - bank holidays in the middle of the week
        /// </summary>
        /// <param name="firstDay">First day in the time interval</param>
        /// <param name="lastDay">Last day in the time interval</param>
        /// <param name="bankHolidays">List of bank holidays excluding weekends</param>
        /// <returns>Number of business days during the 'span'</returns>
        public static int BusinessDaysUntil(DateTime firstDay, DateTime lastDay)
        {
            firstDay = firstDay.Date;
            lastDay = lastDay.Date;
            if (firstDay > lastDay)
                throw new ArgumentException("Incorrect last day " + lastDay);

            TimeSpan span = lastDay - firstDay;
            int businessDays = span.Days + 1;
            int fullWeekCount = businessDays / 7;
            // find out if there are weekends during the time exceedng the full weeks
            if (businessDays > fullWeekCount * 7)
            {
                // we are here to find out if there is a 1-day or 2-days weekend
                // in the time interval remaining after subtracting the complete weeks
                int firstDayOfWeek = (int)firstDay.DayOfWeek;
                int lastDayOfWeek = (int)lastDay.DayOfWeek;
                if (lastDayOfWeek < firstDayOfWeek)
                    lastDayOfWeek += 7;
                if (firstDayOfWeek <= 6)
                {
                    if (lastDayOfWeek >= 7)// Both Saturday and Sunday are in the remaining time interval
                        businessDays -= 2;
                    else if (lastDayOfWeek >= 6)// Only Saturday is in the remaining time interval
                        businessDays -= 1;
                }
                else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7)// Only Sunday is in the remaining time interval
                    businessDays -= 1;
            }

            // subtract the weekends during the full weeks in the interval
            businessDays -= fullWeekCount + fullWeekCount;
            DateTime[] bankHolidays = BankDays.Select(x => new DateTime(DateTime.Now.Year, x.Item1, x.Item2)).ToArray();
            // subtract the number of bank holidays during the time interval
            foreach (DateTime bankHoliday in bankHolidays)
            {
                DateTime bh = bankHoliday.Date;
                if (firstDay <= bh && bh <= lastDay)
                    --businessDays;
            }

            return businessDays;
        }


        public static ExchangeStatus CurrentExchangeStatus(Exchange exchange)
        {
            try
            {
                if (m_oOpeningHours == null)
                {
                    GoogleHistoricalDataInterpreter oGHDI = new GoogleHistoricalDataInterpreter();
                    m_oOpeningHours = oGHDI.GetStartEndOfExchange(Instruments[0], exchange);
                }
                if ((DateTime.Now >= m_oOpeningHours.Item1 && DateTime.Now <= m_oOpeningHours.Item2))
                {
                    m_oExchangeStatus = ExchangeStatus.Open;
                }
                else
                {
                    m_oExchangeStatus = ExchangeStatus.Closed;
                }
            }
            catch
            {
                m_oExchangeStatus = ExchangeStatus.Undefined;
            }
            return m_oExchangeStatus;
        }

        public static HistoricalQuote GetHistoricalQuoteOnline(Instrument instrument, Exchange exchange)
        {
            GoogleHistoricalDataInterpreter oGHDI = new GoogleHistoricalDataInterpreter();
            return oGHDI.GetHistoricalData(instrument, exchange);
        }
        public static HistoricalQuote GetHistoricalQuoteOnline(Instrument instrument, Exchange exchange, DateTime FromDate)
        {
            GoogleHistoricalDataInterpreter oGHDI = new GoogleHistoricalDataInterpreter();
            return oGHDI.GetHistoricalData(instrument, exchange, FromDate);
        }

        private static string GetFullPathOfHistoricalDataForInstrument(Instrument Instrument)
        {
            string FileName = ImperaturGlobal.SystemData.HistoricalQuoteFile.Replace("{exchange}", SystemData.Exchange).Replace("{symbol}", Instrument.Symbol);
            return string.Format(@"{0}\{1}\{2}\{3}", ImperaturGlobal.SystemData.SystemDirectory, ImperaturGlobal.SystemData.QuoteDirectory, ImperaturGlobal.SystemData.HistoricalQuoteDirectory, FileName);
        }
        public static HistoricalQuote HistoricalQuote(Instrument Instrument)
        {
            return (HistoricalQuote)DeserializeJSON.DeserializeObjectFromFile(GetFullPathOfHistoricalDataForInstrument(Instrument));
        }
        #endregion
    }
}

