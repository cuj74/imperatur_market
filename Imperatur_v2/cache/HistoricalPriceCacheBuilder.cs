using Imperatur_v2.json;
using Imperatur_v2.securites;
using Imperatur_v2.shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_v2.cache
{
    public class HistoricalPriceCacheBuilder : IHistoricalPriceCacheBuilder
    {
        private Instrument[] m_oInstruments;
        string m_oPathToSerializeDirectory;
        string m_oFileNamePattern;
        Exchange m_oCurrentExchange;
        GoogleHistoricalDataInterpreter m_oGHDI;

        public HistoricalPriceCacheBuilder(Instrument[] Instruments, string PathToSerializeDirectory, string FileNamePattern, string Exchange)
        {
            m_oInstruments = Instruments;
            m_oPathToSerializeDirectory = PathToSerializeDirectory;
            m_oFileNamePattern = FileNamePattern;
            m_oCurrentExchange = new Exchange
            {
                ExhangeCode = Exchange
            };
            m_oGHDI = new GoogleHistoricalDataInterpreter();

        }

        private HistoricalQuote GetHistoricalQuoteOnline(Instrument instrument, Exchange exchange)
        {
            return m_oGHDI.GetHistoricalData(instrument, exchange);
        }
        private HistoricalQuote GetHistoricalQuoteOnline(Instrument instrument, Exchange exchange, DateTime FromDate)
        {
            return m_oGHDI.GetHistoricalData(instrument, exchange, FromDate);
        }

        public string GetFullPathOfHistoricalDataForInstrument(Instrument Instrument)
        {
            string FileName = m_oFileNamePattern.Replace("{exchange}", m_oCurrentExchange.ExhangeCode).Replace("{symbol}", Instrument.Symbol);
            return string.Format(@"{0}\{1}", m_oPathToSerializeDirectory, FileName);
        }

        private void BuildHistoricalCacheForInstrument(Instrument InstrumentToCache)
        {
            HistoricalQuote oH = new HistoricalQuote(null, null, null);
            DateTime oDataFromNeeded = DateTime.Now;
            bool bReadMore = false;
            bool bAllHistoricalDataNeeded = true;

            string FullPath = GetFullPathOfHistoricalDataForInstrument(InstrumentToCache);
            if (File.Exists(FullPath))
            {
                oH = (HistoricalQuote)DeserializeJSON.DeserializeObjectFromFile(FullPath);
                if (oH == null)
                {
                    return;
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
                    oH = GetHistoricalQuoteOnline(InstrumentToCache, m_oCurrentExchange);
                    SerializeJSONdata.SerializeObject(oH, FullPath);
                }
                catch (Exception ex)
                {
                    ImperaturGlobal.GetLog().Error(string.Format("Couldn't retreive and save data for {0}", InstrumentToCache.Symbol), ex);
                }
            }
            else if (bReadMore)
            {
                try
                {
                    HistoricalQuote oHnew = GetHistoricalQuoteOnline(InstrumentToCache, m_oCurrentExchange, oDataFromNeeded);

                    oH.HistoricalQuoteDetails.AddRange(oHnew.HistoricalQuoteDetails.Where(h => h.Date.Date > oDataFromNeeded.Date).ToList());
                    SerializeJSONdata.SerializeObject(oH, FullPath);
                }
                catch (Exception ex)
                {
                    ImperaturGlobal.GetLog().Error(string.Format("Couldn't retreive and save data for {0}", InstrumentToCache.Symbol), ex);
                }
            }
        }

        public void BuildHistoricalPriceCache()
        {
            if (m_oInstruments == null || m_oPathToSerializeDirectory == "" || m_oFileNamePattern == "" || m_oCurrentExchange == null || m_oCurrentExchange.ExhangeCode == "")
            {
                throw new Exception("No instruments to build Historical price cache on or no information about exchange or directory!");
            }

            Parallel.For(0, m_oInstruments.Length-1, new ParallelOptions { MaxDegreeOfParallelism = 10 },
              i =>
              {
                  BuildHistoricalCacheForInstrument(m_oInstruments[i]);
              });
        }
    }
}
