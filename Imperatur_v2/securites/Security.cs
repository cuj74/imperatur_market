using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.monetary;

namespace Imperatur_v2.securites
{
    public class Security
    {
        public string Symbol;
        public string Exchange;
        public DateTime LastTradeDateTime;
        public Decimal Change;
        public Decimal ChangePercent;
        public Money Dividend;
        public Money DividendYield;

    }
    public class Instrument
    {
        public string Symbol;
        public string Name;
        public string ISIN;
        public string Sector;
        public string ICB;
        public string CurrencyCode;

    }
    public enum SecurityType
    {
        Stock
    }
}
