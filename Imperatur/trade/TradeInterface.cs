using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur.monetary;

namespace Imperatur.trade
{
    public interface ITradeInterface
    {
        Money GetGAA();
        Decimal GetQuantity();
        Money GetTradeAmount();
    }
}
