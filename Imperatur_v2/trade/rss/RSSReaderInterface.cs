using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_v2.trade.rss
{
    public interface IRSSReader
    {
        int GetOccurancesOfString(string[] URLs, string[] SearchData);
    }
}
