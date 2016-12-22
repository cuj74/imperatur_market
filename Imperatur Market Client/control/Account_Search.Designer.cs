namespace Imperatur_Market_Client.control
{
    partial class Account_Search
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.textBox_Search = new System.Windows.Forms.TextBox();
            this.button_search = new System.Windows.Forms.Button();
            this.listView_searchresult = new System.Windows.Forms.ListView();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.textBox_Search, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.button_search, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.listView_searchresult, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(333, 415);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // textBox_Search
            // 
            this.textBox_Search.Location = new System.Drawing.Point(3, 3);
            this.textBox_Search.Name = "textBox_Search";
            this.textBox_Search.Size = new System.Drawing.Size(160, 22);
            this.textBox_Search.TabIndex = 1;
            // 
            // button_search
            // 
            this.button_search.Location = new System.Drawing.Point(169, 3);
            this.button_search.Name = "button_search";
            this.button_search.Size = new System.Drawing.Size(75, 23);
            this.button_search.TabIndex = 2;
            this.button_search.Text = "Search";
            this.button_search.UseVisualStyleBackColor = true;
            this.button_search.Click += new System.EventHandler(this.button_search_Click);
            // 
            // listView_searchresult
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.listView_searchresult, 2);
            this.listView_searchresult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_searchresult.FullRowSelect = true;
            this.listView_searchresult.Location = new System.Drawing.Point(3, 53);
            this.listView_searchresult.Name = "listView_searchresult";
            this.listView_searchresult.Size = new System.Drawing.Size(327, 359);
            this.listView_searchresult.TabIndex = 4;
            this.listView_searchresult.UseCompatibleStateImageBehavior = false;
            this.listView_searchresult.View = System.Windows.Forms.View.Details;
            // 
            // Account_Search
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Account_Search";
            this.Size = new System.Drawing.Size(333, 415);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox textBox_Search;
        private System.Windows.Forms.Button button_search;
        private System.Windows.Forms.ListView listView_searchresult;
    }
}
