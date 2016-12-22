using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Imperatur_v2;
using Ninject;
using System.Reflection;
using Imperatur_Market_Client.control;
using Imperatur_v2.events;
using Imperatur_Market_Client.events;

using System.IO;

namespace Imperatur_Market_Client
{
    public partial class MainForm : Form
    {
        public IImperaturMarket m_Ic;
        private string SystemLocation;
        StandardKernel m_oKernel;
        private readonly string SystemLocationCacheFile = "systemlocation.imp";
        public delegate void SelectedAccountEventHandler(object sender, SelectedAccountEventArg e);
        public delegate void ToggleSearchDialogHandler(object sender, ToggleSearchEvents e);

        public MainForm()
        {
            InitializeComponent();
            //typeof(TableLayoutPanel).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(tlp_Account, true, null);
            m_oKernel = new StandardKernel();
            m_oKernel.Load(Assembly.GetExecutingAssembly());
        }

        public object resources { get; private set; }

        private void Form1_Load(object sender, EventArgs e)
        {
            ImperaturData oNewSystem = null;
            bool CreateNewSystem = false;
            using (var form = new dialog.System_Load(ReadSystemLocationFromCache()))
            {
                form.Icon = this.Icon;
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    SystemLocation = form.SystemLocation;
                    CreateNewSystem = form.CreateNewSystem;
                }
            }
            if (CreateNewSystem)
            {
                oNewSystem = CreateNewImperaturMarket(oNewSystem);
            }

            if (oNewSystem != null)
            {
                m_Ic = (ImperaturMarket)ImperaturContainer.BuildImperaturContainer(oNewSystem);
            }
            else
                m_Ic = (ImperaturMarket)ImperaturContainer.BuildImperaturContainer(SystemLocation);

            //this far we save the systemlocation to the clients application folder for easy access
            //SystemLocationCacheFile
            SaveSystemLocationToCache(oNewSystem);

            this.toolStripStatusLabel_system.Text = m_Ic.GetSystemData().SystemDirectory;

            //StandardKernel kernel = new StandardKernel();
            //kernel.Load(Assembly.GetExecutingAssembly());


            CreateTestData();

            //add the controls to the different areas
            AccountTab oAccountTab = new AccountTab(m_Ic.GetAccountHandler());
            oAccountTab.Dock = DockStyle.Fill;
            tabPage_account.Controls.Add(oAccountTab);




    }


    private ImperaturData CreateNewImperaturMarket(ImperaturData oNewSystem)
        {
            using (var form = new dialog.NewSystem())
            {
                form.Icon = this.Icon;
                GroupBox oB = new GroupBox()
                {
                    Dock = DockStyle.Fill,
                    Text = "Create new System",
                    Name = "gp",
                    Visible = true,
                    Padding = new Padding()
                    {
                        All = 20
                    },
                    Margin = new Padding()
                    {
                        All = 20
                    },
                    Bounds = form.Bounds
                };

                TableLayoutPanel tlp = new TableLayoutPanel
                {
                    Name = "tlp",
                    Dock = DockStyle.Fill,
                    RowCount = typeof(ImperaturData).GetFields().Count() + 1,
                    ColumnCount = 2,
                    Visible = true,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,

                };

                int indexcount = 0;
                foreach (var prop in typeof(ImperaturData).GetFields())
                {
                    if (prop.Name != "")
                    {
                        string Value = prop.Name.Equals("SystemDirectory") ? SystemLocation : typeof(ImperaturDataStandard).GetFields().FirstOrDefault(t => t.Name == prop.Name).GetValue(null).ToString();
                        tlp.Controls.Add(new TextBox()
                        {
                            Text =
                            Value
                            ,
                            Name = prop.Name,
                            Anchor = AnchorStyles.Left,
                            Width = 300,
                            AutoSize = true,
                            ReadOnly = prop.Name.Equals("SystemDirectory") && SystemLocation.Length > 0 ? true : false
                        }, 1, indexcount);
                        tlp.Controls.Add(new Label() { Text = prop.Name, Anchor = AnchorStyles.Left, AutoSize = true }, 0, indexcount);
                    }
                    indexcount++;
                }

                Button oButtonOk = new Button()
                {
                    Name = "buttonok",
                    Text = "OK",
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    DialogResult = DialogResult.OK
                };

                Button oButtonCancel = new Button()
                {
                    Name = "buttoncancel",
                    Text = "Cancel",
                    AutoSizeMode = AutoSizeMode.GrowAndShrink
                };

                oButtonOk.Click += OButtonOk_Click;
                oButtonCancel.Click += OButtonCancel_Click;

                tlp.Controls.Add(oButtonOk, 0, indexcount);
                tlp.Controls.Add(oButtonCancel, 1, indexcount);

                oB.Controls.Add(tlp);
                form.Controls.Add(oB);

                form.Height = tlp.Height;


                var result = form.ShowDialog();
                while (!result.Equals(DialogResult.OK) && !result.Equals(DialogResult.Cancel))
                {

                }
                if (result == DialogResult.OK)
                {
                    oNewSystem = form.SystemData;
                }
                else
                {
                    this.Close();
                }
            }

            return oNewSystem;
        }

