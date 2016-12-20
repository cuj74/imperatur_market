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
        public static ImperaturData SystemData;
        public static List<Instrument> Instruments;
        #endregion

        #region Init

        static ICurrencyExhangeHandler _CurrencyExchangeHandler = null;

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

        internal static void Initialize(ImperaturData SystemDataToCache)
        {
            if (SystemData == null)
                SystemData = SystemDataToCache;

            if (Instruments == null)
                Instruments = ReadInstrumentsFromAssembly();

            BuildCurrencyCodeCache();

            if (!GlobalCachingProvider.Instance.FindItem(ImperaturGlobal.CountryCache))
                GlobalCachingProvider.Instance.AddItem(ImperaturGlobal.CountryCache, new CountryCache());

            //BuildCurrencyExchangeCache(BFS);

        }
        //CurrencyTools.GetCurrencies().ToArray()

        private static List<Instrument> ReadInstrumentsFromAssembly()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Imperatur_v2.cache.instrument.json";
            List<Instrument> oInstruments;
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                oInstruments = (List<Instrument>)json.DeserializeJSON.DeserializeObject(reader.ReadToEnd());
            }
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
