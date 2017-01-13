using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.securites;

namespace Imperatur_v2.order
{
    public interface ITrigger
    {
        bool Evaluate(Instrument Instrument);
    }
}
