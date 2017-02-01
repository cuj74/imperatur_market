using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_Market_Core.system
{
    public interface ISystemHandler
    {
        bool CreateSystem(string SystemLocation);
        bool VerifySystem(string SystemLocation);
        bool ReadSystem(string SystemLocation);
    }
}
