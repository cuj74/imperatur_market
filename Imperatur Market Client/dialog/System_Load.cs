using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Imperatur_Market_Client.dialog
{
    public partial class System_Load : Form
    {
        public string SystemLocation { get; set; }
        public bool CreateNewSystem { get; set; }
        public System_Load()
        {
            InitializeComponent();
            CreateNewSystem = false;
            SystemLocation = "";
        }

        private void button_browse_System_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            DialogResult result = fbd.ShowDialog();

            if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                textBox_system_directory.Text = fbd.SelectedPath;
            }
        }

        private void button_Load_System_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(textBox_system_directory.Text))
            {
                DialogResult oDr = MessageBox.Show(string.Format(@"Directory {0} does not exists.{1}Would you like to create a new system?", textBox_system_directory.Text, Environment.NewLine), "Directory does not exists", MessageBoxButtons.YesNo);
                SystemLocation = textBox_system_directory.Text;
                if (oDr.Equals(DialogResult.No))
                {
                    this.Close();
                }
                if (oDr.Equals(DialogResult.Yes))
                {
                    CreateNewSystem = true;
                }
            }
            else
            {
                SystemLocation = textBox_system_directory.Text;
            }
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SystemLocation = textBox_system_directory.Text;
            CreateNewSystem = true;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
