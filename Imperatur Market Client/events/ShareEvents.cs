using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_Market_Client.events
{
    public class ToggleSearchEvents : EventArgs
    {
        public bool Collapse;
    }

    public class SelectedAccountEventArg : EventArgs
    {
        public Guid Identifier;
    }

    public class SelectedSymbolEventArg : EventArgs
    {
        public string Symbol;
    }
}