        private void CreateTestData()
        {
            List<Imperatur_v2.account.IAccountInterface> oLA = new List<Imperatur_v2.account.IAccountInterface>();
            string[] CustomersFirstName = new string[] {
                "Jenny",
                "Zeinab",
                "Edvin",
                "Jan Mattias Hasse",
                "Knut Torsten",
                "Henrik",
                "Anna Maja Margareta",
                "steffe",
                "Lars Torbjörn",
                "emmy",
                "Tomi",
                "Magnus",
                "Raad",
                "Mathias",
                "Sandra",
                "Simon",
                "Anna",
                "Niklas",
                "Jörgen",
                "Annika",
                "Johanna",
                "Eva Charlotte Josefine",
                "Sofia",
                "Linda",
                "Joel",
                "Sari Tuulikki",
                "Emma",
                "Patrick",
                "grizelda"
            };

            string[] CustomersLastName = new string[] {
                "Boudaeva",
                "Wallin",
                "Hedman",
                "Rådström",
                "Olsson",
                "Pusvaskis",
                "Chavez Jara",
                "Fredriksson",
                "Lehtonen Svensson",
                "Lundgren",
                "Fagervall",
                "Falk",
                "Hedlund",
                "Lund",
                "Pettersson",
                "Svensson",
                "Sjögren",
                "Blad",
                "Sanderholm",
                "Nyback",
                "Koskei",
                "Fridlund",
                "Nilsson",
                "Karlsson",
                "Burman",
                "Gradin",
                "Olsson",
                "Pettersson",
                "Berglönn"

            };

            Imperatur_v2.customer.Customer oC;
            Imperatur_v2.account.Account oA;
            for (int i = 0; i < CustomersFirstName.Count()-1; i++)
            {
                oC = new Imperatur_v2.customer.Customer();
                oC.FirstName = CustomersFirstName[i];
                oC.LastName = CustomersLastName[1];
                oC.Idnumber = "7405255915";
                oA = new Imperatur_v2.account.Account(oC, Imperatur_v2.account.AccountType.Customer, string.Format("{0} konto", oC.FirstName));
                oLA.Add((Imperatur_v2.account.IAccountInterface)oA);
            }

            try
            {
                m_Ic.GetAccountHandler().CreateAccount(oLA);
            }
            catch (Exception ex)
            {
                string ff = "";
            }
        }

        private string[] ReadSystemLocationFromCache()
        {
            string SystemLocationCacheFilePath = AppDomain.CurrentDomain.BaseDirectory + @"\" + this.SystemLocationCacheFile;
            if (File.Exists(SystemLocationCacheFilePath))
            {
                return File.ReadAllLines(SystemLocationCacheFilePath);
            }
            return null;
        }

