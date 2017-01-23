using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2;

namespace ImperaturService.core
{
    public class ImperaturServiceCore
    {
        private ImperaturMarket m_oImperaturCore;

        public ImperaturMarket ImperaturMarket
        {
            get
            {
                return m_oImperaturCore;
            }
        }

        public ImperaturServiceCore(string SystemLocation)
        {
            m_oImperaturCore = (ImperaturMarket)ImperaturContainer.BuildImperaturContainer(SystemLocation);
        }
    }
}
