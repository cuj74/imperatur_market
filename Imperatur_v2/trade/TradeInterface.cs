﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.monetary;

namespace Imperatur_v2.trade
{
    interface ITradeInterface
    {
        Money GetGAA();
        Decimal GetQuantity();
        Money GetTradeAmount();

    }
}
