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
            list.AddRange(ImperaturGlobal.Instruments.Select(i=>i.Symbol).ToArray());
            comboBox_Symbols.AutoCompleteMode = AutoCompleteMode.Suggest;
            comboBox_Symbols.AutoCompleteSource = AutoCompleteSource.CustomSource;
            comboBox_Symbols.AutoCompleteCustomSource = list;
            comboBox_Symbols.DataSource = ImperaturGlobal.Instruments.Select(i => i.Symbol).ToList();
            comboBox_Symbols.Text = "";
            comboBox_Symbols.SelectedIndexChanged += ComboBox_Symbols_SelectedIndexChanged;
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
