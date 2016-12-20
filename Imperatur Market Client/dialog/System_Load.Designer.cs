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
            this.button_browse_System = new System.Windows.Forms.Button();
            this.textBox_system_directory = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button_createnewsystem = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_Load_System
            // 
            this.button_Load_System.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_Load_System.Location = new System.Drawing.Point(316, 170);
            this.button_Load_System.Name = "button_Load_System";
            this.button_Load_System.Size = new System.Drawing.Size(75, 23);
            this.button_Load_System.TabIndex = 0;
            this.button_Load_System.Text = "Load";
            this.button_Load_System.UseVisualStyleBackColor = true;
            this.button_Load_System.Click += new System.EventHandler(this.button_Load_System_Click);
            // 
            // button_cancel
            // 
            this.button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_cancel.Location = new System.Drawing.Point(86, 170);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(75, 23);
            this.button_cancel.TabIndex = 1;
            this.button_cancel.Text = "Cancel";
            this.button_cancel.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button_createnewsystem);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBox_system_directory);
            this.groupBox1.Controls.Add(this.button_Load_System);
            this.groupBox1.Controls.Add(this.button_cancel);
            this.groupBox1.Controls.Add(this.button_browse_System);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(492, 220);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Imperatur Market";
            // 
            // button_browse_System
            // 
            this.button_browse_System.Location = new System.Drawing.Point(387, 40);
            this.button_browse_System.Name = "button_browse_System";
            this.button_browse_System.Size = new System.Drawing.Size(75, 23);
            this.button_browse_System.TabIndex = 0;
            this.button_browse_System.Text = "Browse";
            this.button_browse_System.UseVisualStyleBackColor = true;
            this.button_browse_System.Click += new System.EventHandler(this.button_browse_System_Click);
            // 
            // textBox_system_directory
            // 
            this.textBox_system_directory.Location = new System.Drawing.Point(26, 40);
            this.textBox_system_directory.Name = "textBox_system_directory";
            this.textBox_system_directory.Size = new System.Drawing.Size(338, 20);
            this.textBox_system_directory.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Directory to load system from";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 89);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Or...";
            // 
            // button_createnewsystem
            // 
            this.button_createnewsystem.Location = new System.Drawing.Point(26, 116);
            this.button_createnewsystem.Name = "button_createnewsystem";
            this.button_createnewsystem.Size = new System.Drawing.Size(139, 23);
            this.button_createnewsystem.TabIndex = 4;
            this.button_createnewsystem.Text = "Create new system";
            this.button_createnewsystem.UseVisualStyleBackColor = true;
            this.button_createnewsystem.Click += new System.EventHandler(this.button4_Click);
            // 
            // System_Load
            // 
            this.AcceptButton = this.button_Load_System;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button_cancel;
            this.ClientSize = new System.Drawing.Size(492, 220);
            this.Controls.Add(this.groupBox1);
            this.Name = "System_Load";
            this.Text = "Load or Create system";
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
        private System.Windows.Forms.TextBox textBox_system_directory;
        private System.Windows.Forms.Button button_browse_System;
    }
}