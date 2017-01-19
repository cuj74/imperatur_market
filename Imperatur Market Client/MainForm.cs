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
using Imperatur_v2.shared;
using System.IO;
using Imperatur_v2.account;
using Imperatur_v2.monetary;

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
        public delegate void SelectedSymbolEventHandler(object sender, SelectedSymbolEventArg e);
        private Timer UpdateLatestTransactions;


        private AccountTab m_oAccountTab;
        delegate void RefreshQuotes();

        public MainForm()
        {
            InitializeComponent();
            //typeof(TableLayoutPanel).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(tlp_Account, true, null);
            m_oKernel = new StandardKernel();
            m_oKernel.Load(Assembly.GetExecutingAssembly());
            UpdateLatestTransactions = new Timer();
            UpdateLatestTransactions.Interval = 1000 * 60; //every minute
            UpdateLatestTransactions.Tick += UpdateLatestTransactions_Tick;
            UpdateLatestTransactions.Enabled = true;

        }

        private void UpdateLatestTransactions_Tick(object sender, EventArgs e)
        {
            DataGridView TransactionGrid = new DataGridView();

            TransactionGrid.AutoGenerateColumns = false;
            TransactionGrid.AllowUserToAddRows = false;

            TransactionGrid.Columns.Add(
                new DataGridViewTextBoxColumn()
                {
                    CellTemplate = new DataGridViewTextBoxCell(),
                    Name = "Amount",
                    HeaderText = "Amount",
                    DataPropertyName = "Amount",
                    ReadOnly = true
                }
            );
            TransactionGrid.Columns.Add(
                new DataGridViewTextBoxColumn()
                {
                    CellTemplate = new DataGridViewTextBoxCell(),
                    Name = "TransactionDate",
                    HeaderText = "Date",
                    DataPropertyName = "TransactionDate",
                    ReadOnly = true
                }
            );
            TransactionGrid.Columns.Add(
                new DataGridViewTextBoxColumn()
                {
                    CellTemplate = new DataGridViewTextBoxCell(),
                    Name = "TransactionType",
                    HeaderText = "Type",
                    DataPropertyName = "TransactionType",
                    ReadOnly = true
                }
            );
            TransactionGrid.Columns.Add(
                new DataGridViewTextBoxColumn()
                {
                    CellTemplate = new DataGridViewTextBoxCell(),
                    Name = "Symbol",
                    HeaderText = "Symbol",
                    DataPropertyName = "Symbol",
                    ReadOnly = true
                }
            );
            TransactionGrid.Columns.Add(
                new DataGridViewTextBoxColumn()
                {
                    CellTemplate = new DataGridViewTextBoxCell(),
                    Name = "Revenue",
                    HeaderText = "Revenue",
                    DataPropertyName = "Revenue",
                    ReadOnly = true
                }
            );

            TransactionGrid.Columns.Add(
                new DataGridViewTextBoxColumn()
                {
                    CellTemplate = new DataGridViewTextBoxCell(),
                    Name = "Tradeprice",
                    HeaderText = "Tradeprice",
                    DataPropertyName = "Tradeprice",
                    ReadOnly = true
                }
            );

            TransactionGrid.Columns.Add(
            new DataGridViewTextBoxColumn()
            {
                CellTemplate = new DataGridViewTextBoxCell(),
                Name = "Quantity",
                HeaderText = "Quantity",
                DataPropertyName = "Quantity",
                ReadOnly = true
            }
            );

            DataTable TransactionsDT = new DataTable();
            TransactionsDT.Columns.Add("Amount");
            TransactionsDT.Columns.Add("TransactionDate");
            TransactionsDT.Columns.Add("TransactionType");
            TransactionsDT.Columns.Add("Symbol");
            TransactionsDT.Columns.Add("Revenue");
            TransactionsDT.Columns.Add("Tradeprice");
            TransactionsDT.Columns.Add("Quantity");

            DataRow row = null;
            /*
            List<TransactionType> BuySell = new List<TransactionType>();
            BuySell.Add(TransactionType.Buy);
            BuySell.Add(TransactionType.Sell);

            var SumTrans = from a in m_Ic.GetAccountHandler().Accounts()
                           select new
                                     {
                                         account = a.AccountName,
                                         transactions = a.Transactions
                                     };


            //get all trade
            var LatestTransactions =
            from t in SumTrans
            join bs in BuySell on t.transactions. equals bs
            select t;
            */


            foreach (ITransactionInterface oT in m_Ic.GetAccountHandler().Accounts().SelectMany(t => t.Transactions).Where(t=>!t.TransactionType.Equals(TransactionType.Transfer)).OrderByDescending(t => t.TransactionDate).Take(20))
            {
                row = TransactionsDT.NewRow();
                row["Amount"] = oT.DebitAmount.ToString();
                row["TransactionDate"] = oT.TransactionDate;
                row["TransactionType"] = oT.TransactionType.ToString();
                row["Symbol"] = oT.SecuritiesTrade != null ? oT.SecuritiesTrade.Security.Symbol : "";
                row["Revenue"] = oT.SecuritiesTrade != null && oT.SecuritiesTrade.Revenue != null ? oT.SecuritiesTrade.Revenue.ToString() : "";
                row["Tradeprice"] = oT.SecuritiesTrade != null ? oT.SecuritiesTrade.AverageAcquisitionValue.ToString() : "";
                row["Quantity"] = oT.SecuritiesTrade != null ? oT.SecuritiesTrade.Quantity.ToString() : "";
                
                TransactionsDT.Rows.Add(row);

            }
            TransactionGrid.DataSource = TransactionsDT;
            TransactionGrid.Dock = DockStyle.Fill;
            TransactionGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;

            UserControl TransactionControl = new CreateDataGridControlFromObject(
            new DataGridForControl
            {
                DataGridViewToBuild = TransactionGrid,
                GroupBoxCaption = "Latest transactions"
            }
            );
            LatestTransactionsPane.Controls.Clear();
            LatestTransactionsPane.Controls.Add(TransactionControl);
        }

        //public object resources { get; private set; }

        private bool ShowSystemLoad()
        {
            bool CreateNewSystem = false;
            //bool bShowAgain = false;
            using (var form = new dialog.System_Load(ReadSystemLocationFromCache()))
            {
                form.Icon = this.Icon;
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    SystemLocation = form.SystemLocation;
                    CreateNewSystem = form.CreateNewSystem;
                }
                if (result == DialogResult.None)
                    return ShowSystemLoad();
            }

            return CreateNewSystem;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ImperaturData oNewSystem = null;
            bool CreateNewSystem = ShowSystemLoad();

            dialog.WaitDialog oW = new dialog.WaitDialog();
            oW.Show();
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
            oW.Close();
            //this far we save the systemlocation to the clients application folder for easy access
            //SystemLocationCacheFile
            SaveSystemLocationToCache(oNewSystem);

            this.toolStripStatusLabel_system.Text =
                string.Format("{0} | {1} | {2}", m_Ic.GetSystemData().SystemDirectory, m_Ic.GetSystemData().SystemCurrency, m_Ic.SystemExchangeStatus.ToString());
                

            //StandardKernel kernel = new StandardKernel();
            //kernel.Load(Assembly.GetExecutingAssembly());


            //CreateTestData();

            //add the controls to the different areas
            m_oAccountTab = new AccountTab(m_Ic.GetAccountHandler(), m_Ic.GetTradeHandler());
            m_oAccountTab.Dock = DockStyle.Fill;
            tabPage_account.Controls.Add(m_oAccountTab);
            m_Ic.QuoteUpdateEvent += M_Ic_QuoteUpdateEvent;
            this.checkBox_automaticTrading.Checked = m_Ic.GetSystemData().IsAutomaticMaintained;

    }

        private void M_Ic_QuoteUpdateEvent(object sender, EventArgs e)
        {
            
            this.toolStripStatusLabel_system.Text =
               string.Format("{0} | {1} | {2} | last update {3}", m_Ic.GetSystemData().SystemDirectory, m_Ic.GetSystemData().SystemCurrency,m_Ic.SystemExchangeStatus.ToString(), DateTime.Now.ToString());
            //TODO: needs invoke
            /*
            if (m_oAccountTab.InvokeRequired)
            {
                RefreshQuotes d = new RefreshQuotes();
                this.Invoke(d, new object[] { text });
            }
            this.Invoke(m_oAccountTab.RefreshSelectedAccountData());
            */

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
                        if (prop.FieldType.Equals(typeof(string))) {
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

                
                try
                {
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
                catch (Exception ex)
                {
                    int gg = 0;
                }
            }

            return oNewSystem;
        }
        
        private void CreateTestData()
        {
            //List<IAccountInterface> oLA = new List<Imperatur_v2.account.IAccountInterface>();
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
                "Grizelda"
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
            //Account oA;
            for (int i = 0; i < CustomersFirstName.Count()-1; i++)
            {


                oC = new Imperatur_v2.customer.Customer();
                oC.FirstName = CustomersFirstName[i];
                oC.LastName = CustomersLastName[CustomersFirstName.Count() - 1-i];
                oC.Idnumber = "7405255915";
                m_Ic.GetAccountHandler().CreateAccount(oC, Imperatur_v2.account.AccountType.Customer, string.Format("{0} konto", oC.FirstName));

              
            }

            foreach (IAccountInterface oAdeposit in m_Ic.GetAccountHandler().Accounts().Where(a=>a.GetAccountType().Equals(AccountType.Customer)))
            {
                m_Ic.GetAccountHandler().DepositAmount(oAdeposit.Identifier,
                   m_Ic.GetMoney(200000, "SEK"));
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
            string DefaultMarker = ":def:";
            string SystemLocationCacheFilePath = AppDomain.CurrentDomain.BaseDirectory + @"\" + this.SystemLocationCacheFile;
            string PossibleNewSystem = (oNewSystem != null && oNewSystem.SystemDirectory != null && oNewSystem.SystemDirectory != "") ?
                    oNewSystem.SystemDirectory
                    : SystemLocation;
            if (!File.Exists(SystemLocationCacheFilePath))
            {
                try
                {
                    File.AppendAllText(SystemLocationCacheFilePath, DefaultMarker + PossibleNewSystem + Environment.NewLine);
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
                    string[] SystemLocationsFromFile = File.ReadAllLines(SystemLocationCacheFilePath);

                    //system exists without default marker
                    if (Array.Find(SystemLocationsFromFile, element => element.Equals(PossibleNewSystem)) != null)
                    {
                        //add the defaultmarker and save
                        //first remove file
                        File.Delete(SystemLocationCacheFilePath);
                        File.AppendAllText(SystemLocationCacheFilePath, string.Join(Environment.NewLine, SystemLocationsFromFile
                            .Where(element => !element.Equals(PossibleNewSystem)).ToArray()) + Environment.NewLine);
                        File.AppendAllText(SystemLocationCacheFilePath, DefaultMarker + PossibleNewSystem + Environment.NewLine);
                        return;
                    }
                    //system does not exists with default marker
                    else if (Array.Find(SystemLocationsFromFile, element => element.Equals(DefaultMarker + PossibleNewSystem)) == null)
                    {
                        File.AppendAllText(SystemLocationCacheFilePath, PossibleNewSystem + Environment.NewLine);
                    }

                        
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
                if (prop.Name != "" && prop.FieldType.Equals(typeof(string)))
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
                ULR_Quotes = SystemData.FirstOrDefault(t => t.Key == "ULR_Quotes").Value,
                QuoteRefreshTime = SystemData.FirstOrDefault(t => t.Key == "QuoteRefreshTime").Value,
                DailyQuoteDirectory = SystemData.FirstOrDefault(t => t.Key == "DailyQuoteDirectory").Value,
                Exchange = SystemData.FirstOrDefault(t => t.Key == "Exchange").Value,
                HistoricalQuoteDirectory = SystemData.FirstOrDefault(t => t.Key == "HistoricalQuoteDirectory").Value,
                HistoricalQuoteFile = SystemData.FirstOrDefault(t => t.Key == "HistoricalQuoteFile").Value

            };
            //all fields must be filled in!
            bool AllFieldsOk = true; 
            foreach (var prop in typeof(ImperaturData).GetFields())
            {
                if (prop.Name != "" && prop.FieldType.Equals(typeof(string))
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

        private void checkBox_automaticTrading_CheckedChanged(object sender, EventArgs e)
        {
            m_Ic.SetAutomaticTrading(checkBox_automaticTrading.Checked);
            if (checkBox_automaticTrading.Checked)
            {
                if (m_Ic.GetAccountHandler().Accounts().Where(a=>a.GetAccountType().Equals(AccountType.Customer)).Count() == 0)
                {
                    if (MessageBox.Show("Create test accounts?", "Create test accounts?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        CreateTestData();
                    }
                }
            }
        }
    }
}
