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
using Imperatur_v2.monetary;
using Imperatur_Market_Client.events;

namespace Imperatur_Market_Client.control
{
    public partial class Account_Holdings : UserControl
    {
        public event MainForm.SelectedAccountEventHandler SelectedAccount;
        private IAccountHandlerInterface m_oAh;
        private ITradeHandlerInterface m_oTradeHandler;
        private UserControl TotalAvailableFunds;
        private IAccountInterface m_oA;

        public Account_Holdings(IAccountHandlerInterface AccountHandler, ITradeHandlerInterface TradeHandler)
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            m_oAh = AccountHandler;
            m_oTradeHandler = TradeHandler;

            listView_holdings.View = View.Details;
            listView_holdings.Columns.Add("Name");
            listView_holdings.Columns.Add("Quantity");
            listView_holdings.Columns.Add("PurchaseAmount");
            listView_holdings.Columns.Add("Change");
            listView_holdings.Columns.Add("ChangePercent");
            listView_holdings.Columns.Add("CurrentValue");
            listView_holdings.Columns.Add("Action");
            listView_holdings.Columns.Add("Info");

            ListViewExtender extender = new ListViewExtender(listView_holdings);
            ListViewButtonColumn buttonAction = new ListViewButtonColumn(6);
            buttonAction.Click += ButtonAction_Click;
            buttonAction.FixedWidth = true;
            extender.AddColumn(buttonAction);

            //ListViewExtender extenderInfo = new ListViewExtender(listView_holdings);
            ListViewButtonColumn buttonActionInfo = new ListViewButtonColumn(7);
            buttonActionInfo.Click += ButtonActionInfo_Click;
            buttonActionInfo.FixedWidth = true;
            extender.AddColumn(buttonActionInfo);


        }

        private void ButtonActionInfo_Click(object sender, ListViewColumnMouseEventArgs e)
        {

            if (m_oA == null)
            {
                return;
            }
            string Symbol = e.Item.Tag.ToString();

            DataGridView TransactionGrid = new DataGridView();

            TransactionGrid.AutoGenerateColumns = false;
            TransactionGrid.AllowUserToAddRows = false;

            TransactionGrid.Columns.Add(
                new DataGridViewTextBoxColumn()
                {
                    CellTemplate = new DataGridViewTextBoxCell(),
                    Name = "Amount",
                    HeaderText = "Amount",
                    DataPropertyName = "Amount",
                    ReadOnly = true
                }
            );
            TransactionGrid.Columns.Add(
                new DataGridViewTextBoxColumn()
                {
                    CellTemplate = new DataGridViewTextBoxCell(),
                    Name = "TransactionDate",
                    HeaderText = "Date",
                    DataPropertyName = "TransactionDate",
                    ReadOnly = true
                }
            );
            TransactionGrid.Columns.Add(
                new DataGridViewTextBoxColumn()
                {
                    CellTemplate = new DataGridViewTextBoxCell(),
                    Name = "TransactionType",
                    HeaderText = "Type",
                    DataPropertyName = "TransactionType",
                    ReadOnly = true
                }
            );
            TransactionGrid.Columns.Add(
                new DataGridViewTextBoxColumn()
                {
                    CellTemplate = new DataGridViewTextBoxCell(),
                    Name = "Symbol",
                    HeaderText = "Symbol",
                    DataPropertyName = "Symbol",
                    ReadOnly = true
                }
            );
            TransactionGrid.Columns.Add(
                new DataGridViewTextBoxColumn()
                {
                    CellTemplate = new DataGridViewTextBoxCell(),
                    Name = "Revenue",
                    HeaderText = "Revenue",
                    DataPropertyName = "Revenue",
                    ReadOnly = true
                }
            );

            DataTable TransactionsDT = new DataTable();
            TransactionsDT.Columns.Add("Amount");
            TransactionsDT.Columns.Add("TransactionDate");
            TransactionsDT.Columns.Add("TransactionType");
            TransactionsDT.Columns.Add("Symbol");
            TransactionsDT.Columns.Add("Revenue");

            DataRow row = null;
            foreach (ITransactionInterface oT in m_oA.Transactions.Where(t=> t.SecuritiesTrade != null && t.SecuritiesTrade.Security.Symbol.Equals(Symbol)))
            {
                row = TransactionsDT.NewRow();
                row["Amount"] = oT.DebitAccount.Equals(m_oA.Identifier) ? oT.DebitAmount.ToString() : oT.CreditAmount.ToString();
                row["TransactionDate"] = oT.TransactionDate;
                row["TransactionType"] = oT.TransactionType.ToString();
                row["Symbol"] = oT.SecuritiesTrade != null ? oT.SecuritiesTrade.Security.Symbol : "";
                row["Revenue"] = oT.SecuritiesTrade != null && oT.SecuritiesTrade.Revenue != null ? oT.SecuritiesTrade.Revenue.ToString() : "";
                TransactionsDT.Rows.Add(row);

            }
            TransactionGrid.DataSource = TransactionsDT;
            TransactionGrid.Dock = DockStyle.Fill;

            UserControl TransactionControl = new CreateDataGridControlFromObject(
            new DataGridForControl
            {
                DataGridViewToBuild = TransactionGrid,
                GroupBoxCaption = string.Format("Transactions on {0} for {1}", m_oA.GetCustomer().FullName, Symbol)
            }
            );

            Form Ftrans = new Form();
            Ftrans.Controls.Add(TransactionControl);
            Ftrans.ShowDialog();


        }