        private void SaveSystemLocationToCache(ImperaturData oNewSystem)
        {
            string SystemLocationCacheFilePath = AppDomain.CurrentDomain.BaseDirectory + @"\" + this.SystemLocationCacheFile;
            string PossibleNewSystem = (oNewSystem != null && oNewSystem.SystemDirectory != null && oNewSystem.SystemDirectory != "") ?
                    oNewSystem.SystemDirectory
                    : SystemLocation;
            if (!File.Exists(SystemLocationCacheFilePath))
            {
                try
                {
                    File.AppendAllText(SystemLocationCacheFilePath, PossibleNewSystem + Environment.NewLine);
                }
                catch(Exception ex)
                {
                    int gg = 0;
                }

            }
            else
            {
                //is the directory already in the cache?
                try { 
                    if (Array.Find(File.ReadAllLines(SystemLocationCacheFilePath), element => element.Equals(PossibleNewSystem)) != null)
                        return;
                    else
                        File.AppendAllText(SystemLocationCacheFilePath, PossibleNewSystem + Environment.NewLine);
                }
                catch(Exception ex)
                {
                    int gg2= 0;
                }
            }
        }

       
        private void OButtonCancel_Click(object sender, EventArgs e)
        {
            Button ob = (Button)sender;
            dialog.NewSystem oNewSystem = (dialog.NewSystem)ob.Parent.Parent.Parent;
            oNewSystem.Close();

        }

        private void OButtonOk_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> SystemData = new Dictionary<string, string>();
            Button ob = (Button)sender;
            foreach (var prop in typeof(ImperaturData).GetFields())
            {
                if (prop.Name != "")
                {
                    SystemData.Add(prop.Name, ((TextBox)ob.Parent.Controls[prop.Name]).Text);

                }
            }
            dialog.NewSystem oNewSystem = (dialog.NewSystem)ob.Parent.Parent.Parent;
            oNewSystem.SystemData = new ImperaturData
            {
                AcccountDirectory = SystemData.FirstOrDefault(t => t.Key == "AcccountDirectory").Value,
                CustomerDirectory = SystemData.FirstOrDefault(t => t.Key == "CustomerDirectory").Value,
                AccountFile = SystemData.FirstOrDefault(t => t.Key == "AccountFile").Value,
                CustomerFile = SystemData.FirstOrDefault(t => t.Key == "CustomerFile").Value,
                QuoteDirectory = SystemData.FirstOrDefault(t => t.Key == "QuoteDirectory").Value,
                QuoteFile = SystemData.FirstOrDefault(t => t.Key == "QuoteFile").Value,
                SystemCurrency = SystemData.FirstOrDefault(t => t.Key == "SystemCurrency").Value,
                SystemDirectory = SystemData.FirstOrDefault(t => t.Key == "SystemDirectory").Value,
                ULR_Quotes = SystemData.FirstOrDefault(t => t.Key == "ULR_Quotes").Value

            };
            //all fields must be filled in!
            bool AllFieldsOk = true; 
            foreach (var prop in typeof(ImperaturData).GetFields())
            {
                if (prop.Name != "" 
                    &&
                    (
                    typeof(ImperaturData).GetFields().FirstOrDefault(t => t.Name == prop.Name).GetValue(oNewSystem.SystemData) != null
                    &&
                    typeof(ImperaturData).GetFields().FirstOrDefault(t => t.Name == prop.Name).GetValue(oNewSystem.SystemData).ToString().Trim().Length == 0
                    )
                    ||
                    typeof(ImperaturData).GetFields().FirstOrDefault(t => t.Name == prop.Name).GetValue(oNewSystem.SystemData) == null
                    )
                {
                    MessageBox.Show("All fields must be filled!");
                    AllFieldsOk = false;
                    break;
                }
            }
            if (AllFieldsOk)
            {
                oNewSystem.DialogResult = DialogResult.OK;
                oNewSystem.Close();
            }
            else
                oNewSystem.DialogResult = DialogResult.None;
        }
    }
}
