namespace Imperatur_test_form
{
    partial class Form_imperaturclient
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button_accountlist = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox_Account = new System.Windows.Forms.GroupBox();
            this.label_totalfunds = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button_deposit = new System.Windows.Forms.Button();
            this.textBox_deposit_currency = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox_deposit_amount = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label_ticker_info = new System.Windows.Forms.Label();
            this.comboBox_tickers = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.button_buy_security = new System.Windows.Forms.Button();
            this.textBox_buy_security_quantity = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.dataGridView_total_worth = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.label_Name = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label_accounttype = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dataGridView_availablefunds = new System.Windows.Forms.DataGridView();
            this.label3 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dataGridView_holdings = new System.Windows.Forms.DataGridView();
            this.label4 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuStrip2 = new System.Windows.Forms.MenuStrip();
            this.archiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox_Account.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_total_worth)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_availablefunds)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_holdings)).BeginInit();
            this.menuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_accountlist
            // 
            this.button_accountlist.Location = new System.Drawing.Point(12, 528);
            this.button_accountlist.Name = "button_accountlist";
            this.button_accountlist.Size = new System.Drawing.Size(109, 23);
            this.button_accountlist.TabIndex = 0;
            this.button_accountlist.Text = "Save accounts";
            this.button_accountlist.UseVisualStyleBackColor = true;
            this.button_accountlist.Click += new System.EventHandler(this.button1_Click);
            // 
            // listView1
            // 
            this.listView1.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listView1.Dock = System.Windows.Forms.DockStyle.Top;
            this.listView1.FullRowSelect = true;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(341, 472);
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 52);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listView1);
            this.splitContainer1.Panel1.Controls.Add(this.button_accountlist);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox_Account);
            this.splitContainer1.Size = new System.Drawing.Size(1308, 721);
            this.splitContainer1.SplitterDistance = 341;
            this.splitContainer1.TabIndex = 2;
            // 
            // groupBox_Account
            // 
            this.groupBox_Account.AutoSize = true;
            this.groupBox_Account.Controls.Add(this.label_totalfunds);
            this.groupBox_Account.Controls.Add(this.groupBox2);
            this.groupBox_Account.Controls.Add(this.groupBox1);
            this.groupBox_Account.Controls.Add(this.tableLayoutPanel1);
            this.groupBox_Account.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_Account.Location = new System.Drawing.Point(0, 0);
            this.groupBox_Account.Name = "groupBox_Account";
            this.groupBox_Account.Size = new System.Drawing.Size(963, 721);
            this.groupBox_Account.TabIndex = 0;
            this.groupBox_Account.TabStop = false;
            this.groupBox_Account.Text = "Account info";
            // 
            // label_totalfunds
            // 
            this.label_totalfunds.AutoSize = true;
            this.label_totalfunds.Location = new System.Drawing.Point(886, 674);
            this.label_totalfunds.Name = "label_totalfunds";
            this.label_totalfunds.Size = new System.Drawing.Size(45, 17);
            this.label_totalfunds.TabIndex = 2;
            this.label_totalfunds.Text = "Name";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button_deposit);
            this.groupBox2.Controls.Add(this.textBox_deposit_currency);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.textBox_deposit_amount);
            this.groupBox2.Location = new System.Drawing.Point(15, 617);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(651, 74);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Deposit";
            // 
            // button_deposit
            // 
            this.button_deposit.Location = new System.Drawing.Point(542, 20);
            this.button_deposit.Name = "button_deposit";
            this.button_deposit.Size = new System.Drawing.Size(75, 23);
            this.button_deposit.TabIndex = 9;
            this.button_deposit.Text = "Deposit";
            this.button_deposit.UseVisualStyleBackColor = true;
            this.button_deposit.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // textBox_deposit_currency
            // 
            this.textBox_deposit_currency.Location = new System.Drawing.Point(307, 21);
            this.textBox_deposit_currency.Name = "textBox_deposit_currency";
            this.textBox_deposit_currency.Size = new System.Drawing.Size(100, 22);
            this.textBox_deposit_currency.TabIndex = 8;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(236, 26);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 17);
            this.label8.TabIndex = 7;
            this.label8.Text = "Currency";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(32, 26);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 17);
            this.label7.TabIndex = 6;
            this.label7.Text = "Amount";
            // 
            // textBox_deposit_amount
            // 
            this.textBox_deposit_amount.Location = new System.Drawing.Point(106, 21);
            this.textBox_deposit_amount.Name = "textBox_deposit_amount";
            this.textBox_deposit_amount.Size = new System.Drawing.Size(100, 22);
            this.textBox_deposit_amount.TabIndex = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label_ticker_info);
            this.groupBox1.Controls.Add(this.comboBox_tickers);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.button_buy_security);
            this.groupBox1.Controls.Add(this.textBox_buy_security_quantity);
            this.groupBox1.Location = new System.Drawing.Point(15, 494);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(651, 117);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Buy";
            // 
            // label_ticker_info
            // 
            this.label_ticker_info.AutoSize = true;
            this.label_ticker_info.Location = new System.Drawing.Point(9, 65);
            this.label_ticker_info.Name = "label_ticker_info";
            this.label_ticker_info.Size = new System.Drawing.Size(46, 17);
            this.label_ticker_info.TabIndex = 6;
            this.label_ticker_info.Text = "label9";
            // 
            // comboBox_tickers
            // 
            this.comboBox_tickers.FormattingEnabled = true;
            this.comboBox_tickers.Location = new System.Drawing.Point(9, 21);
            this.comboBox_tickers.Name = "comboBox_tickers";
            this.comboBox_tickers.Size = new System.Drawing.Size(121, 24);
            this.comboBox_tickers.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(148, 27);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 17);
            this.label6.TabIndex = 5;
            this.label6.Text = "Quantity";
            // 
            // button_buy_security
            // 
            this.button_buy_security.Location = new System.Drawing.Point(524, 23);
            this.button_buy_security.Name = "button_buy_security";
            this.button_buy_security.Size = new System.Drawing.Size(75, 23);
            this.button_buy_security.TabIndex = 2;
            this.button_buy_security.Text = "Buy";
            this.button_buy_security.UseVisualStyleBackColor = true;
            this.button_buy_security.Click += new System.EventHandler(this.button_buy_security_Click);
            // 
            // textBox_buy_security_quantity
            // 
            this.textBox_buy_security_quantity.Location = new System.Drawing.Point(230, 24);
            this.textBox_buy_security_quantity.Name = "textBox_buy_security_quantity";
            this.textBox_buy_security_quantity.Size = new System.Drawing.Size(100, 22);
            this.textBox_buy_security_quantity.TabIndex = 4;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.panel3, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label_Name, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label_accounttype, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 3);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(15, 21);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(838, 467);
            this.tableLayoutPanel1.TabIndex = 1;
            this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // panel3
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.panel3, 2);
            this.panel3.Controls.Add(this.label5);
            this.panel3.Controls.Add(this.dataGridView_total_worth);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 349);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(832, 115);
            this.panel3.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 1);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 17);
            this.label5.TabIndex = 5;
            this.label5.Text = "Total worth";
            // 
            // dataGridView_total_worth
            // 
            this.dataGridView_total_worth.AllowUserToAddRows = false;
            this.dataGridView_total_worth.AllowUserToDeleteRows = false;
            this.dataGridView_total_worth.AllowUserToResizeRows = false;
            this.dataGridView_total_worth.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.dataGridView_total_worth.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_total_worth.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dataGridView_total_worth.Location = new System.Drawing.Point(0, 21);
            this.dataGridView_total_worth.Name = "dataGridView_total_worth";
            this.dataGridView_total_worth.RowTemplate.Height = 24;
            this.dataGridView_total_worth.Size = new System.Drawing.Size(832, 94);
            this.dataGridView_total_worth.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Account type";
            // 
            // label_Name
            // 
            this.label_Name.AutoSize = true;
            this.label_Name.Location = new System.Drawing.Point(422, 0);
            this.label_Name.Name = "label_Name";
            this.label_Name.Size = new System.Drawing.Size(0, 17);
            this.label_Name.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // label_accounttype
            // 
            this.label_accounttype.AutoSize = true;
            this.label_accounttype.Location = new System.Drawing.Point(422, 20);
            this.label_accounttype.Name = "label_accounttype";
            this.label_accounttype.Size = new System.Drawing.Size(90, 17);
            this.label_accounttype.TabIndex = 2;
            this.label_accounttype.Text = "Account type";
            // 
            // panel1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.panel1, 2);
            this.panel1.Controls.Add(this.dataGridView_availablefunds);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 43);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(832, 147);
            this.panel1.TabIndex = 5;
            // 
            // dataGridView_availablefunds
            // 
            this.dataGridView_availablefunds.AllowUserToAddRows = false;
            this.dataGridView_availablefunds.AllowUserToDeleteRows = false;
            this.dataGridView_availablefunds.AllowUserToResizeRows = false;
            this.dataGridView_availablefunds.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.dataGridView_availablefunds.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_availablefunds.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dataGridView_availablefunds.Location = new System.Drawing.Point(0, 41);
            this.dataGridView_availablefunds.Name = "dataGridView_availablefunds";
            this.dataGridView_availablefunds.RowTemplate.Height = 24;
            this.dataGridView_availablefunds.Size = new System.Drawing.Size(832, 106);
            this.dataGridView_availablefunds.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 17);
            this.label3.TabIndex = 4;
            this.label3.Text = "Available funds";
            // 
            // panel2
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.panel2, 2);
            this.panel2.Controls.Add(this.dataGridView_holdings);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 196);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(832, 147);
            this.panel2.TabIndex = 6;
            // 
            // dataGridView_holdings
            // 
            this.dataGridView_holdings.AllowUserToAddRows = false;
            this.dataGridView_holdings.AllowUserToDeleteRows = false;
            this.dataGridView_holdings.AllowUserToResizeRows = false;
            this.dataGridView_holdings.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.dataGridView_holdings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_holdings.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dataGridView_holdings.Location = new System.Drawing.Point(0, -2);
            this.dataGridView_holdings.Name = "dataGridView_holdings";
            this.dataGridView_holdings.RowTemplate.Height = 24;
            this.dataGridView_holdings.Size = new System.Drawing.Size(832, 149);
            this.dataGridView_holdings.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 17);
            this.label4.TabIndex = 5;
            this.label4.Text = "Holdings";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Location = new System.Drawing.Point(0, 28);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1308, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuStrip2
            // 
            this.menuStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.archiveToolStripMenuItem});
            this.menuStrip2.Location = new System.Drawing.Point(0, 0);
            this.menuStrip2.Name = "menuStrip2";
            this.menuStrip2.Size = new System.Drawing.Size(1308, 28);
            this.menuStrip2.TabIndex = 4;
            this.menuStrip2.Text = "menuStrip2";
            // 
            // archiveToolStripMenuItem
            // 
            this.archiveToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.archiveToolStripMenuItem.Name = "archiveToolStripMenuItem";
            this.archiveToolStripMenuItem.Size = new System.Drawing.Size(70, 24);
            this.archiveToolStripMenuItem.Text = "Archive";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(108, 26);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // Form_imperaturclient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1308, 773);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.menuStrip2);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form_imperaturclient";
            this.Text = "Form_imperaturclient";
            this.Load += new System.EventHandler(this.Form_imperaturclient_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox_Account.ResumeLayout(false);
            this.groupBox_Account.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_total_worth)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_availablefunds)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_holdings)).EndInit();
            this.menuStrip2.ResumeLayout(false);
            this.menuStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_accountlist;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.MenuStrip menuStrip2;
        private System.Windows.Forms.ToolStripMenuItem archiveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox_Account;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label_accounttype;
        private System.Windows.Forms.Label label_Name;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dataGridView_availablefunds;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView dataGridView_holdings;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label_totalfunds;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox_buy_security_quantity;
        private System.Windows.Forms.ComboBox comboBox_tickers;
        private System.Windows.Forms.Button button_buy_security;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button_deposit;
        private System.Windows.Forms.TextBox textBox_deposit_currency;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox_deposit_amount;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label_ticker_info;
        private System.Windows.Forms.DataGridView dataGridView_total_worth;
        private System.Windows.Forms.Panel panel3;
    }
}