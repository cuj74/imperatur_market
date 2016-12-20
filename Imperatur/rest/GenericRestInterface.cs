using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Imperatur.rest
{
    interface GenericRestInterface
    {
        object GetJson(string URL);
    }
}
