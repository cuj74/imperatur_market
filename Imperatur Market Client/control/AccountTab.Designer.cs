namespace Imperatur_Market_Client.control
{
    partial class AccountTab
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
            this.tlp_Account = new System.Windows.Forms.TableLayoutPanel();
            this.SuspendLayout();
            // 
            // tlp_Account
            // 
            this.tlp_Account.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tlp_Account.ColumnCount = 4;
            this.tlp_Account.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlp_Account.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlp_Account.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlp_Account.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlp_Account.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Account.Location = new System.Drawing.Point(0, 0);
            this.tlp_Account.Name = "tlp_Account";
            this.tlp_Account.Padding = new System.Windows.Forms.Padding(0, 0, 0, 20);
            this.tlp_Account.RowCount = 1;
            this.tlp_Account.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Account.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 693F));
            this.tlp_Account.Size = new System.Drawing.Size(920, 642);
            this.tlp_Account.TabIndex = 1;
            // 
            // AccountTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlp_Account);
            this.Name = "AccountTab";
            this.Size = new System.Drawing.Size(920, 642);
            this.Load += new System.EventHandler(this.AccountTab_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlp_Account;
    }
}
