namespace Imperatur_Market_Client.dialog
{
    partial class WaitDialog
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
            this.label_load_status = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label_load_status
            // 
            this.label_load_status.AutoSize = true;
            this.label_load_status.Location = new System.Drawing.Point(12, 20);
            this.label_load_status.Name = "label_load_status";
            this.label_load_status.Size = new System.Drawing.Size(71, 17);
            this.label_load_status.TabIndex = 0;
            this.label_load_status.Text = "Loading...";
            this.label_load_status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // WaitDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(381, 61);
            this.ControlBox = false;
            this.Controls.Add(this.label_load_status);
            this.Name = "WaitDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_load_status;
    }
}