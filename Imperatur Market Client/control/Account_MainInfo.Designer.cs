namespace Imperatur_Market_Client.control
{
    partial class Account_MainInfo
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel_maininfo = new System.Windows.Forms.TableLayoutPanel();
            this.button_Show_Transactions = new System.Windows.Forms.Button();
            this.tableLayoutPanel_maininfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel_maininfo
            // 
            this.tableLayoutPanel_maininfo.ColumnCount = 1;
            this.tableLayoutPanel_maininfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_maininfo.Controls.Add(this.button_Show_Transactions, 0, 3);
            this.tableLayoutPanel_maininfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_maininfo.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel_maininfo.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutPanel_maininfo.Name = "tableLayoutPanel_maininfo";
            this.tableLayoutPanel_maininfo.RowCount = 4;
            this.tableLayoutPanel_maininfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 122F));
            this.tableLayoutPanel_maininfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_maininfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_maininfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel_maininfo.Size = new System.Drawing.Size(216, 210);
            this.tableLayoutPanel_maininfo.TabIndex = 0;
            // 
            // button_Show_Transactions
            // 
            this.button_Show_Transactions.Location = new System.Drawing.Point(2, 180);
            this.button_Show_Transactions.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button_Show_Transactions.Name = "button_Show_Transactions";
            this.button_Show_Transactions.Size = new System.Drawing.Size(79, 19);
            this.button_Show_Transactions.TabIndex = 0;
            this.button_Show_Transactions.Text = "Transactions";
            this.button_Show_Transactions.UseVisualStyleBackColor = true;
            this.button_Show_Transactions.Click += new System.EventHandler(this.button_Show_Transactions_Click);
            // 
            // Account_MainInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel_maininfo);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Account_MainInfo";
            this.Size = new System.Drawing.Size(216, 210);
            this.tableLayoutPanel_maininfo.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_maininfo;
        private System.Windows.Forms.Button button_Show_Transactions;
    }
}
