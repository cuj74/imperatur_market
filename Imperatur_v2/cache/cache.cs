using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;
using System.Globalization;
using System.Web;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using Imperatur_v2.monetary;
using Imperatur_v2.securites;

namespace Imperatur_v2.cache
{

    public interface ICache
    {
        /// <summary>
        /// Get the current cache
        /// </summary>
        /// <returns>List&lt;Tuple&lt;string, string, string&gt;&gt;</returns>
        List<Tuple<string, string, string>> GetCache();
    }

    public interface IGlobalCachingProvider
    {
        void AddItem(string key, object value);
        object GetItem(string key);

    }

    public abstract class CachingProviderBase
    {
        public CachingProviderBase()
        {

        }

        protected MemoryCache cache = new MemoryCache("CachingProvider");

        static readonly object padlock = new object();

        protected virtual bool FindItem(string key)
        {
            lock (padlock)
            {
                return (cache[key] != null) ? true : false;
            }
        }

        protected virtual void AddItem(string key, object value)
        {
            lock (padlock)
            {
                cache.Add(key, value, DateTimeOffset.MaxValue);
            }
        }

        protected virtual void RemoveItem(string key)
        {
            lock (padlock)
            {
                cache.Remove(key);
            }
        }

        protected virtual object GetItem(string key, bool remove)
        {
            lock (padlock)
            {
                var res = cache[key];

                if (res != null)
                {
                    if (remove == true)
                        cache.Remove(key);
                }
                else
                {
                    throw new Exception("Item not found in cache");
                }
                return res;
            }
        }
    }


    public class GlobalCachingProvider : CachingProviderBase, IGlobalCachingProvider
    {
        #region Singleton

        protected GlobalCachingProvider()
        {
        }

