using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Imperatur_v2.events;
using Imperatur_v2;
using Imperatur_v2.handler;



namespace Imperatur_Market_Client.control
{
    public partial class AccountTab : UserControl
    {
        private IAccountHandlerInterface m_AccountHandler;
        private UserControl AccountMainInfo;
        public AccountTab(IAccountHandlerInterface AccountHandler)
        {
            InitializeComponent();
            m_AccountHandler = AccountHandler;
            typeof(TableLayoutPanel).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(tlp_Account, true, null);
        }

        private void AccountTab_Load(object sender, EventArgs e)
        {
            Account_Search oControl_Account_Search = new control.Account_Search(m_AccountHandler);
            oControl_Account_Search.SelectedAccount += OControl_Account_Search_SelectedAccount;
            //AccountMainInfo = new UserControl();

            tlp_Account.Controls.Add(oControl_Account_Search, 0, 0);
        }
        private void OControl_Account_Search_SelectedAccount(object sender, SelectedAccountEventArg e)
        {
            try
            {
                AccountMainInfo = new CreateControlFromObject(m_AccountHandler.GetAccount(e.Identifier),
                    "Account main info",
                    new string[]
                    {
                    "Name",
                    "AccountType",
                    "Customer"
                    });
                AccountMainInfo.Name = "AccountMainInfo";

                if (!tlp_Account.Controls.ContainsKey(AccountMainInfo.Name))
                {
                    tlp_Account.Controls.Add(AccountMainInfo, 1, 0);
                }
                else
                {
                    tlp_Account.Controls.RemoveByKey(AccountMainInfo.Name);
                    tlp_Account.Controls.Add(AccountMainInfo, 1, 0);
                }
            }
            catch(Exception ex)
            {
                int ggf = 0;
            }
            AccountMainInfo.Refresh();
        }
    }
}
