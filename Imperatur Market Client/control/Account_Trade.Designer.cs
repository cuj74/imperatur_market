﻿namespace Imperatur_Market_Client.control
{
    partial class Account_Trade
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
            PresentationControls.CheckBoxProperties checkBoxProperties1 = new PresentationControls.CheckBoxProperties();
            PresentationControls.CheckBoxProperties checkBoxProperties2 = new PresentationControls.CheckBoxProperties();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel_Trade = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button_buy_recommdation = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label_instrument_info = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_Quantity = new System.Windows.Forms.TextBox();
            this.button_BuySecurity = new System.Windows.Forms.Button();
            this.comboBox_Symbols = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel_Graph = new System.Windows.Forms.TableLayoutPanel();
            this.panel_chart = new System.Windows.Forms.Panel();
            this.panel_vol = new System.Windows.Forms.Panel();
            this.comboBox_daterange = new System.Windows.Forms.ComboBox();
            this.checkBoxComboBox_TA = new PresentationControls.CheckBoxComboBox();
            this.checkBoxComboBox_Settings = new PresentationControls.CheckBoxComboBox();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel_Trade.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel_Graph.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel_Trade);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Size = new System.Drawing.Size(543, 341);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Trade";
            // 
            // tableLayoutPanel_Trade
            // 
            this.tableLayoutPanel_Trade.ColumnCount = 1;
            this.tableLayoutPanel_Trade.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_Trade.Controls.Add(this.button_buy_recommdation, 0, 2);
            this.tableLayoutPanel_Trade.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel_Trade.Controls.Add(this.tableLayoutPanel_Graph, 0, 1);
            this.tableLayoutPanel_Trade.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_Trade.Location = new System.Drawing.Point(3, 17);
            this.tableLayoutPanel_Trade.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanel_Trade.Name = "tableLayoutPanel_Trade";
            this.tableLayoutPanel_Trade.RowCount = 3;
            this.tableLayoutPanel_Trade.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel_Trade.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel_Trade.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel_Trade.Size = new System.Drawing.Size(537, 322);
            this.tableLayoutPanel_Trade.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label_instrument_info);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.textBox_Quantity);
            this.panel1.Controls.Add(this.button_BuySecurity);
            this.panel1.Controls.Add(this.comboBox_Symbols);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 2);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(531, 44);
            this.panel1.TabIndex = 0;
            // 
            // button_buy_recommdation
            // 
            this.button_buy_recommdation.Location = new System.Drawing.Point(4, 245);
            this.button_buy_recommdation.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button_buy_recommdation.Name = "button_buy_recommdation";
            this.button_buy_recommdation.Size = new System.Drawing.Size(100, 28);
            this.button_buy_recommdation.TabIndex = 1;
            this.button_buy_recommdation.Text = "Recommendation";
            this.button_buy_recommdation.UseVisualStyleBackColor = true;
            this.button_buy_recommdation.Click += new System.EventHandler(this.button_buy_recommdation_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(290, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 17);
            this.label3.TabIndex = 1;
            // 
            // label_instrument_info
            // 
            this.label_instrument_info.AutoSize = true;
            this.label_instrument_info.Location = new System.Drawing.Point(12, 70);
            this.label_instrument_info.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_instrument_info.Name = "label_instrument_info";
            this.label_instrument_info.Size = new System.Drawing.Size(0, 17);
            this.label_instrument_info.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(168, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "Quantity";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "Instrument";
            // 
            // textBox_Quantity
            // 
            this.textBox_Quantity.Location = new System.Drawing.Point(171, 34);
            this.textBox_Quantity.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox_Quantity.Name = "textBox_Quantity";
            this.textBox_Quantity.Size = new System.Drawing.Size(100, 22);
            this.textBox_Quantity.TabIndex = 2;
            // 
            // button_BuySecurity
            // 
            this.button_BuySecurity.Location = new System.Drawing.Point(293, 33);
            this.button_BuySecurity.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button_BuySecurity.Name = "button_BuySecurity";
            this.button_BuySecurity.Size = new System.Drawing.Size(75, 23);
            this.button_BuySecurity.TabIndex = 1;
            this.button_BuySecurity.Text = "Buy";
            this.button_BuySecurity.UseVisualStyleBackColor = true;
            this.button_BuySecurity.Click += new System.EventHandler(this.button_BuySecurity_Click);
            // 
            // comboBox_Symbols
            // 
            this.comboBox_Symbols.FormattingEnabled = true;
            this.comboBox_Symbols.Location = new System.Drawing.Point(16, 34);
            this.comboBox_Symbols.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBox_Symbols.Name = "comboBox_Symbols";
            this.comboBox_Symbols.Size = new System.Drawing.Size(121, 24);
            this.comboBox_Symbols.TabIndex = 0;
            // 
            // tableLayoutPanel_Graph
            // 
            this.tableLayoutPanel_Graph.ColumnCount = 6;
            this.tableLayoutPanel_Graph.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel_Graph.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel_Graph.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel_Graph.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel_Graph.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel_Graph.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel_Graph.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel_Graph.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel_Graph.Controls.Add(this.panel_chart, 0, 0);
            this.tableLayoutPanel_Graph.Controls.Add(this.panel_vol, 0, 1);
            this.tableLayoutPanel_Graph.Controls.Add(this.comboBox_daterange, 0, 2);
            this.tableLayoutPanel_Graph.Controls.Add(this.checkBoxComboBox_TA, 1, 2);
            this.tableLayoutPanel_Graph.Controls.Add(this.checkBoxComboBox_Settings, 2, 2);
            this.tableLayoutPanel_Graph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_Graph.Location = new System.Drawing.Point(3, 50);
            this.tableLayoutPanel_Graph.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanel_Graph.Name = "tableLayoutPanel_Graph";
            this.tableLayoutPanel_Graph.RowCount = 3;
            this.tableLayoutPanel_Graph.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 76.92308F));
            this.tableLayoutPanel_Graph.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 23.07692F));
            this.tableLayoutPanel_Graph.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tableLayoutPanel_Graph.Size = new System.Drawing.Size(531, 189);
            this.tableLayoutPanel_Graph.TabIndex = 2;
            // 
            // panel_chart
            // 
            this.tableLayoutPanel_Graph.SetColumnSpan(this.panel_chart, 6);
            this.panel_chart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_chart.Location = new System.Drawing.Point(4, 4);
            this.panel_chart.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel_chart.Name = "panel_chart";
            this.panel_chart.Size = new System.Drawing.Size(524, 111);
            this.panel_chart.TabIndex = 3;
            // 
            // panel_vol
            // 
            this.tableLayoutPanel_Graph.SetColumnSpan(this.panel_vol, 6);
            this.panel_vol.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_vol.Location = new System.Drawing.Point(4, 123);
            this.panel_vol.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel_vol.Name = "panel_vol";
            this.panel_vol.Size = new System.Drawing.Size(524, 27);
            this.panel_vol.TabIndex = 3;
            // 
            // comboBox_daterange
            // 
            this.comboBox_daterange.FormattingEnabled = true;
            this.comboBox_daterange.Location = new System.Drawing.Point(3, 157);
            this.comboBox_daterange.Name = "comboBox_daterange";
            this.comboBox_daterange.Size = new System.Drawing.Size(94, 24);
            this.comboBox_daterange.TabIndex = 4;
            this.comboBox_daterange.Text = "Range";
            // 
            // checkBoxComboBox_TA
            // 
            checkBoxProperties1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBoxComboBox_TA.CheckBoxProperties = checkBoxProperties1;
            this.checkBoxComboBox_TA.DisplayMemberSingleItem = "";
            this.checkBoxComboBox_TA.FormattingEnabled = true;
            this.checkBoxComboBox_TA.Location = new System.Drawing.Point(103, 157);
            this.checkBoxComboBox_TA.Name = "checkBoxComboBox_TA";
            this.checkBoxComboBox_TA.Size = new System.Drawing.Size(94, 24);
            this.checkBoxComboBox_TA.TabIndex = 5;
            this.checkBoxComboBox_TA.Text = "TA";
            // 
            // checkBoxComboBox_Settings
            // 
            checkBoxProperties2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBoxComboBox_Settings.CheckBoxProperties = checkBoxProperties2;
            this.checkBoxComboBox_Settings.DisplayMemberSingleItem = "";
            this.checkBoxComboBox_Settings.FormattingEnabled = true;
            this.checkBoxComboBox_Settings.Location = new System.Drawing.Point(203, 157);
            this.checkBoxComboBox_Settings.Name = "checkBoxComboBox_Settings";
            this.checkBoxComboBox_Settings.Size = new System.Drawing.Size(94, 24);
            this.checkBoxComboBox_Settings.TabIndex = 6;
            this.checkBoxComboBox_Settings.Text = "Settings";
            // 
            // Account_Trade
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Account_Trade";
            this.Size = new System.Drawing.Size(543, 341);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel_Trade.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel_Graph.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Trade;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox comboBox_Symbols;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_Quantity;
        private System.Windows.Forms.Button button_BuySecurity;
        private System.Windows.Forms.Label label_instrument_info;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button_buy_recommdation;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Graph;
        private System.Windows.Forms.Panel panel_chart;
        private System.Windows.Forms.Panel panel_vol;
        private System.Windows.Forms.ComboBox comboBox_daterange;
        private PresentationControls.CheckBoxComboBox checkBoxComboBox_TA;
        private PresentationControls.CheckBoxComboBox checkBoxComboBox_Settings;
    }
}
