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
            this.tabControl_Imperatur_main = new System.Windows.Forms.TabControl();
            this.tabPage_system = new System.Windows.Forms.TabPage();
            this.tabPage_account = new System.Windows.Forms.TabPage();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.archiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip_imperatur = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel_system = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl_Imperatur_main.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.statusStrip_imperatur.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl_Imperatur_main
            // 
            this.tabControl_Imperatur_main.Controls.Add(this.tabPage_system);
            this.tabControl_Imperatur_main.Controls.Add(this.tabPage_account);
            this.tabControl_Imperatur_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl_Imperatur_main.Location = new System.Drawing.Point(0, 24);
            this.tabControl_Imperatur_main.Name = "tabControl_Imperatur_main";
            this.tabControl_Imperatur_main.SelectedIndex = 0;
            this.tabControl_Imperatur_main.Size = new System.Drawing.Size(1432, 608);
            this.tabControl_Imperatur_main.TabIndex = 0;
            // 
            // tabPage_system
            // 
            this.tabPage_system.Location = new System.Drawing.Point(4, 22);
            this.tabPage_system.Name = "tabPage_system";
            this.tabPage_system.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_system.Size = new System.Drawing.Size(1424, 582);
            this.tabPage_system.TabIndex = 0;
            this.tabPage_system.Text = "System";
            this.tabPage_system.UseVisualStyleBackColor = true;
            // 
            // tabPage_account
            // 
            this.tabPage_account.Location = new System.Drawing.Point(4, 22);
            this.tabPage_account.Name = "tabPage_account";
            this.tabPage_account.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_account.Size = new System.Drawing.Size(1424, 582);
            this.tabPage_account.TabIndex = 1;
            this.tabPage_account.Text = "Account";
            this.tabPage_account.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.archiveToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1432, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // archiveToolStripMenuItem
            // 
            this.archiveToolStripMenuItem.Name = "archiveToolStripMenuItem";
            this.archiveToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.archiveToolStripMenuItem.Text = "Archive";
            // 
            // statusStrip_imperatur
            // 
            this.statusStrip_imperatur.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel_system});
            this.statusStrip_imperatur.Location = new System.Drawing.Point(0, 610);
            this.statusStrip_imperatur.Name = "statusStrip_imperatur";
            this.statusStrip_imperatur.Size = new System.Drawing.Size(1432, 22);
            this.statusStrip_imperatur.TabIndex = 4;
            this.statusStrip_imperatur.Text = "statusStrip_imperatur";
            // 
            // toolStripStatusLabel_system
            // 
            this.toolStripStatusLabel_system.Name = "toolStripStatusLabel_system";
            this.toolStripStatusLabel_system.Size = new System.Drawing.Size(154, 17);
            this.toolStripStatusLabel_system.Text = "toolStripStatusLabel_system";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1432, 632);
            this.Controls.Add(this.statusStrip_imperatur);
            this.Controls.Add(this.tabControl_Imperatur_main);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl_Imperatur_main.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip_imperatur.ResumeLayout(false);
            this.statusStrip_imperatur.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl_Imperatur_main;
        private System.Windows.Forms.TabPage tabPage_system;
        private System.Windows.Forms.TabPage tabPage_account;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem archiveToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip_imperatur;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_system;
    }
}

