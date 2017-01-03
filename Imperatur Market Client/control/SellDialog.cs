using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Imperatur_v2.monetary;
using Imperatur_v2.account;
using Imperatur_v2.handler;

namespace Imperatur_Market_Client.control
{
    public partial class SellDialog : Form
    {
        public int ReturnQuantity { get; set; }
        private IAccountHandlerInterface oAH;
       // private ITradeHandlerInterface m_oTradeHandler;
        private IAccountInterface oA;
        private int oQ;
        private string oT;
        public SellDialog(IAccountHandlerInterface AccountHandler, IAccountInterface Account, string Ticker, int Quantity)//, ITradeHandlerInterface Tradehandler)
        {
            InitializeComponent();
            oAH = AccountHandler;
            oA = Account;
            textBox_quantity.TextChanged += TextBox_quantity_TextChanged;
            oQ = Quantity;
            oT = Ticker;
            groupBox1.Text += ": " + Ticker;
            //m_oTradeHandler = Tradehandler;
        }

        private void TextBox_quantity_TextChanged(object sender, EventArgs e)
        {
            int nQ;
            if (Int32.TryParse(textBox_quantity.Text, out nQ) && nQ <= oQ && nQ > 0)
            {
                IMoney Rev = new Money(0m, new Currency("SEK"));
                bool bOK = false;
                try
                {
                    Rev = oAH.CalculateHoldingSell(oA.Identifier, nQ, oT);
                    bOK = true;
                }
                catch
                {
                    //lkjsdfkl
                }

                if (bOK)
                    label_revenue.Text = Rev.ToString(true, true);
                else
                    label_revenue.Text = "N/A";

                button_sell.Enabled = true;
            }
            else
            {
                label_revenue.Text = "";
                button_sell.Enabled = false;
            }
        }

        private void SellDialog_Load(object sender, EventArgs e)
        {
            textBox_quantity.Text = oQ.ToString();
            //label_revenue.Text = "";

        }

        private void button_sell_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("are you sure?", "Are you sure?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                ReturnQuantity = int.Parse(textBox_quantity.Text);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }
    }
}
