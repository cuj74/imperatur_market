using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Imperatur_v2.handler;
using Imperatur_v2;
using Imperatur_v2.account;
using Imperatur_v2.events;
using Imperatur_v2.shared;
using Imperatur_v2.trade;
using Imperatur_Market_Client.events;
using Imperatur_v2.monetary;
using System.Net;
using Imperatur_v2.trade.analysis;
using Imperatur_v2.securites;
using ZedGraph;

namespace Imperatur_Market_Client.control
{
    public partial class Account_Trade : UserControl
    {
        public event MainForm.SelectedAccountEventHandler SelectedAccount;
        private IAccountHandlerInterface m_oAh;
        private IAccountInterface m_oAccountData;
        private ITradeHandlerInterface m_oTradeHandler;
        private Imperatur_v2.trade.analysis.SecurityAnalysis m_oS;
        private int m_oGraphSettingDays;
        private Instrument m_oI;
        private bool ShowMovingAverage = false;

        public Account_Trade(IAccountHandlerInterface AccountHandler, ITradeHandlerInterface TradeHandler)
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            m_oAh = AccountHandler;
            m_oTradeHandler = TradeHandler;
            //comboBox_Symbols
            AutoCompleteStringCollection list = new AutoCompleteStringCollection();
            list.AddRange(ImperaturGlobal.Instruments.Select(i => i.Symbol).ToArray());
            comboBox_Symbols.AutoCompleteMode = AutoCompleteMode.Suggest;
            comboBox_Symbols.AutoCompleteSource = AutoCompleteSource.CustomSource;
            comboBox_Symbols.AutoCompleteCustomSource = list;
            comboBox_Symbols.DataSource = ImperaturGlobal.Instruments.Select(i => i.Symbol).ToList();
            comboBox_Symbols.Text = "";
            comboBox_Symbols.SelectedIndexChanged += ComboBox_Symbols_SelectedIndexChanged;
            this.textBox_Quantity.KeyDown += TextBox_Quantity_KeyDown;
            tableLayoutPanel_Graph.Visible = false;
            m_oGraphSettingDays = 7;
        }

        private void TextBox_Quantity_KeyDown(object sender, KeyEventArgs e)
        {
            int QuantityToBuy;
            bool isNumeric = int.TryParse(textBox_Quantity.Text.Trim(), out QuantityToBuy);
            if (m_oAccountData != null && isNumeric && comboBox_Symbols.SelectedItem != null && comboBox_Symbols.SelectedItem.ToString().Length > 0)
            {

                IMoney MarketValue = ImperaturGlobal.Quotes.Where(q => q.Symbol.Equals(comboBox_Symbols.SelectedItem.ToString())).First().LastTradePrice.Multiply(Convert.ToDecimal(QuantityToBuy));
                if (
                    MarketValue.Amount >
                    m_oAccountData.GetAvailableFunds().Where(m => m.CurrencyCode.Equals(MarketValue.CurrencyCode)).First().Amount
                    )
                {
                    button_BuySecurity.Enabled = false;
                    return;
                }
                else
                    button_BuySecurity.Enabled = true;
            }

            if (e.KeyCode == Keys.Enter)
            {
                button_BuySecurity_Click(this, null);
            }

        }

        protected virtual void OnSelectedAccount(SelectedAccountEventArg e)
        {
            if (SelectedAccount != null)
                SelectedAccount(this, e);
        }

        public void SetSelectedSymbol(string Symbol)
        {
            comboBox_Symbols.SelectedItem = Symbol;
        }