        protected virtual void OnSelectedAccount(SelectedAccountEventArg e)
        {
            if (SelectedAccount != null)
                SelectedAccount(this, e);
        }

        private void RefreshData()
        {
            OnSelectedAccount(new SelectedAccountEventArg()
            {
                Identifier = m_oA.Identifier
            });
        }

        public void UpdateAccountInfo(IAccountInterface AccountData)
        {
            m_oA = AccountData;
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
                        oH.CurrentAmount.ToString(),
                        "Sell",
                        "Info"
                    }
                    );
                oHoldingRow.SubItems.Add(oH.Name);

                oHoldingRow.Tag = oH.Name.ToString();
                listView_holdings.Items.Add(oHoldingRow);
            }


            listView_holdings.Refresh();


            DataGridView TotalAvilableFundsGrid = new DataGridView();

            TotalAvilableFundsGrid.AutoGenerateColumns = false;
            TotalAvilableFundsGrid.AllowUserToAddRows = false;

            TotalAvilableFundsGrid.Columns.Add(
                new DataGridViewTextBoxColumn()
                {
                    CellTemplate = new DataGridViewTextBoxCell(),
                    Name = "Deposit",
                    HeaderText = "Deposit",
                    DataPropertyName = "Deposit",
                    ReadOnly = true
                }
            );

            TotalAvilableFundsGrid.Columns.Add(
                new DataGridViewTextBoxColumn()
                {
                    CellTemplate = new DataGridViewTextBoxCell(),
                    Name = "Amount",
                    HeaderText = "Amount",
                    DataPropertyName = "Amount",
                    ReadOnly = true
                }
            );

            TotalAvilableFundsGrid.Columns.Add(
                new DataGridViewTextBoxColumn()
                {
                    CellTemplate = new DataGridViewTextBoxCell(),
                    Name = "Change",
                    HeaderText = "Change",
                    DataPropertyName = "Change",
                    ReadOnly = true
                }
            );

            TotalAvilableFundsGrid.Columns.Add(
                new DataGridViewTextBoxColumn()
                {
                    CellTemplate = new DataGridViewTextBoxCell(),
                    Name = "ChangePercent",
                    HeaderText = "ChangePercent",
                    DataPropertyName = "ChangePercent",
                    ReadOnly = true
                }
            );

            DataTable TotalAvilableFundsDT = new DataTable();
            TotalAvilableFundsDT.Columns.Add("Deposit");
            TotalAvilableFundsDT.Columns.Add("Amount");
            TotalAvilableFundsDT.Columns.Add("Change");
            TotalAvilableFundsDT.Columns.Add("ChangePercent");

            List<IMoney> TotalFundsList = AccountData.GetTotalFunds() ?? new List<IMoney>();
            List<IMoney> TotalDeposit = AccountData.GetDepositedAmount() ?? new List<IMoney>();



            DataRow row = null;
            foreach (IMoney oM in TotalFundsList)
            {
                row = TotalAvilableFundsDT.NewRow();
                row["Deposit"] = TotalDeposit.Where(od => od.CurrencyCode.Equals(oM.CurrencyCode)).First().ToString();
                row["Amount"] = oM.ToString(true, true);
                row["Change"] = oM.Subtract(TotalDeposit.Where(od => od.CurrencyCode.Equals(oM.CurrencyCode)).First()).ToString();
                row["ChangePercent"] = string.Format("{0}%", TotalDeposit.Where(od => od.CurrencyCode.Equals(oM.CurrencyCode)).First().Amount > 0 ? oM.Subtract(TotalDeposit.Where(od => od.CurrencyCode.Equals(oM.CurrencyCode)).First().Amount).Divide(TotalDeposit.Where(od => od.CurrencyCode.Equals(oM.CurrencyCode)).First().Amount).Multiply(100).ToString(true, false) : "0");
                TotalAvilableFundsDT.Rows.Add(row);

            }

            TotalAvilableFundsGrid.DataSource = TotalAvilableFundsDT;
            TotalAvilableFundsGrid.Dock = DockStyle.Top;
            TotalAvailableFunds = new CreateDataGridControlFromObject(
                new DataGridForControl
                {
                    DataGridViewToBuild = TotalAvilableFundsGrid,
                    GroupBoxCaption = "Current Funds"
                }
                );

            TotalAvailableFunds.Name = "AccountMainAvailableFunds";
            if (!tableLayoutPanel1.Controls.ContainsKey(TotalAvailableFunds.Name))
            {
                tableLayoutPanel1.Controls.Add(TotalAvailableFunds, 0, 2);
            }
            else
            {
                tableLayoutPanel1.Controls.RemoveByKey(TotalAvailableFunds.Name);
                tableLayoutPanel1.Controls.Add(TotalAvailableFunds, 0, 2);
            }


        }

        private void ButtonAction_Click(object sender, ListViewColumnMouseEventArgs e)
        {
            ShowSellDialog(e.Item.Tag.ToString(), Convert.ToInt32(Convert.ToDecimal(e.Item.SubItems[1].Text)));
        }

        private void ShowSellDialog(string Symbol, int MaxQuantity)
        {
            using (var form = new SellDialog(m_oAh, m_oA, Symbol, MaxQuantity))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    int SellQuantity = form.ReturnQuantity;            //values preserved after close

                    m_oA.SellHoldingFromAccount(SellQuantity, Symbol, m_oTradeHandler);
                    RefreshData();
                }
                else
                {
                    form.Close();
                }

            }
        }
    }
}
