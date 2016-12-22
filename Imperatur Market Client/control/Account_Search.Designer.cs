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
            this.button1 = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 72.01794F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.98206F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.textBox_Search, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.listView_searchresult, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.button_search, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.button1, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 41F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(250, 337);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // textBox_Search
            // 
            this.textBox_Search.Location = new System.Drawing.Point(2, 2);
            this.textBox_Search.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox_Search.Name = "textBox_Search";
            this.textBox_Search.Size = new System.Drawing.Size(121, 20);
            this.textBox_Search.TabIndex = 1;
            // 
            // button_search
            // 
            this.button_search.Location = new System.Drawing.Point(167, 2);
            this.button_search.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button_search.Name = "button_search";
            this.button_search.Size = new System.Drawing.Size(56, 19);
            this.button_search.TabIndex = 2;
            this.button_search.Text = "Search";
            this.button_search.UseVisualStyleBackColor = true;
            this.button_search.Click += new System.EventHandler(this.button_search_Click);
            // 
            // listView_searchresult
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.listView_searchresult, 3);
            this.listView_searchresult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_searchresult.FullRowSelect = true;
            this.listView_searchresult.Location = new System.Drawing.Point(2, 43);
            this.listView_searchresult.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.listView_searchresult.Name = "listView_searchresult";
            this.listView_searchresult.Size = new System.Drawing.Size(246, 292);
            this.listView_searchresult.TabIndex = 4;
            this.listView_searchresult.UseCompatibleStateImageBehavior = false;
            this.listView_searchresult.View = System.Windows.Forms.View.Details;
            // 
            // button1
            // 
            this.button1.Image = global::Imperatur_Market_Client.Properties.Resources.Collapse;
            this.button1.Location = new System.Drawing.Point(232, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(15, 23);
            this.button1.TabIndex = 5;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Account_Search
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Account_Search";
            this.Size = new System.Drawing.Size(250, 337);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox textBox_Search;
        private System.Windows.Forms.Button button_search;
        private System.Windows.Forms.ListView listView_searchresult;
        private System.Windows.Forms.Button button1;
    }
}
