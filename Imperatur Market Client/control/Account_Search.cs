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
using Imperatur_Market_Client.events;

namespace Imperatur_Market_Client.control
{
    public partial class Account_Search : UserControl
    {
        private IAccountHandlerInterface m_oAh;
        public event MainForm.SelectedAccountEventHandler SelectedAccount;
        public event MainForm.ToggleSearchDialogHandler ToggleSearchDialog;

        public Account_Search(IAccountHandlerInterface AccountHandler)
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            this.listView_searchresult.SelectedIndexChanged += ListView_searchresult_SelectedIndexChanged;
            m_oAh = AccountHandler;
            listView_searchresult.View = View.Details;
            listView_searchresult.Columns.Add("Name");
            listView_searchresult.Columns.Add("Amount");
            listView_searchresult.Columns.Add("Current");
            this.textBox_Search.KeyDown += TextBox_Search_KeyDown;

        }

        private void TextBox_Search_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button_search_Click(this, null);
            }
        }

        protected virtual void OnToggleSearchDialog(ToggleSearchEvents e)
        {
            if (ToggleSearchDialog != null)
                ToggleSearchDialog(this, e);
        }


        protected virtual void OnSelectedAccount(SelectedAccountEventArg e)
        {
            if (SelectedAccount != null)
                SelectedAccount(this, e);
        }


        private void ListView_searchresult_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listView_searchresult.SelectedItems.Count == 1)
            {
                OnSelectedAccount(new SelectedAccountEventArg()
                {
                    Identifier = new Guid(this.listView_searchresult.SelectedItems[0].Tag.ToString())
                });
            }
                
        }

        private void button_search_Click(object sender, EventArgs e)
        {
            if (this.textBox_Search.Text.Trim() == "")
            {
                return;
            }
           
            if (listView_searchresult.Items.Count > 0)
                listView_searchresult.Items.Clear();

            //populate the account list

            foreach (IAccountInterface oA in m_oAh.SearchAccount(this.textBox_Search.Text.Trim()))
            {
                ListViewItem oSearchResultRow = new ListViewItem(
                    new string[]
                    {
                        String.Format("{0} {1}", oA.GetCustomer().FirstName, oA.GetCustomer().LastName),
                        "0",
                        "0",
                    }
                    );
                oSearchResultRow.Tag = oA.Identifier.ToString();
                listView_searchresult.Items.Add(oSearchResultRow);
            }
            listView_searchresult.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OnToggleSearchDialog(new ToggleSearchEvents()
            {
                Collapse = true
            });
        }
    }
}
