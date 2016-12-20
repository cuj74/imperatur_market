using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Imperatur.monetary;
using Imperatur.account;

namespace Imperatur_test_form
{
    public partial class SellDialog : Form
    {
        public int ReturnQuantity { get; set; }
        private Imperatur.handler.AccountHandler oAH;
        private Account oA;
        private int oQ;
        private string oT;
        public SellDialog(Imperatur.handler.AccountHandler AccountHandler, Account Account, string Ticker, int Quantity)
        {
            InitializeComponent();
            oAH = AccountHandler;
            oA = Account;
            textBox_quantity.TextChanged += TextBox_quantity_TextChanged;
            oQ = Quantity;
            oT = Ticker;
            groupBox1.Text += ": " + Ticker;  
        }

        private void TextBox_quantity_TextChanged(object sender, EventArgs e)
        {
            int nQ;
            if (Int32.TryParse(textBox_quantity.Text, out nQ) && nQ <= oQ && nQ > 0)
            {
                Money Rev = oAH.CalculateHoldingSell(oA.Identifier, nQ, oT);
                if (Rev != null)
                    label_revenue.Text = Rev.ToString(true, true);

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
