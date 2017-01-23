using Imperatur_v2.securites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_v2.cache
{
    public interface IHistoricalPriceCacheBuilder
    {
        void BuildHistoricalPriceCache();
        string GetFullPathOfHistoricalDataForInstrument(Instrument Instrument);
    }
}
