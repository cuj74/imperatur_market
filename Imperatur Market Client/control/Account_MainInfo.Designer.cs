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
            this.SuspendLayout();
            // 
            // tableLayoutPanel_maininfo
            // 
            this.tableLayoutPanel_maininfo.ColumnCount = 1;
            this.tableLayoutPanel_maininfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_maininfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel_maininfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_maininfo.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel_maininfo.Name = "tableLayoutPanel_maininfo";
            this.tableLayoutPanel_maininfo.RowCount = 2;
            this.tableLayoutPanel_maininfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_maininfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_maininfo.Size = new System.Drawing.Size(288, 258);
            this.tableLayoutPanel_maininfo.TabIndex = 0;
            // 
            // Account_MainInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel_maininfo);
            this.Name = "Account_MainInfo";
            this.Size = new System.Drawing.Size(288, 258);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_maininfo;
    }
}
