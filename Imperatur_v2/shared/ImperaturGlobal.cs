using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.cache;
using Imperatur_v2.monetary;
using Imperatur_v2.securites;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Ninject;

namespace Imperatur_v2.shared
{
    public sealed class ImperaturGlobal
    {
        //private static void OnTimedEvent(object source, ElapsedEventArgs e)
        //{
        //    //Do the stuff you want to be done every hour;
        //}


        #region cache
        public static string CurrencyCodeCache = "CURRENCYCODECACHE";
        public static string CountryCache = "COUNTRYCACHE";
        public static string CurrencyExchangeCache = "CURRENCYEXCHANGECACHE";
        public static string InstrumentCache = "INSTRUMENTCACHE";
        public static string BusinessAccountCache = "BUSINESSACCOUNTCACHE";
        public static ImperaturData SystemData;
        public static List<Instrument> Instruments;
        private static StandardKernel m_oKernel;
        #endregion

        #region Init

        static ICurrencyExhangeHandler _CurrencyExchangeHandler = null;
        public static StandardKernel Kernel
        {
            get {
                if (m_oKernel == null)
                {
                    m_oKernel = new StandardKernel();
                    m_oKernel.Load(Assembly.GetExecutingAssembly());
                }
                return m_oKernel;
            }
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

        internal static void Initialize(ImperaturData SystemDataToCache, StandardKernel NinjectKernel, List<account.AccountCacheType> BusinessAccounts)
        {
            if (SystemData == null)
                SystemData = SystemDataToCache;

            if (Instruments == null)
                Instruments = ReadInstrumentsFromAssembly();

            if (m_oKernel == null)
                m_oKernel = NinjectKernel;

            BuildCurrencyCodeCache();

            InitializeBusinessAccount(BusinessAccounts);


            if (!GlobalCachingProvider.Instance.FindItem(ImperaturGlobal.CountryCache))
                GlobalCachingProvider.Instance.AddItem(ImperaturGlobal.CountryCache, new CountryCache());

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
        #endregion
    }
}
