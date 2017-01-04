using System;
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

namespace Imperatur_Market_Client.control
{
    public partial class Account_MainInfo : UserControl
    {
        private UserControl AccountMainInfo;
        private UserControl AccountMainAvailableFunds;
        private IAccountInterface m_oA;

        public Account_MainInfo()
        {
            InitializeComponent();
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
                int ggf = 0;
            }
            if (AccountMainInfo != null)
                AccountMainInfo.Refresh();
            if (AccountMainAvailableFunds != null)
                AccountMainAvailableFunds.Refresh();
            
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

            DataTable TransactionsDT = new DataTable();
            TransactionsDT.Columns.Add("Amount");
            TransactionsDT.Columns.Add("TransactionDate");
            TransactionsDT.Columns.Add("TransactionType");
            TransactionsDT.Columns.Add("Symbol");

            DataRow row = null;
            foreach (ITransactionInterface oT in m_oA.Transactions)
            {
                row = TransactionsDT.NewRow();
                row["Amount"] = oT.DebitAccount.Equals(m_oA.Identifier) ? oT.DebitAmount.ToString() : oT.CreditAmount.ToString();
                row["TransactionDate"] = oT.TransactionDate;
                row["TransactionType"] = oT.TransactionType.ToString();
                row["Symbol"] = oT.SecuritiesTrade != null ? oT.SecuritiesTrade.Security.Symbol : "";

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
