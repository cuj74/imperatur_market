﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Imperatur_v2.account;
using Imperatur_v2.handler;
using Imperatur_v2.monetary;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Imperatur_v2.order;

namespace Imperatur_Market_Client.control
{
    public partial class Account_MainInfo : UserControl
    {
        private UserControl AccountMainInfo;
        private UserControl AccountMainAvailableFunds;
        private UserControl OrderUnprocessed;
        private IAccountInterface m_oA;
        private IOrderQueue m_oOrderQueueHandler;

        public Account_MainInfo(IOrderQueue OrderQueueHandler)
        {
            InitializeComponent();
            m_oOrderQueueHandler = OrderQueueHandler;
        }

        public void AddControlToTLP(UserControl NewControl, int row)
        {
            tableLayoutPanel_maininfo.Controls.Add(NewControl, 0, row);
        }

        public void UpdateAcountInfo(IAccountInterface AccountData)
        {
            m_oA = AccountData;
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
                int gg = 0;
            }
            if (AccountMainInfo != null)
                AccountMainInfo.Refresh();
            if (AccountMainAvailableFunds != null)
                AccountMainAvailableFunds.Refresh();


            ShowOrders(m_oA.Identifier);
        }

        private void ShowOrders(Guid AccountIdentifier)
        {

            DataGridView OrdersGrid = new DataGridView();

            OrdersGrid.AutoGenerateColumns = false;
            OrdersGrid.AllowUserToAddRows = false;

            OrdersGrid.Columns.Add(
                new DataGridViewTextBoxColumn()
                {
                    CellTemplate = new DataGridViewTextBoxCell(),
                    Name = "Instrument",
                    HeaderText = "Instrument",
                    DataPropertyName = "Instrument",
                    ReadOnly = true
                }
            );
            OrdersGrid.Columns.Add(
                new DataGridViewTextBoxColumn()
                {
                    CellTemplate = new DataGridViewTextBoxCell(),
                    Name = "Quantity",
                    HeaderText = "Quantity",
                    DataPropertyName = "Quantity",
                    ReadOnly = true
                }
            );
            OrdersGrid.Columns.Add(
                new DataGridViewTextBoxColumn()
                {
                    CellTemplate = new DataGridViewTextBoxCell(),
                    Name = "Type",
                    HeaderText = "Type",
                    DataPropertyName = "Type",
                    ReadOnly = true
                }
            );

            DataTable OrdersDT = new DataTable();
            OrdersDT.Columns.Add("Instrument");
            OrdersDT.Columns.Add("Quantity");
            OrdersDT.Columns.Add("Type");

            DataRow row = null;
            foreach (IOrder oOrder in m_oOrderQueueHandler.GetOrdersForAccount(AccountIdentifier))
            {
                row = OrdersDT.NewRow();
                row["Instrument"] = oOrder.Symbol;
                row["Quantity"] = oOrder.Quantity;
                row["Type"] = oOrder.OrderType.ToString();

                OrdersDT.Rows.Add(row);

            }
            OrdersGrid.DataSource = OrdersDT;
            OrdersGrid.Dock = DockStyle.Top;
            OrderUnprocessed = new CreateDataGridControlFromObject(
                new DataGridForControl
                {
                    DataGridViewToBuild = OrdersGrid,
                    GroupBoxCaption = "Order unprocessed"
                }
                );

            OrderUnprocessed.Name = "OrderUnprocessed";
            if (!tableLayoutPanel_maininfo.Controls.ContainsKey(OrderUnprocessed.Name))
            {
                tableLayoutPanel_maininfo.Controls.Add(OrderUnprocessed, 0, 2);
            }
            else
            {
                tableLayoutPanel_maininfo.Controls.RemoveByKey(OrderUnprocessed.Name);
                tableLayoutPanel_maininfo.Controls.Add(OrderUnprocessed, 0, 2);
            }
            if (AccountMainInfo != null)
                AccountMainInfo.Refresh();
            if (OrderUnprocessed != null)
                OrderUnprocessed.Refresh();
        }

        private void button_Show_Transactions_Click(object sender, EventArgs e)
        {
            if (m_oA == null)
            {
                return;
            }

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
            foreach (ITransactionInterface oT in m_oA.Transactions)
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
            TransactionGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;

            UserControl TransactionControl = new CreateDataGridControlFromObject(
            new DataGridForControl
            {
                DataGridViewToBuild = TransactionGrid,
                GroupBoxCaption = string.Format("Transactions on {0}", m_oA.GetCustomer().FullName)
            }
            );

            Form Ftrans = new Form();
            Ftrans.Controls.Add(TransactionControl);
            Ftrans.ShowDialog();


            //window.ShowDialog();

        }
    }
}
