using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Imperatur;
using Imperatur.account;
using Imperatur.monetary;
using Imperatur.trade;
using Imperatur.cache;

namespace Imperatur_test_form
{
    public partial class Form_imperaturclient : Form
    {
        ImperaturContainer Ic;// = new ImperaturContainer(textBox_systemdir.Text);
        private string _SystemDirectory;
        private Account _SelectedAccount;
        private List<string> _AllowedSecurities;
        public Form_imperaturclient(string SystemDirectory)
        {
            InitializeComponent();
            _SystemDirectory = SystemDirectory;
        }

        private void Form_imperaturclient_Load(object sender, EventArgs e)
        {
            InitImperatur();
        }

        private void RefreshAccountlist()
        {
            if (listView1.Items.Count > 0)
                listView1.Clear();

            //populate the account list
            listView1.View = View.Details;
            listView1.Columns.Add("Id");
            listView1.Columns.Add("Type");
            listView1.Columns.Add("Name");
            listView1.Columns.Add("Available funds");

            foreach (Account oA in Ic.AccountHandler.Accounts)
            {
                string Amount = "";
                if (oA.GetAvailableFunds(Ic.AccountHandler.HouseAndBankAccountsGuid).Count > 0)
                    Amount = oA.GetAvailableFunds(Ic.AccountHandler.HouseAndBankAccountsGuid).First().Amount.ToString();
                listView1.Items.Add(
                    new ListViewItem(
                    new string[]
                    {
                        oA.Identifier.ToString(),
                        oA.AccountType.ToString(),
                        oA.Name,
                        Amount
                    }
                    )
                    );
            }
            listView1.Refresh();

        }

        private void InitImperatur()
        {
            if (Ic == null)
                Ic = new ImperaturContainer(_SystemDirectory);

            
            listView1.SelectedIndexChanged += ListView1_SelectedIndexChanged;
            RefreshAccountlist();
            
            var Tickers = Ic.GetQuotes().GroupBy(h => h.Symbol)
                   .Select(grp =>  grp.First())
                   .ToList();
            comboBox_tickers.DataSource = Tickers.Select(t=>t.Symbol).ToList();
            comboBox_tickers.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_tickers.SelectedIndexChanged += ComboBox_tickers_SelectedIndexChanged;


        }

