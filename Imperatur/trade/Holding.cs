using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur.monetary;

namespace Imperatur.trade
{
    public class Holding
    {
        public string Name;
        public Decimal Quantity;
        public Money Change;
        public Decimal ChangePercent;
        public Decimal ChangeToday;
        public Money PurchaseAmount;
        public Money CurrentAmount;
    }
}
