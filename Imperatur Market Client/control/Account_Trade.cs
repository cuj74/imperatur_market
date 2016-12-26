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
        private IAccountHandlerInterface m_oAh;
        private IAccountInterface m_oAccountData;

        public Account_Trade(IAccountHandlerInterface AccountHandler)
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            m_oAh = AccountHandler;
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

        private void ComboBox_Symbols_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox_Symbols.SelectedItem != null)
            {
                label_instrument_info.Text = ImperaturGlobal.Quotes.Where(q => q.Symbol.Equals(comboBox_Symbols.SelectedItem.ToString())).First().Dividend.ToString();
            }
        }

        public void UpdateAcountInfo(IAccountInterface AccountData)
        {
            m_oAccountData = AccountData;
            var newSymbolsToShow = from i in ImperaturGlobal.Instruments
                                   join a in m_oAccountData.GetAvailableFunds() on i.CurrencyCode equals a.CurrencyCode().GetCurrencyString()
                                   select i.Symbol;
            comboBox_Symbols.DataSource = newSymbolsToShow.ToList();
            comboBox_Symbols.Refresh();
        }

        private void button_BuySecurity_Click(object sender, EventArgs e)
        {
            //m_oAccountData.
        }
    }
}
