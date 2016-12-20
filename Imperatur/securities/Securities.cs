using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur.monetary;

namespace Imperatur.securities
{
    public class Securities : ISecuritiesInterface
    {
        public string Symbol;
        public string Exchange;
        public DateTime LastTradeDateTime;
        public Decimal Change;
        public Decimal ChangePercent;
        public Money Dividend;
        public Money DividendYield;
    }
}
