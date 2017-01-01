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
using Imperatur_v2.trade;

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

            listView_holdings.View = View.Details;
            listView_holdings.Columns.Add("Name");
            listView_holdings.Columns.Add("Quantity");
            listView_holdings.Columns.Add("PurchaseAmount");
            listView_holdings.Columns.Add("Change");
            listView_holdings.Columns.Add("ChangePercent");
            listView_holdings.Columns.Add("CurrentValue");
            
        }

        public void UpdateAccountInfo(IAccountInterface AccountData)
        {

            if (listView_holdings.Items.Count > 0)
                listView_holdings.Items.Clear();



            foreach (Holding oH in AccountData.GetHoldings())
            {
                ListViewItem oHoldingRow = new ListViewItem(
                    new string[]
                    {
                        oH.Name,
                        oH.Quantity.ToString(),
                        oH.PurchaseAmount.ToString(),
                        oH.Change.ToString(true, true),
                        Math.Round(oH.ChangePercent, 2, MidpointRounding.AwayFromZero).ToString() + " %",
                        oH.CurrentAmount.ToString()
                    }
                    );

                oHoldingRow.Tag = oH.Name.ToString();
                listView_holdings.Items.Add(oHoldingRow);
            }
            listView_holdings.Refresh();
                        /*
            try
            {
                AccountMainInfo = new CreateInfoControlFromObject(AccountData,
                    "Account main info",
                    new string[]
                    {
                    "Name",
                    "AccountType",
                    "Customer"
                    });
                AccountMainInfo.Name = "AccountMainInfo";
                if (!tableLayoutPanel_maininfo.Controls.ContainsKey(AccountMainInfo.Name))
                {
                    tableLayoutPanel_maininfo.Controls.Add(AccountMainInfo, 0, 0);
                }
                else
                {
                    tableLayoutPanel_maininfo.Controls.RemoveByKey(AccountMainInfo.Name);
                    tableLayoutPanel_maininfo.Controls.Add(AccountMainInfo, 0, 0);
                }

                DataGridView AvilableFundsGrid = new DataGridView();

                AvilableFundsGrid.AutoGenerateColumns = false;
                AvilableFundsGrid.AllowUserToAddRows = false;

                AvilableFundsGrid.Columns.Add(
                    new DataGridViewTextBoxColumn()
                    {
                        CellTemplate = new DataGridViewTextBoxCell(),
                        Name = "Amount",
                        HeaderText = "Amount",
                        DataPropertyName = "Amount",
                        ReadOnly = true
                    }
                );
                DataTable AvilableFundsDT = new DataTable();
                AvilableFundsDT.Columns.Add("Amount");

                DataRow row = null;
                foreach (IMoney oM in AccountData.GetAvailableFunds())
                {
                    row = AvilableFundsDT.NewRow();
                    row["Amount"] = oM.ToString(true, true);
                    AvilableFundsDT.Rows.Add(row);

                }
                AvilableFundsGrid.DataSource = AvilableFundsDT;
                AvilableFundsGrid.Dock = DockStyle.Top;
                AccountMainAvailableFunds = new CreateDataGridControlFromObject(
                    new DataGridForControl
                    {
                        DataGridViewToBuild = AvilableFundsGrid,
                        GroupBoxCaption = "Available Funds"
                    }
                    );

                AccountMainAvailableFunds.Name = "AccountMainAvailableFunds";
                if (!tableLayoutPanel_maininfo.Controls.ContainsKey(AccountMainAvailableFunds.Name))
                {
                    tableLayoutPanel_maininfo.Controls.Add(AccountMainAvailableFunds, 0, 1);
                }
                else
                {
                    tableLayoutPanel_maininfo.Controls.RemoveByKey(AccountMainAvailableFunds.Name);
                    tableLayoutPanel_maininfo.Controls.Add(AccountMainAvailableFunds, 0, 1);
                }

            }
            catch (Exception ex)
            {
                int ggf = 0;
            }
            if (AccountMainInfo != null)
                AccountMainInfo.Refresh();
            if (AccountMainAvailableFunds != null)
                AccountMainAvailableFunds.Refresh();
                */

        }
    }
}
