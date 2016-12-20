using Imperatur_v2.monetary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_v2.trade
{
    public class Holding
    {
        public string Name;
        public string Symbol;
        public string ISIN;
        public Decimal Quantity;
        public Money Change;
        public Decimal ChangePercent;
        public Decimal ChangeToday;
        public Money PurchaseAmount;
        public Money CurrentAmount;
    }
}
