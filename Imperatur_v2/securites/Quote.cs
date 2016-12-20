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
        public DateTime Loggedat;
        public string Symbol;
        public Money Low;
        public Money High;
        public Money Last;
        public Decimal Change;
        public Decimal ChangePercent;
        public Money Dividend;
        public Money DividendYield;
    }
}
