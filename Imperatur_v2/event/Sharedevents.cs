using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_v2.events
{
    //public class SelectedAccountEventArg : EventArgs
    //{
    //    public Guid Identifier;
    //}
    
    public class SaveAccountEventArg : EventArgs
    {
        public Guid Identifier;
    }

}
