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
using Imperatur_v2.shared;
using Imperatur_Market_Client.events;
using Imperatur_v2.monetary;
using System.Net;

namespace Imperatur_Market_Client.control
{
    public partial class Account_Trade : UserControl
    {
        public event MainForm.SelectedAccountEventHandler SelectedAccount;
        private IAccountHandlerInterface m_oAh;
        private IAccountInterface m_oAccountData;
        private ITradeHandlerInterface m_oTradeHandler;

        public Account_Trade(IAccountHandlerInterface AccountHandler, ITradeHandlerInterface TradeHandler)
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            m_oAh = AccountHandler;
            m_oTradeHandler = TradeHandler;
            //comboBox_Symbols
            AutoCompleteStringCollection list = new AutoCompleteStringCollection();
            list.AddRange(ImperaturGlobal.Instruments.Select(i => i.Symbol).ToArray());
            comboBox_Symbols.AutoCompleteMode = AutoCompleteMode.Suggest;
            comboBox_Symbols.AutoCompleteSource = AutoCompleteSource.CustomSource;
            comboBox_Symbols.AutoCompleteCustomSource = list;
            comboBox_Symbols.DataSource = ImperaturGlobal.Instruments.Select(i => i.Symbol).ToList();
            comboBox_Symbols.Text = "";
            comboBox_Symbols.SelectedIndexChanged += ComboBox_Symbols_SelectedIndexChanged;
            this.textBox_Quantity.KeyDown += TextBox_Quantity_KeyDown;
        }

        private void TextBox_Quantity_KeyDown(object sender, KeyEventArgs e)
        {
            int QuantityToBuy;
            bool isNumeric = int.TryParse(textBox_Quantity.Text.Trim(), out QuantityToBuy);
            if (m_oAccountData != null && isNumeric && comboBox_Symbols.SelectedItem.ToString().Length > 0)
            {

                IMoney MarketValue = ImperaturGlobal.Quotes.Where(q => q.Symbol.Equals(comboBox_Symbols.SelectedItem.ToString())).First().LastTradePrice.Multiply(Convert.ToDecimal(QuantityToBuy));
                if (
                    MarketValue.Amount >
                    m_oAccountData.GetAvailableFunds().Where(m => m.CurrencyCode.Equals(MarketValue.CurrencyCode)).First().Amount
                    )
                {
                    button_BuySecurity.Enabled = false;
                    return;
                }
                else
                    button_BuySecurity.Enabled = true;
            }

                if (e.KeyCode == Keys.Enter)
            {
                button_BuySecurity_Click(this, null);
            }

        }

        protected virtual void OnSelectedAccount(SelectedAccountEventArg e)
        {
            if (SelectedAccount != null)
                SelectedAccount(this, e);
        }

        private void ComboBox_Symbols_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox_Symbols.SelectedItem != null)
            {
                if (ImperaturGlobal.Quotes.Where(q => q.Symbol.Equals(comboBox_Symbols.SelectedItem.ToString())).Count() > 0)
                {
                    label_instrument_info.Text = ImperaturGlobal.Quotes.Where(q => q.Symbol.Equals(comboBox_Symbols.SelectedItem.ToString())).First().LastTradePrice.ToString();
                    using (WebClient webClient = new WebClient())
                    {
                        //webClient.DownloadFile("https://www.google.com/finance/getchart?q=" + comboBox_Symbols.SelectedItem.ToString(), "image.png");
                        pictureBox_graph.Load("https://www.google.com/finance/getchart?q=" + comboBox_Symbols.SelectedItem.ToString().Replace(" ", "-"));
                        pictureBox_graph.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                }
                else
                    label_instrument_info.Text = "N/A";
            }
        }

        public void UpdateAccountInfo(IAccountInterface AccountData)
        {
            m_oAccountData = AccountData;
            var newSymbolsToShow = from i in ImperaturGlobal.Instruments
                                   join a in m_oAccountData.GetAvailableFunds() on i.CurrencyCode equals a.CurrencyCode.GetCurrencyString()
                                   select i.Symbol;
            comboBox_Symbols.DataSource = newSymbolsToShow.ToList();
            comboBox_Symbols.Refresh();
        }

        private void button_BuySecurity_Click(object sender, EventArgs e)
        {
            int QuantityToBuy;
            bool isNumeric = int.TryParse(textBox_Quantity.Text.Trim(), out QuantityToBuy);
            if (m_oAccountData != null && isNumeric && comboBox_Symbols.SelectedItem.ToString().Length > 0)
            {
                /*
                IMoney MarketValue = ImperaturGlobal.Quotes.Where(q => q.Symbol.Equals(comboBox_Symbols.SelectedItem.ToString())).First().LastTradePrice.Multiply(Convert.ToDecimal(QuantityToBuy));
                if (
                    MarketValue.Amount >
                    m_oAccountData.GetAvailableFunds().Where(m=>m.CurrencyCode.Equals(MarketValue.CurrencyCode)).First().Amount
                    )
                {
                    MessageBox.Show(string.Format
                }*/
                string sMessage = string.Format("Are you sure you want to buy {0} of {1} for {2}?",
                    QuantityToBuy,
                    comboBox_Symbols.SelectedItem.ToString(),
                    ImperaturGlobal.Quotes.Where(q => q.Symbol.Equals(comboBox_Symbols.SelectedItem.ToString())).First().LastTradePrice.Multiply(Convert.ToDecimal(QuantityToBuy)).ToString());
                DialogResult dialogResult = MessageBox.Show(sMessage, "Buy stock?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    if (!m_oAccountData.AddHoldingToAccount(
                        QuantityToBuy,
                        comboBox_Symbols.SelectedItem.ToString(),
                        m_oTradeHandler
                        ))
                    {
                        MessageBox.Show(string.Format("Couldn't complete transaction! {0}", m_oAccountData.GetLastErrorMessage));
                    }
                    else
                    {
                        //refresh current account and list!
                        RefreshData();
                    }
                }
                else if (dialogResult == DialogResult.No)
                {
                    //do something else
                }


            }
        }

        private void RefreshData()
        {
            OnSelectedAccount(new SelectedAccountEventArg()
            {
                Identifier = m_oAccountData.Identifier
            });
        }
    }
}
