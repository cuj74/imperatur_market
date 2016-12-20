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


namespace Imperatur_Market_Client
{
    public partial class MainForm : Form
    {
        public ImperaturMarket m_Ic;
        private string SystemLocation;
        

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ImperaturData oNewSystem = null;
            bool CreateNewSystem = false;
            using (var form = new dialog.System_Load())
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    SystemLocation = form.SystemLocation;
                    CreateNewSystem = form.CreateNewSystem;
                }
            }
            if (CreateNewSystem)
            {
                using (var form = new dialog.NewSystem())
                {
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
            }
            /*
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());

            var FundContainer = kernel.Get<IImperaturMarket>(
                new Ninject.Parameters.ConstructorArgument("SystemLocation", SystemLocation)
                
                new Ninject.Parameters.ConstructorArgument("BFSDataHandler", BFSDataHandler),
                new Ninject.Parameters.ConstructorArgument("SQLConnection", SQLConnection),
                new Ninject.Parameters.ConstructorArgument("DisplayCurrency", DisplayCurrencyCode)
                );
            */
            if (oNewSystem != null)
            {
                m_Ic = (ImperaturMarket)ImperaturContainer.BuildImperaturContainer(oNewSystem);
            }
            else
                m_Ic = (ImperaturMarket)ImperaturContainer.BuildImperaturContainer(SystemLocation);

            //statusStrip_imperatur.

            this.toolStripStatusLabel_system.Text = m_Ic.GetSystemData().SystemDirectory;
            

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
