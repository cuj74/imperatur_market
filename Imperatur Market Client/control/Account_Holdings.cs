using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Imperatur_v2.handler;
using Imperatur_v2;
using Imperatur_v2.account;
using Imperatur_v2.events;

namespace Imperatur_Market_Client.control
{
    public partial class Account_Holdings : UserControl
    {
        private IAccountHandlerInterface m_oAh;
        public Account_Holdings(IAccountHandlerInterface AccountHandler)
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            m_oAh = AccountHandler;
        }
    }
}