        private void ComboBox_Symbols_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox_Symbols.SelectedItem != null)
            {
                if (ImperaturGlobal.Quotes.Where(q => q.Symbol.Equals(comboBox_Symbols.SelectedItem.ToString())).Count() > 0)
                {
                    label_instrument_info.Text = ImperaturGlobal.Quotes.Where(q => q.Symbol.Equals(comboBox_Symbols.SelectedItem.ToString())).First().LastTradePrice.ToString() + " " + ImperaturGlobal.Instruments.Where(i => i.Symbol.Equals(comboBox_Symbols.SelectedItem.ToString())).First().Name;
                    /*using (WebClient webClient = new WebClient())
                    {
                        //webClient.DownloadFile("https://www.google.com/finance/getchart?q=" + comboBox_Symbols.SelectedItem.ToString(), "image.png");
                        pictureBox_graph.Load("https://www.google.com/finance/getchart?q=" + comboBox_Symbols.SelectedItem.ToString().Replace(" ", "-"));
                        pictureBox_graph.SizeMode = PictureBoxSizeMode.StretchImage;
                    }*/
                    m_oI = ImperaturGlobal.Instruments.Where(i => i.Symbol.Equals(comboBox_Symbols.SelectedItem.ToString())).First();
                    m_oS = new Imperatur_v2.trade.analysis.SecurityAnalysis(m_oI); 
                    label3.Text = m_oS.StandardDeviationForRange(DateTime.Now.AddDays(-7), DateTime.Now).ToString();
                    label3.Text += " | " + m_oS.StandardDeviation.ToString();
                    ChangeGraph(m_oGraphSettingDays);

                }
                else
                    label_instrument_info.Text = "N/A";
            }
        }

        private void ChangeGraph(int days, bool AddMovingAverage = false)
        {
            if (m_oS != null)
            {
                List<HistoricalQuoteDetails> oH = m_oS.GetDataForRange(DateTime.Now.AddDays(-days), DateTime.Now);
                string[] xData = days > 1 ?
                    oH.Select(h => h.Date.ToShortDateString()).ToArray()
                    :
                    oH.Select(h => h.Date.ToShortTimeString()).ToArray();

                

                CreateGraph(oH.Select(h => Convert.ToDouble(h.Close)).ToArray(), xData, (days == 1), AddMovingAverage ? m_oS.MovingAverageForRange(DateTime.Now.AddDays(-days), DateTime.Now).ToArray() : new double[0] { });
            }
        }

        private void CreateGraph(double[] yData, string[] xData, bool ShowTimeInX, double[] movingaverage_yData)
        {
            string ButtonName = "b_daterange0";
            if (!tableLayoutPanel_Graph.Controls.ContainsKey(ButtonName))
            {
                List<string> ButtonControl = new List<string>
                {
                    "1d","3d", "1w", "1m", "1y"
                };

                for (int i = 0; i < 5; i++)
                {
                    Button b = new Button();
                    b.Name = ButtonName.Replace("0", i.ToString());
                    b.Text = ButtonControl[i];
                    b.Click += BChartChange_Click;
                    this.tableLayoutPanel_Graph.Controls.Add(b, i, 1);
                }
                //add moving average
                Button bma = new Button();
                bma.Name = "b_movingaverage";
                bma.Text = "MA";
                bma.Click += BChartChange_Click;
                this.tableLayoutPanel_Graph.Controls.Add(bma, 5, 1);
            }

            ZedGraphControl oZGP = new ZedGraphControl();
            //generate pane
            var pane = oZGP.GraphPane;
            pane.Title.Text = string.Format("{0} ({1})", m_oI.Name, m_oI.Symbol);
            pane.YAxis.Title.Text = m_oI.CurrencyCode.ToString();

            pane.XAxis.Scale.IsVisible = true;
            pane.YAxis.Scale.IsVisible = true;

            pane.XAxis.MajorGrid.IsVisible = false;
            pane.YAxis.MajorGrid.IsVisible = false;

            pane.XAxis.Scale.TextLabels = xData;
            if (ShowTimeInX)
                pane.XAxis.Type = AxisType.Text;
            else
                pane.XAxis.Type = AxisType.Date;

            pane.YAxis.Title.Text = "Date";


            LineItem pointsCurve = pane.AddCurve("", null, yData, Color.Black);
            pointsCurve.Line.IsVisible = true;
            pointsCurve.Line.Width = 0.5F;
            
            pointsCurve.Symbol.Fill = new Fill(new Color[] { Color.Blue, Color.Green, Color.Red });
            pointsCurve.Symbol.Fill.Type = FillType.Solid;
            
            pointsCurve.Symbol.Type = SymbolType.Circle;
            pointsCurve.Symbol.Size = 1.0F;
            pointsCurve.Symbol.Border.IsVisible = false;

            if (movingaverage_yData.Count() > 0)
            {
                LineItem maCurve = pane.AddCurve("", null, movingaverage_yData, Color.PaleVioletRed);
                maCurve.Line.IsVisible = true;
                maCurve.Line.Width = 0.5F;

                maCurve.Symbol.Fill = new Fill(new Color[] { Color.Blue, Color.Green, Color.Red });
                maCurve.Symbol.Fill.Type = FillType.Solid;
  
                maCurve.Symbol.Type = SymbolType.Circle;
                maCurve.Symbol.Size = 1.0F;
                maCurve.Symbol.Border.IsVisible = false;

                //band upper
                double[] upper = movingaverage_yData.ToList().Select(x => Convert.ToDouble(m_oS.StandardDeviation) + x).ToArray();
                LineItem 
                maCurveUpper = pane.AddCurve("", null, upper, Color.AliceBlue);
                maCurveUpper.Line.IsVisible = true;
                maCurveUpper.Line.Width = 0.5F;
                maCurveUpper.Symbol.Fill = new Fill(new Color[] { Color.Blue, Color.Green, Color.Red });
                maCurveUpper.Symbol.Fill.Type = FillType.Solid;
                maCurveUpper.Symbol.Type = SymbolType.Circle;
                maCurveUpper.Symbol.Size = 1.0F;
                maCurveUpper.Symbol.Border.IsVisible = false;
            }


            

            pane.AxisChange();
            oZGP.Refresh();
            oZGP.Name = "StockChart";
            oZGP.Dock = DockStyle.Fill;





            if (!panel_chart.Controls.ContainsKey(oZGP.Name))
            {
                panel_chart.Controls.Add(oZGP);
            }
            else
            {
                panel_chart.Controls.RemoveByKey(oZGP.Name);
                panel_chart.Controls.Add(oZGP);
            }

            tableLayoutPanel_Graph.Visible = true;
        }

        private void BChartChange_Click(object sender, EventArgs e)
        {
            //"1d","3d", "1w", "1m", "1y"
            Button ob = (Button)sender;
            if (ob.Name == "b_movingaverage")
            {
                if (ShowMovingAverage)
                {
                    ShowMovingAverage = false;
                }
                else
                    ShowMovingAverage = true;

                ChangeGraph(m_oGraphSettingDays, ShowMovingAverage);
                return;
            }

            switch (ob.Text)
            {
                case "1d":
                    m_oGraphSettingDays = 1;
                    ChangeGraph(1, ShowMovingAverage);
                    break;
                case "3d":
                    m_oGraphSettingDays = 3;
                    ChangeGraph(3, ShowMovingAverage);
                    break;
                case "1w":
                    m_oGraphSettingDays = 7;
                    ChangeGraph(7, ShowMovingAverage);
                    break;
                case "1m":
                    m_oGraphSettingDays = 30;
                    ChangeGraph(30, ShowMovingAverage);
                    break;
                case "1y":
                    m_oGraphSettingDays = 360;
                    ChangeGraph(360, ShowMovingAverage);
                    break;
                default:
                    break;
            }
        }

        public void UpdateAccountInfo(IAccountInterface AccountData)
        {
            m_oAccountData = AccountData;
            var newSymbolsToShow = from i in ImperaturGlobal.Instruments
                                   join a in m_oAccountData.GetAvailableFunds() on i.CurrencyCode equals a.CurrencyCode.CurrencyCode
                                   select i.Symbol;
            comboBox_Symbols.DataSource = newSymbolsToShow.ToList();
            comboBox_Symbols.Refresh();
        }

        private void button_BuySecurity_Click(object sender, EventArgs e)
        {
            int QuantityToBuy;
            bool isNumeric = int.TryParse(textBox_Quantity.Text.Trim(), out QuantityToBuy);
            if (m_oAccountData != null && isNumeric && comboBox_Symbols.SelectedItem != null && comboBox_Symbols.SelectedItem.ToString().Length > 0)
            {
                /*
                IMoney MarketValue = ImperaturGlobal.Quotes.Where(q => q.Symbol.Equals(comboBox_Symbols.SelectedItem.ToString())).First().LastTradePrice.Multiply(Convert.ToDecimal(QuantityToBuy));
                if (
                    MarketValue.Amount >
                    m_oAccountData.GetAvailableFunds().Where(m=>m.CurrencyCode.Equals(MarketValue.CurrencyCode)).First().Amount
                    )
                {
                    MessageBox.Show(string.Format
                }*/
                string sMessage = string.Format("Are you sure you want to buy {0} of {1} for {2}?",
                    QuantityToBuy,
                    comboBox_Symbols.SelectedItem.ToString(),
                    ImperaturGlobal.Quotes.Where(q => q.Symbol.Equals(comboBox_Symbols.SelectedItem.ToString())).First().LastTradePrice.Multiply(Convert.ToDecimal(QuantityToBuy)).ToString());
                DialogResult dialogResult = MessageBox.Show(sMessage, "Buy stock?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    if (!m_oAccountData.AddHoldingToAccount(
                        QuantityToBuy,
                        comboBox_Symbols.SelectedItem.ToString(),
                        m_oTradeHandler
                        ))
                    {
                        MessageBox.Show(string.Format("Couldn't complete transaction! {0}", m_oAccountData.GetLastErrorMessage));
                    }
                    else
                    {
                        //refresh current account and list!
                        RefreshData();
                    }
                }
                else if (dialogResult == DialogResult.No)
                {
                    //do something else
                }


            }
        }

        private void RefreshData()
        {
            OnSelectedAccount(new SelectedAccountEventArg()
            {
                Identifier = m_oAccountData.Identifier
            });
        }

        private void button_buy_recommdation_Click(object sender, EventArgs e)
        {
            Instrument i;
            if (m_oAccountData != null && comboBox_Symbols.SelectedItem.ToString().Length > 0)
            {
                i = ImperaturGlobal.Instruments.Where(ins => ins.Symbol.Equals(comboBox_Symbols.SelectedItem.ToString())).First();

                int[] Intervals = Enumerable.Range(30, 180).ToArray();
                //int[] Intervals = new int[200];
                //for (int inv = 0; inv < 199; inv++)
                //{
                //    Intervals[inv] = 40 + (inv*2);
                //}
                SecurityAnalysis oSA = new SecurityAnalysis(i);
                if (!oSA.HasValue)
                {
                    return;
                }
                TradingRecommendation oTradeRec = new TradingRecommendation();
                bool bReccomend = false;
                foreach (int Interval in Intervals)
                {
                    if (m_oAccountData.GetAvailableFunds(new List<ICurrency> { ImperaturGlobal.GetMoney(0, i.CurrencyCode).CurrencyCode }).Count > 0 && m_oAccountData.GetAvailableFunds(new List<ICurrency> { ImperaturGlobal.GetMoney(0, i.CurrencyCode).CurrencyCode }).First().Amount < 1000m)
                    {
                        break;
                    }
                    bReccomend = oSA.RangeConvergeWithElliotForBuy(Interval, out oTradeRec);
                    if (bReccomend)
                        break;

                    /*if (oSA.RangeConvergeWithElliotForBuy(Interval, out SaleValue))
                    {
                        MessageBox.Show("Yes + " + SaleValue.ToString());

                        
                                            int Quantity = (int)(oA.GetAvailableFunds(new List<ICurrency> { GetMoney(0, i.CurrencyCode).CurrencyCode }).First().Amount
                                                /
                                                ImperaturGlobal.Quotes.Where(q => q.Symbol.Equals(i.Symbol)).First().LastTradePrice.Amount);


                                            //ImperaturGlobal.Quotes.Where(q => q.Symbol.Equals(comboBox_Symbols.SelectedItem.ToString())).First().LastTradePrice.Multiply(Convert.ToDecimal(QuantityToBuy))
                                            m_oAccountHandler.Accounts()[0].AddHoldingToAccount(
                                                Quantity - 1,
                                                i.Symbol,
                                                m_oTradeHandler
                                                );
                                                


                    }*/

                }
                if (bReccomend)
                {
                    MessageBox.Show(string.Format("Yes {0} at {1}", oTradeRec.BuyAtPrice ?? 0, oTradeRec.PredictedBuyDate ?? DateTime.MinValue));
                }
                else
                {
                    MessageBox.Show("No reccomendation");
                }

                //moving range??
            }
        }
    }
}
