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
        public System_Load(string[] AutoCompeleteSystemLocation)
        {
            InitializeComponent();
            CreateNewSystem = false;
            SystemLocation = "";
            string LastSystemToLoad = "";
            if (AutoCompeleteSystemLocation.Where(ac=>ac.StartsWith(":def:")).Count() > 0)
            {
                LastSystemToLoad = AutoCompeleteSystemLocation.Where(ac => ac.StartsWith(":def:")).First().Replace(":def:", "");
            }
            string[] CheckedSystemLocations = AutoCompeleteSystemLocation
                .Select(ac=>ac.Replace(":def:", ""))
                .Where(x => Directory.Exists(x).Equals(true)).ToArray();

            if (AutoCompeleteSystemLocation != null && AutoCompeleteSystemLocation.Count() > 0)
            {
                AutoCompleteStringCollection list = new AutoCompleteStringCollection();
                list.AddRange(CheckedSystemLocations);
                comboBox_SystemDirectory.AutoCompleteMode = AutoCompleteMode.Suggest;
                comboBox_SystemDirectory.AutoCompleteSource = AutoCompleteSource.CustomSource;
                comboBox_SystemDirectory.AutoCompleteCustomSource = list;
                comboBox_SystemDirectory.DataSource = CheckedSystemLocations.ToList();
            }
            if (LastSystemToLoad == "" && AutoCompeleteSystemLocation != null && AutoCompeleteSystemLocation.Count() > 1)
            {
                comboBox_SystemDirectory.Text = "";
            }
            else if (LastSystemToLoad != "")
            {
                comboBox_SystemDirectory.Text = LastSystemToLoad;
            }
            comboBox_SystemDirectory.Focus();
        }

     
        private void button_browse_System_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            DialogResult result = fbd.ShowDialog();

            if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                comboBox_SystemDirectory.Text = fbd.SelectedPath;
            }
        }

        private void button_Load_System_Click(object sender, EventArgs e)
        {
            string SuggestedSystemToLoad = comboBox_SystemDirectory.Text;
            if (!Directory.Exists(SuggestedSystemToLoad))
            {
                DialogResult oDr = MessageBox.Show(string.Format(@"Directory {0} does not exists.{1}Would you like to create a new system?", SuggestedSystemToLoad, Environment.NewLine), "Directory does not exists", MessageBoxButtons.YesNo);
                SystemLocation = SuggestedSystemToLoad;
                if (oDr.Equals(DialogResult.No))
                {
                    this.DialogResult = DialogResult.None;
                    return;
                }
                if (oDr.Equals(DialogResult.Yes))
                {
                    CreateNewSystem = true;
                }
            }
            else
            {
                SystemLocation = SuggestedSystemToLoad;
            }
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SystemLocation = comboBox_SystemDirectory.SelectedItem.ToString();
            CreateNewSystem = true;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void System_Load_Load(object sender, EventArgs e)
        {
            comboBox_SystemDirectory.Focus();
        }
    }
}
