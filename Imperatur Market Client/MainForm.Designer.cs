namespace Imperatur_Market_Client
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.archiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripStatusLabel_system = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip_imperatur = new System.Windows.Forms.StatusStrip();
            this.tabPage_systemtest = new System.Windows.Forms.TabPage();
            this.tabPage_system = new System.Windows.Forms.TabPage();
            this.checkBox_automaticTrading = new System.Windows.Forms.CheckBox();
            this.tabControl_Imperatur_main = new System.Windows.Forms.TabControl();
            this.tabPage_account = new System.Windows.Forms.TabPage();
            this.LatestTransactionsPane = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.statusStrip_imperatur.SuspendLayout();
            this.tabPage_system.SuspendLayout();
            this.tabControl_Imperatur_main.SuspendLayout();
            this.SuspendLayout();
            // 
            // archiveToolStripMenuItem
            // 
            this.archiveToolStripMenuItem.Name = "archiveToolStripMenuItem";
            this.archiveToolStripMenuItem.Size = new System.Drawing.Size(70, 24);
            this.archiveToolStripMenuItem.Text = "Archive";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.archiveToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1909, 28);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripStatusLabel_system
            // 
            this.toolStripStatusLabel_system.Name = "toolStripStatusLabel_system";
            this.toolStripStatusLabel_system.Size = new System.Drawing.Size(194, 20);
            this.toolStripStatusLabel_system.Text = "toolStripStatusLabel_system";
            // 
            // statusStrip_imperatur
            // 
            this.statusStrip_imperatur.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip_imperatur.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel_system});
            this.statusStrip_imperatur.Location = new System.Drawing.Point(0, 753);
            this.statusStrip_imperatur.Name = "statusStrip_imperatur";
            this.statusStrip_imperatur.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip_imperatur.Size = new System.Drawing.Size(1909, 25);
            this.statusStrip_imperatur.TabIndex = 4;
            this.statusStrip_imperatur.Text = "statusStrip_imperatur";
            // 
            // tabPage_systemtest
            // 
            this.tabPage_systemtest.Location = new System.Drawing.Point(4, 25);
            this.tabPage_systemtest.Name = "tabPage_systemtest";
            this.tabPage_systemtest.Size = new System.Drawing.Size(1901, 721);
            this.tabPage_systemtest.TabIndex = 2;
            this.tabPage_systemtest.Text = "System Test";
            this.tabPage_systemtest.UseVisualStyleBackColor = true;
            // 
            // tabPage_system
            // 
            this.tabPage_system.Controls.Add(this.LatestTransactionsPane);
            this.tabPage_system.Controls.Add(this.checkBox_automaticTrading);
            this.tabPage_system.Location = new System.Drawing.Point(4, 25);
            this.tabPage_system.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage_system.Name = "tabPage_system";
            this.tabPage_system.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage_system.Size = new System.Drawing.Size(1901, 721);
            this.tabPage_system.TabIndex = 0;
            this.tabPage_system.Text = "System";
            this.tabPage_system.UseVisualStyleBackColor = true;
            // 
            // checkBox_automaticTrading
            // 
            this.checkBox_automaticTrading.AutoSize = true;
            this.checkBox_automaticTrading.Location = new System.Drawing.Point(25, 21);
            this.checkBox_automaticTrading.Name = "checkBox_automaticTrading";
            this.checkBox_automaticTrading.Size = new System.Drawing.Size(140, 21);
            this.checkBox_automaticTrading.TabIndex = 0;
            this.checkBox_automaticTrading.Text = "Automatic trading";
            this.checkBox_automaticTrading.UseVisualStyleBackColor = true;
            this.checkBox_automaticTrading.CheckedChanged += new System.EventHandler(this.checkBox_automaticTrading_CheckedChanged);
            // 
            // tabControl_Imperatur_main
            // 
            this.tabControl_Imperatur_main.Controls.Add(this.tabPage_system);
            this.tabControl_Imperatur_main.Controls.Add(this.tabPage_account);
            this.tabControl_Imperatur_main.Controls.Add(this.tabPage_systemtest);
            this.tabControl_Imperatur_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl_Imperatur_main.Location = new System.Drawing.Point(0, 28);
            this.tabControl_Imperatur_main.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl_Imperatur_main.Name = "tabControl_Imperatur_main";
            this.tabControl_Imperatur_main.SelectedIndex = 0;
            this.tabControl_Imperatur_main.Size = new System.Drawing.Size(1909, 750);
            this.tabControl_Imperatur_main.TabIndex = 0;
            // 
            // tabPage_account
            // 
            this.tabPage_account.Location = new System.Drawing.Point(4, 25);
            this.tabPage_account.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage_account.Name = "tabPage_account";
            this.tabPage_account.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage_account.Size = new System.Drawing.Size(1901, 721);
            this.tabPage_account.TabIndex = 1;
            this.tabPage_account.Text = "Account";
            this.tabPage_account.UseVisualStyleBackColor = true;
            // 
            // LatestTransactionsPane
            // 
            this.LatestTransactionsPane.Location = new System.Drawing.Point(25, 65);
            this.LatestTransactionsPane.Name = "LatestTransactionsPane";
            this.LatestTransactionsPane.Size = new System.Drawing.Size(899, 632);
            this.LatestTransactionsPane.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1909, 778);
            this.Controls.Add(this.statusStrip_imperatur);
            this.Controls.Add(this.tabControl_Imperatur_main);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Text = "Imperatur Market";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip_imperatur.ResumeLayout(false);
            this.statusStrip_imperatur.PerformLayout();
            this.tabPage_system.ResumeLayout(false);
            this.tabPage_system.PerformLayout();
            this.tabControl_Imperatur_main.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripMenuItem archiveToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_system;
        private System.Windows.Forms.StatusStrip statusStrip_imperatur;
        private System.Windows.Forms.TabPage tabPage_systemtest;
        private System.Windows.Forms.TabPage tabPage_system;
        private System.Windows.Forms.TabControl tabControl_Imperatur_main;
        private System.Windows.Forms.TabPage tabPage_account;
        private System.Windows.Forms.CheckBox checkBox_automaticTrading;
        private System.Windows.Forms.Panel LatestTransactionsPane;
    }
}