        private void ComboBox_tickers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_tickers.SelectedValue.ToString() == "")
            {
                return;

            }
            if (_AllowedSecurities.Where(x => x.Equals(comboBox_tickers.SelectedValue.ToString())).Count() > 0)
            {
                button_buy_security.Enabled = true;
                Quote oQ = Ic.GetQuotes().Where(q => q.Symbol.Equals(comboBox_tickers.SelectedValue.ToString())).First();
                if (oQ != null)
                {
                    label_ticker_info.Text = string.Format("{0} | {1} | {2} | {3}",
                        oQ.CompanyName,
                        oQ.Dividend.ToString(), oQ.Change.ToString(), oQ.ChangePercent.ToString());
                }
            }
            else
            {
                button_buy_security.Enabled = false;
                label_ticker_info.Text = "Purchase is not possible (either no enough funds or currency)";
            }
        }

        private void ListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count > 0)
            {
                dataGridView_availablefunds.AutoGenerateColumns = false;
                //create the column programatically
                if (dataGridView_availablefunds.Columns.Count == 0)
                {
                    dataGridView_availablefunds.Columns.Add(CreateTextBox("Amount", "Amount", ""));
                }

                dataGridView_holdings.AutoGenerateColumns = false;
                //create the column programatically
                if (dataGridView_holdings.Columns.Count == 0)
                {
                    dataGridView_holdings.Columns.Add(CreateTextBox("Name", "Name", ""));
                    dataGridView_holdings.Columns.Add(CreateTextBox("Quantity", "Quantity", ""));
                    dataGridView_holdings.Columns.Add(CreateTextBox("PurchaseAmount", "Purchase Amount", ""));
                    dataGridView_holdings.Columns.Add(CreateTextBox("Change", "Change", ""));
                    dataGridView_holdings.Columns.Add(CreateTextBox("Changepercent", "Change percent", ""));
                    dataGridView_holdings.Columns.Add(CreateTextBox("CurrentWorth", "Current value", ""));

                    DataGridViewButtonColumn SellButtonColumn = new DataGridViewButtonColumn();
                    SellButtonColumn.HeaderText = "Sell";
                    SellButtonColumn.Name = "sell_holding";
                    SellButtonColumn.Text = "Sell";
                    SellButtonColumn.UseColumnTextForButtonValue = true;
                    dataGridView_holdings.Columns.Add(SellButtonColumn);

                    dataGridView_holdings.CellClick += DataGridView_holdings_CellClick;
                }
                dataGridView_total_worth.AutoGenerateColumns = false;
                if (dataGridView_total_worth.Columns.Count == 0)
                {
                    dataGridView_total_worth.Columns.Add(CreateTextBox("Deposit", "Deposit", ""));
                    dataGridView_total_worth.Columns.Add(CreateTextBox("Amount", "Amount", ""));
                    dataGridView_total_worth.Columns.Add(CreateTextBox("Change", "Change", ""));
                    dataGridView_total_worth.Columns.Add(CreateTextBox("ChangePercent", "ChangePercent", ""));
                }
                

                PopulateCurrentAccount(this.listView1.SelectedItems[0].SubItems[0].Text);

            }
        }

        private void DataGridView_holdings_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex !=
                dataGridView_holdings.Columns["sell_holding"].Index)
            {
                PictureBox pb1 = new PictureBox();
                pb1.ImageLocation = "https://www.google.com/finance/getchart?q=" + dataGridView_holdings.Rows[e.RowIndex].Cells["Name"].Value.ToString(); 
                pb1.SizeMode = PictureBoxSizeMode.AutoSize;
                this.Controls.Add(pb1);
                return;
            }
            //retrieve the ticker
            string TickerToSell = dataGridView_holdings.Rows[e.RowIndex].Cells["Name"].Value.ToString();
            decimal MaxQuantity = Decimal.Parse(dataGridView_holdings.Rows[e.RowIndex].Cells["Quantity"].Value.ToString());


            using (var form = new SellDialog(Ic.AccountHandler, _SelectedAccount, TickerToSell, Decimal.ToInt32(MaxQuantity)))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    int SellQuantity = form.ReturnQuantity;            //values preserved after close
                    Ic.AccountHandler.SellHoldingFromAccount(_SelectedAccount.Identifier, SellQuantity, TickerToSell);
                }
            }
            RefreshData();
            //throw new NotImplementedException();
        }

        private void PopulateCurrentAccount(string Identifier)
        {
            Account oA = Ic.AccountHandler.GetAccount(new Guid(Identifier));
            _SelectedAccount = oA;

            DataTable dt_Property = new DataTable();
            dt_Property.Columns.Add("Amount");
            DataRow row = null;
            foreach (Money m in oA.GetAvailableFunds(Ic.AccountHandler.HouseAndBankAccountsGuid))
            {
                row = dt_Property.NewRow();
                row["Amount"] = m.ToString();
                dt_Property.Rows.Add(row);

            }
            dataGridView_availablefunds.DataSource = dt_Property;


            DataTable dt_holdings = new DataTable();
            dt_holdings.Columns.Add("Name");
            dt_holdings.Columns.Add("Quantity");
            dt_holdings.Columns.Add("PurchaseAmount");
            dt_holdings.Columns.Add("Change");
            dt_holdings.Columns.Add("Changepercent");
            dt_holdings.Columns.Add("CurrentWorth");

            foreach (Holding oh in Ic.AccountHandler.GetAccountHoldings(oA.Identifier))
            {
                row = dt_holdings.NewRow();
                row["Name"] = oh.Name;
                row["Quantity"] = oh.Quantity;
                row["PurchaseAmount"] = oh.PurchaseAmount.ToString();
                row[("Change")] = oh.Change.ToString(true, true);
                row[("Changepercent")] = Math.Round(oh.ChangePercent, 2, MidpointRounding.AwayFromZero).ToString() + " %";
                row[("CurrentWorth")] = oh.CurrentAmount.ToString();
                dt_holdings.Rows.Add(row);

            }

            dataGridView_holdings.DataSource = dt_holdings;

            if (oA.AccountType != AccountType.Customer)
            {
                dataGridView_holdings.Hide();
            }
            else
                dataGridView_holdings.Show();

            label_Name.Text = oA.Name.ToString();
            label_accounttype.Text = oA.AccountType.ToString();

            //här  måste jag justera så att det blir per valuta!!! Alltid ta ut en lista och redovisa alla valutor för sig!
            List<Money> TotalFundsList = Ic.AccountHandler.GetTotalFundsOfAccount(oA.Identifier).Count > 0 ? Ic.AccountHandler.GetTotalFundsOfAccount(oA.Identifier) : new List<Money>();
            List<Money> TotalDeposit = Ic.AccountHandler.GetDepositedAmountOnAccount(oA.Identifier) != null ? Ic.AccountHandler.GetDepositedAmountOnAccount(oA.Identifier) : new List<Money>();
            /*
            List<Money> NewTotalDeposit = OrigTotalDeposit;//.Select(td=>td.SwitchSign()).ToList();
            List<Money> NewTotalFundsList = OrigTotalFundsList;
            NewTotalFundsList.AddRange(NewTotalDeposit);

            //group by currency
            List<Money> GroupResult = NewTotalFundsList
            .GroupBy(l => l.CurrencyCode)
            .Select(cl => new Money
            {
                CurrencyCode = cl.First().CurrencyCode,
                Amount = cl.Sum(c => c.Amount),
            }).ToList();

            //NewTotalFundsList = NewTotalFundsList.GroupBy(a => a.CurrencyCode).Select(grp => grp.Sum().ToList();
            */
            DataTable dt_Total = new DataTable();
            dt_Total.Columns.Add("Deposit");
            dt_Total.Columns.Add("Amount");
            dt_Total.Columns.Add("Change");
            dt_Total.Columns.Add("ChangePercent");

            foreach (Money oM in TotalFundsList)
            {
                row = dt_Total.NewRow();
                row["Deposit"] = TotalDeposit.Where(od => od.CurrencyCode.Equals(oM.CurrencyCode)).First().ToString();
                row["Amount"] = oM.ToString();
                row["Change"] = oM.Subtract(TotalDeposit.Where(od => od.CurrencyCode.Equals(oM.CurrencyCode)).First()).ToString();
                row["ChangePercent"] = 
                    string.Format("{0}%", TotalDeposit.Where(od => od.CurrencyCode.Equals(oM.CurrencyCode)).First().Amount > 0 ? oM.Subtract(TotalDeposit.Where(od => od.CurrencyCode.Equals(oM.CurrencyCode)).First().Amount).Divide(TotalDeposit.Where(od => od.CurrencyCode.Equals(oM.CurrencyCode)).First().Amount).Multiply(100).ToString(true, false) : "0");

                dt_Total.Rows.Add(row);

            }
            dataGridView_total_worth.DataSource = dt_Total;
            //string sChangeOfAccountInPercent = "";
            //string sChangeOfAccountInAmount = "";
            //Money TotalFunds = Ic.AccountHandler.GetTotalFundsOfAccount(oA.Identifier).Count > 0 ? Ic.AccountHandler.GetTotalFundsOfAccount(oA.Identifier).First() : new Money(0m, "SEK");
            //Money Deposited = Ic.AccountHandler.GetDepositedAmountOnAccount(oA.Identifier) != null ? Ic.AccountHandler.GetDepositedAmountOnAccount(oA.Identifier) : new Money(0m, "SEK");
            //sChangeOfAccountInAmount = TotalFunds.Subtract(Deposited).ToString(true, true);
            //sChangeOfAccountInPercent = string.Format("{0}%", Deposited.Amount > 0 ? TotalFunds.Subtract(Deposited).Divide(Deposited).Multiply(100).ToString(true, false) : "0");


            /*
            label_totalfunds.Text =
                string.Format("{0} ({1} {2})",
                Ic.AccountHandler.GetTotalFundsOfAccount(oA.Identifier).Count > 0 ? Ic.AccountHandler.GetTotalFundsOfAccount(oA.Identifier).First().ToString() : new Money(0m, "SEK").ToString(),
                sChangeOfAccountInAmount,
                sChangeOfAccountInPercent
                );*/
            //only show the securites where the user can buy from
            var AccountCurrencies = Ic.AccountHandler.GetTotalFundsOfAccount(oA.Identifier).GroupBy(a => a.CurrencyCode).Select(grp => grp.First()).ToList();
            var AllowedSecurities =
                    from q in Ic.GetQuotes()
                    join ac in AccountCurrencies on q.Dividend.CurrencyCode equals ac.CurrencyCode
                    where q.Dividend.Amount <= ac.Amount
                    select q;
            _AllowedSecurities = AllowedSecurities.Select(a => a.Symbol).ToList();
        }

        private DataGridViewTextBoxColumn CreateTextBox(string Name, string HeaderText, string DataPropertyName)
        {
            DataGridViewCell cell = new DataGridViewTextBoxCell();
            return new DataGridViewTextBoxColumn()
            {
                CellTemplate = cell,
                Name = Name,
                HeaderText = HeaderText,
                DataPropertyName = Name
            };
        }

        private void Form_Activated(object sender, System.EventArgs e)
        {
           
        }

        private void button1_Click(object sender, EventArgs e)
        {

            Ic.AccountHandler.SaveAccounts();
        }

        private void button_buy_security_Click(object sender, EventArgs e)
        {
            int QuantityToBuy;
            bool isNumeric = int.TryParse(textBox_buy_security_quantity.Text.Trim(), out QuantityToBuy);
            if (_SelectedAccount != null && isNumeric && comboBox_tickers.SelectedItem.ToString().Length> 0)
            {
                if (!Ic.AccountHandler.AddHoldingToAccount(
                    _SelectedAccount.Identifier,
                    QuantityToBuy,
                    comboBox_tickers.SelectedItem.ToString()
                    ))
                {
                    MessageBox.Show(string.Format("Couldn't complete transaction! {0}", Ic.AccountHandler.GetLastErrorMessage));
                }
                else
                {
                    //refresh current account and list!
                    RefreshData();
                }
             }
        }
        /// <summary>
        /// Deposit money on the account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click_1(object sender, EventArgs e)
        {
            decimal AmountToDeposit;
            bool isNumeric = decimal.TryParse(this.textBox_deposit_amount.Text.Trim(), out AmountToDeposit);
            //deposit
            if (_SelectedAccount != null && _SelectedAccount.AccountType.Equals(AccountType.Customer) && isNumeric && textBox_deposit_currency.Text != "")
            {
                if (
                !Ic.AccountHandler.DepositAmount(_SelectedAccount.Identifier, new Money(AmountToDeposit, textBox_deposit_currency.Text.Trim().ToUpper())))
                    MessageBox.Show("The deposit failed");
                else
                    RefreshData();
            }
        }
        private void RefreshData()
        {
            RefreshAccountlist();
            if (_SelectedAccount != null)
                PopulateCurrentAccount(_SelectedAccount.Identifier.ToString());
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