        public static GlobalCachingProvider Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }
            internal static readonly GlobalCachingProvider instance = new GlobalCachingProvider();
        }

        #endregion

        #region ICachingProvider

        public virtual new void AddItem(string key, object value)
        {
            base.AddItem(key, value);
        }


        public virtual new bool FindItem(string key)
        {
            return base.FindItem(key);
        }

        public virtual object GetItem(string key)
        {
            return base.GetItem(key, false);//Remove default is true because it's Global Cache!
        }

        public virtual new object GetItem(string key, bool remove)
        {
            return base.GetItem(key, remove);
        }


        #endregion

    }
    public class InstrumentCache : ICache
    {
        private List<Tuple<string, string, string>> m_oInstruments;

        public List<Tuple<string, string, string>> GetCache()
        {
            return m_oInstruments;
        }
        public InstrumentCache(Instrument[] ListofInstruments)
        {
            m_oInstruments = new List<Tuple<string, string, string>>();
            m_oInstruments.AddRange(
            (
                       from i in ListofInstruments
                       select Tuple.Create(
                            i.Symbol,
                            i.Name,
                            i.CurrencyCode
                           )
                       ).ToArray()
            );
        }
    }

    public class BusinessAccountCache : ICache
    {
        private List<Tuple<string, string, string>> m_oBusinessAccount;

        public List<Tuple<string, string, string>> GetCache()
        {
            return m_oBusinessAccount;
        }
        public BusinessAccountCache(account.AccountCacheType[] ListofBusinessAccount)
        {
            m_oBusinessAccount = new List<Tuple<string, string, string>>();
            m_oBusinessAccount.AddRange(
            (
                       from i in ListofBusinessAccount
                       select Tuple.Create(
                            i.Identifier.ToString(),
                            i.AccountType.ToString(),
                            ""
                           )
                       ).ToArray()
            );
        }
    }


    public class CurrencyCodeCache : ICache
    {
        private List<Tuple<string, string, string>> m_oCurrencyCodes;

        public List<CurrencyExchange> GetCurrencyExchangeCache()
        {
            return null;
        }

        public CurrencyCodeCache(CurrencyInfoCache[] ListOfCurrencies)
        {
            //TODO: populate from BFS to get the right Guid as the last string in the Tuple
            m_oCurrencyCodes = new List<Tuple<string, string, string>>();


            var CurrencyNames = (

                from c in CultureInfo.GetCultures(CultureTypes.SpecificCultures).ToList()
                select new
                {
                    name = new RegionInfo(c.LCID).CurrencyEnglishName,
                    code = new RegionInfo(c.LCID).ISOCurrencySymbol
                }
                ).Distinct().ToArray();

            var CurrencyJoin =
                (
                from cn in CurrencyNames
                join cb in ListOfCurrencies on cn.code equals cb.Currency
                select new
                {
                    name = cn.name,
                    code = cb.Currency
                }
                ).ToList();

            m_oCurrencyCodes.AddRange(
                (
                                from cn in CurrencyNames
                                join cb in ListOfCurrencies on cn.code equals cb.Currency
                                select Tuple.Create(
                                     cb.Currency,
                                     cn.name,
                                     ""
                                    )

                ).ToArray()
                );

        }


        public List<Tuple<string, string, string>> GetCache()
        {
            return m_oCurrencyCodes;
        }
    }

    [XmlRoot("country_list")]
    [Serializable()]
    public class CountryList
    {
        public CountryList() { Items = new List<CachedCountry>(); }
        [XmlElement("Country")]
        public List<CachedCountry> Items { get; set; }
    }

    public class CachedCountry
    {
        [XmlElement("CountryCode")]
        public String CountryCode { get; set; }
        [XmlElement("CountryName")]
        public String CountryName { get; set; }
        [XmlElement("BFSId")]
        public String BFSId { get; set; }

    }

    public class CountryCache : ICache
    {
        private List<Tuple<string, string, string>> m_oCountries;

        public CountryCache()
        {
            m_oCountries = new List<Tuple<string, string, string>>();

            string oAppPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string oToday = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            string oTodaysFile = string.Format(@"{0}\{1}", oAppPath, string.Format("countrycache_{0}.xml", oToday));

            XmlSerializer oCountrySerilizer = new XmlSerializer(typeof(CountryList));

            if (File.Exists(oTodaysFile))
            {
                try
                {
                    //read cachefile
                    FileStream oCountryStream = new FileStream(oTodaysFile, FileMode.Open);
                    CountryList oCountryList = (CountryList)oCountrySerilizer.Deserialize(oCountryStream);

                    //read into Cache
                    var oCountries = from country in oCountryList.Items
                                     select new
                                     {
                                         countrycode = country.CountryCode,
                                         countryname = country.CountryName,
                                         BFSguid = country.BFSId
                                     };
                    foreach (var Country in oCountries)
                    {
                        m_oCountries.Add(Tuple.Create(Country.countrycode, Country.countryname, Country.BFSguid.ToString()));
                    }
                    //Logger.Instance.Info(string.Format("Read country cache file with {0} countries", m_oCountries.Count()));
                    //shared.FAMGlobal.log.Info(string.Format("Read country cache file with {0} countries", m_oCountries.Count()));
                    return;
                }
                catch
                {

                    //nothing, build the cache again
                }
            }



            //TODO, remove this and replace with data from BFS, but still save the data to file
            #region Geonames, replace with BFS!
            System.Net.WebClient client = new System.Net.WebClient();
            string oCountryListString = client.DownloadString("http://download.geonames.org/export/dump/countryInfo.txt");

            //remove all lines starting with #
            var countrylines = Regex.Split(oCountryListString, "\r\n|\r|\n").Where(x => !x.StartsWith("#"));

            var oCountriesFromExternalSource = from lines in countrylines
                .Where(l => !string.IsNullOrEmpty(l))
                .Select(l => l.Split('\t'))
                                               select new
                                               {
                                                   countrycode = lines[0].Trim(),
                                                   countryname = lines[4].Trim(),
                                                   BFSguid = "0a70de98-5d83-45ad-811c-da6c60656d69" //just for test!
                                               };
            #endregion

            foreach (var Country in oCountriesFromExternalSource)
            {
                m_oCountries.Add(Tuple.Create(Country.countrycode, Country.countryname, Country.BFSguid));
            }



            //save cache file
            CountryList oCountryListToSave = new CountryList();
            oCountryListToSave.Items.AddRange(
                from c in m_oCountries
                select new CachedCountry
                {
                    CountryCode = c.Item1,
                    CountryName = c.Item2,
                    BFSId = c.Item3
                }
                );


            using (FileStream fs = new FileStream(oTodaysFile, FileMode.Create))
            {
                oCountrySerilizer.Serialize(fs, oCountryListToSave);
            }


            //remove older files
            foreach (string oFileToDelete in Directory.EnumerateFiles(oAppPath, "countrycache_*.xml").Where(f => !f.Contains(oToday)))
            {
                FileInfo oFile = new FileInfo(oFileToDelete);
                oFile.Delete();
            }
            //Logger.Instance.Info(string.Format("Created country cache file with {0} countries", m_oCountries.Count()));
            //shared.FAMGlobal.log.Info(string.Format("Created country cache file with {0} countries", m_oCountries.Count()));

        }

        public List<Tuple<string, string, string>> GetCache()
        {
            return m_oCountries;
        }
    }
}
