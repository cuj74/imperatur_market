using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.monetary;

namespace Imperatur_v2.securites
{
    public class Quote
    {
        public DateTime InternalLoggedat;
        public string Symbol;
        public string Exchange;
        public IMoney LastTradePrice;
        public DateTime LastTradeDateTime;
        public int LastTradeSize;
        public IMoney Change;
        public Decimal ChangePercent;
        public IMoney PreviousClosePrice;

    }
}
