﻿using System;
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
        private Account_Search oControl_Account_Search;
        private ColumnStyle InitalColumnStyle;
        private Button ExpandSearch;
        private System.Drawing.Size ExpandButtonSize;
        public AccountTab(IAccountHandlerInterface AccountHandler)
        {
            InitializeComponent();
            m_AccountHandler = AccountHandler;
            typeof(TableLayoutPanel).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(tlp_Account, true, null);
        }

        private void AccountTab_Load(object sender, EventArgs e)
        {
            ExpandButtonSize = new System.Drawing.Size(15, 23);
            oControl_Account_Search = new Account_Search(m_AccountHandler);
            oControl_Account_Search.SelectedAccount += OControl_Account_Search_SelectedAccount;
            oControl_Account_Search.ToggleSearchDialog += OControl_Account_Search_ToggleSearchDialog;
            tlp_Account.Controls.Add(oControl_Account_Search, 0, 0);

            Account_Holdings oControl_Account_Holdings = new Account_Holdings(m_AccountHandler);
            Account_Trade oControl_Account_Trade = new Account_Trade(m_AccountHandler);

            tlp_Account.Controls.Add(oControl_Account_Holdings, 2, 0);
            tlp_Account.Controls.Add(oControl_Account_Trade, 3, 0);
            ExpandSearch = new Button();
            ExpandSearch.Image = global::Imperatur_Market_Client.Properties.Resources.Expand;
            ExpandSearch.ImageAlign = ContentAlignment.MiddleCenter;
            ExpandSearch.Location = new System.Drawing.Point(10, 20);
            ExpandSearch.Size = ExpandButtonSize;
            ExpandSearch.TabIndex = 5;
            ExpandSearch.UseVisualStyleBackColor = true;
            ExpandSearch.Click += ExpandSearch_Click;
            this.Controls.Add(ExpandSearch);
            ExpandSearch.BringToFront();
            ExpandSearch.Visible = false;
            ExpandSearch.MouseEnter += ExpandSearch_MouseEnter;
            ExpandSearch.MouseLeave += ExpandSearch_MouseLeave;

        }

        private void ExpandSearch_MouseLeave(object sender, EventArgs e)
        {
            ExpandSearch.Text = "";
            ExpandSearch.AutoSize = false;
            ExpandSearch.Size = ExpandButtonSize;
            ExpandSearch.ImageAlign = ContentAlignment.MiddleCenter;
        }

        private void ExpandSearch_MouseEnter(object sender, EventArgs e)
        {
            ExpandSearch.Text = "Search";
            ExpandSearch.AutoSize = true;
            ExpandSearch.ImageAlign = ContentAlignment.MiddleRight;
        }

        private void ExpandSearch_Click(object sender, EventArgs e)
        {
            OControl_Account_Search_ToggleSearchDialog(this, new events.ToggleSearchEvents
            {
                Collapse = false
            });
        }

        private void OControl_Account_Search_ToggleSearchDialog(object sender, events.ToggleSearchEvents e)
        {
            if (InitalColumnStyle == null)
            {
                InitalColumnStyle = new ColumnStyle();
                InitalColumnStyle.SizeType = tlp_Account.ColumnStyles[tlp_Account.GetColumn(oControl_Account_Search)].SizeType;
                InitalColumnStyle.Width = tlp_Account.ColumnStyles[tlp_Account.GetColumn(oControl_Account_Search)].Width;
            }
            if (e.Collapse)
            {
                tlp_Account.ColumnStyles[tlp_Account.GetColumn(oControl_Account_Search)].SizeType = SizeType.Absolute;
                tlp_Account.ColumnStyles[tlp_Account.GetColumn(oControl_Account_Search)].Width = 0;
                ExpandSearch.Visible = true;
            }
            else
            {
                ExpandSearch.Visible = false;
                tlp_Account.ColumnStyles[tlp_Account.GetColumn(oControl_Account_Search)].SizeType = InitalColumnStyle.SizeType;
                tlp_Account.ColumnStyles[tlp_Account.GetColumn(oControl_Account_Search)].Width = InitalColumnStyle.Width;
            }
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
            if (AccountMainInfo != null)
                AccountMainInfo.Refresh();
        }
    }
}
