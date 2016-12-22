namespace Imperatur_Market_Client.dialog
{
    partial class System_Load
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
            this.button_Load_System = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBox_SystemDirectory = new System.Windows.Forms.ComboBox();
            this.button_createnewsystem = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button_browse_System = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_Load_System
            // 
            this.button_Load_System.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_Load_System.Location = new System.Drawing.Point(421, 209);
            this.button_Load_System.Margin = new System.Windows.Forms.Padding(4);
            this.button_Load_System.Name = "button_Load_System";
            this.button_Load_System.Size = new System.Drawing.Size(100, 28);
            this.button_Load_System.TabIndex = 0;
            this.button_Load_System.Text = "Load";
            this.button_Load_System.UseVisualStyleBackColor = true;
            this.button_Load_System.Click += new System.EventHandler(this.button_Load_System_Click);
            // 
            // button_cancel
            // 
            this.button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_cancel.Location = new System.Drawing.Point(115, 209);
            this.button_cancel.Margin = new System.Windows.Forms.Padding(4);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(100, 28);
            this.button_cancel.TabIndex = 1;
            this.button_cancel.Text = "Cancel";
            this.button_cancel.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBox_SystemDirectory);
            this.groupBox1.Controls.Add(this.button_createnewsystem);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.button_Load_System);
            this.groupBox1.Controls.Add(this.button_cancel);
            this.groupBox1.Controls.Add(this.button_browse_System);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(656, 271);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Imperatur Market";
            // 
            // comboBox_SystemDirectory
            // 
            this.comboBox_SystemDirectory.FormattingEnabled = true;
            this.comboBox_SystemDirectory.Location = new System.Drawing.Point(34, 49);
            this.comboBox_SystemDirectory.Name = "comboBox_SystemDirectory";
            this.comboBox_SystemDirectory.Size = new System.Drawing.Size(457, 24);
            this.comboBox_SystemDirectory.TabIndex = 5;
            // 
            // button_createnewsystem
            // 
            this.button_createnewsystem.Location = new System.Drawing.Point(35, 143);
            this.button_createnewsystem.Margin = new System.Windows.Forms.Padding(4);
            this.button_createnewsystem.Name = "button_createnewsystem";
            this.button_createnewsystem.Size = new System.Drawing.Size(185, 28);
            this.button_createnewsystem.TabIndex = 4;
            this.button_createnewsystem.Text = "Create new system";
            this.button_createnewsystem.UseVisualStyleBackColor = true;
            this.button_createnewsystem.Click += new System.EventHandler(this.button4_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 110);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Or...";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 20);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(192, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Directory to load system from";
            // 
            // button_browse_System
            // 
            this.button_browse_System.Location = new System.Drawing.Point(516, 49);
            this.button_browse_System.Margin = new System.Windows.Forms.Padding(4);
            this.button_browse_System.Name = "button_browse_System";
            this.button_browse_System.Size = new System.Drawing.Size(100, 28);
            this.button_browse_System.TabIndex = 0;
            this.button_browse_System.Text = "Browse";
            this.button_browse_System.UseVisualStyleBackColor = true;
            this.button_browse_System.Click += new System.EventHandler(this.button_browse_System_Click);
            // 
            // System_Load
            // 
            this.AcceptButton = this.button_Load_System;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button_cancel;
            this.ClientSize = new System.Drawing.Size(656, 271);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "System_Load";
            this.Text = "Load or Create system";
            this.Load += new System.EventHandler(this.System_Load_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_Load_System;
        private System.Windows.Forms.Button button_cancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_createnewsystem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_browse_System;
        private System.Windows.Forms.ComboBox comboBox_SystemDirectory;
    }
}