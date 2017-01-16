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
using Imperatur_v2.trade.recommendation;

namespace Imperatur_Market_Client.control
{
    public partial class Account_Trade : UserControl
    {
        public event MainForm.SelectedAccountEventHandler SelectedAccount;
        private IAccountHandlerInterface m_oAh;
        private IAccountInterface m_oAccountData;
        private ITradeHandlerInterface m_oTradeHandler;
        private ISecurityAnalysis m_oSecAnalysis;
        private int m_oGraphSettingDays;
        private Instrument m_oI;
        private bool ShowMovingAverage = false;
        private bool ShowVolume = false;
        private List<Tuple<int,string, bool>> m_oDateRanges;
        private List<Tuple<TA_Indicator, string, bool>> m_oIndicator;
        private List<Tuple<TA_Settings, string, bool>> m_oGraphSettings;

        private enum TA_Indicator
        {
            Trend,
            MA,
            MA20_50_200,
            EMA,
            Bollinger,
            Crossover
        }

        private enum TA_Settings
        {
            Volume,
            High_Low
        }

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

            m_oDateRanges = new List<Tuple<int, string, bool>>();
            m_oIndicator = new List<Tuple<TA_Indicator, string, bool>>();
            m_oGraphSettings = new List<Tuple<TA_Settings, string, bool>>();

            //DateRanges
            m_oDateRanges.Add(new Tuple<int, string, bool>(1, "1 day", false));
            m_oDateRanges.Add(new Tuple<int, string, bool>(3, "3 days", false));
            m_oDateRanges.Add(new Tuple<int, string, bool>(7, "7 days", false));
            m_oDateRanges.Add(new Tuple<int, string, bool>(90, "3 months", false));
            m_oDateRanges.Add(new Tuple<int, string, bool>(365, "1 year", false));

            //Indicators
            foreach (TA_Indicator indicator in Enum.GetValues(typeof(TA_Indicator)))
            {
                m_oIndicator.Add(new Tuple<TA_Indicator, string, bool>(indicator, indicator.ToString(), false));
            }

            foreach (TA_Settings setting in Enum.GetValues(typeof(TA_Settings)))
            {
                m_oGraphSettings.Add(new Tuple<TA_Settings, string, bool>(setting, setting.ToString(), false));
            }
            foreach (var dr in m_oDateRanges)
            {
                comboBox_daterange.Items.Add(dr.Item2);
            }
            comboBox_daterange.SelectedIndexChanged += ComboBox_daterange_SelectedIndexChanged;

            foreach (var dr in m_oIndicator)
            {
                checkBoxComboBox_TA.Items.Add(dr.Item2);
            }

            checkBoxComboBox_TA.CheckBoxCheckedChanged += CheckBoxComboBox_TA_CheckBoxCheckedChanged;
            //checkBoxComboBox_TA.DropDownClosed += CheckBoxComboBox_TA_DropDownClosed;
            foreach (var dr in m_oGraphSettings)
            {
                checkBoxComboBox_Settings.Items.Add(dr.Item2);
            }
            checkBoxComboBox_Settings.CheckBoxCheckedChanged += CheckBoxComboBox_Settings_CheckBoxCheckedChanged;
            SetDateRangeToNumber(7);
            
        }
        /*
        private void CheckBoxComboBox_TA_DropDownClosed(object sender, EventArgs e)
        {
            foreach (var t in checkBoxComboBox_TA.CheckBoxItems)
            {
                if (t.Checked)
                {
                    m_oGraphSettings = m_oGraphSettings.Select(x => new Tuple<TA_Settings, string, bool>(x.Item1, x.Item2, x.Item1.Equals(t.Text) ? true : x.Item3)).ToList();
                }
            }
            ChangeGraph();
        }*/

        private void CheckBoxComboBox_Settings_CheckBoxCheckedChanged(object sender, EventArgs e)
        {
            var t = (PresentationControls.CheckBoxComboBoxItem)sender;
            m_oGraphSettings = m_oGraphSettings.Select(x => new Tuple<TA_Settings, string, bool>(x.Item1, x.Item2, x.Item2.Equals(t.Text) ? t.Checked : x.Item3)).ToList();
            ChangeGraph();
        }

        private void CheckBoxComboBox_TA_CheckBoxCheckedChanged(object sender, EventArgs e)
        {
            var t = (PresentationControls.CheckBoxComboBoxItem)sender;
            m_oIndicator = m_oIndicator.Select(x => new Tuple<TA_Indicator, string, bool>(x.Item1, x.Item2, x.Item2.Equals(t.Text) ? t.Checked : x.Item3)).ToList();
            ChangeGraph();
        }


        private void SetDateRangeToNumber(int Range)
        {
            m_oDateRanges = m_oDateRanges.Select(x => new Tuple<int, string, bool>(x.Item1, x.Item2, x.Item1.Equals(Range))).ToList();
        }

        private void ComboBox_daterange_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_daterange.SelectedItem != null)
            {
                SetDateRangeToNumber(m_oDateRanges.Where(x => x.Item2.Equals(comboBox_daterange.SelectedItem.ToString())).First().Item1);
            }
            ChangeGraph();
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
                    Quote oQ = ImperaturGlobal.Quotes.Where(q => q.Symbol.Equals(comboBox_Symbols.SelectedItem.ToString())).First();
                    m_oI = ImperaturGlobal.Instruments.Where(i => i.Symbol.Equals(comboBox_Symbols.SelectedItem.ToString())).First();
                    label_instrument_info.Text =
                        string.Format("{0}({1}) Last: {2}", m_oI.Name, m_oI.Symbol, oQ.LastTradePrice);
                        
                    m_oSecAnalysis = m_oTradeHandler.GetSecurityAnalysis(m_oI);
                    label3.Text = string.Format("Today: {0} / {1}", oQ.ChangePercent, oQ.Change);
                    ChangeGraph();
                    

                }
                else
                    label_instrument_info.Text = "N/A";
            }
        }

        private void ChangeGraph()
        {
            int days = m_oDateRanges.Where(x => x.Item3).First().Item1;
            List<HistoricalQuoteDetails> oH = m_oSecAnalysis.GetDataForRange(DateTime.Now.AddDays(-days), DateTime.Now);
            string[] xData = days > 1 ?
                oH.Select(h => h.Date.ToShortDateString()).ToArray()
                :
                oH.Select(h => h.Date.ToShortTimeString()).ToArray();
            double[] yData = oH.Select(h => Convert.ToDouble(h.Close)).ToArray();

            ZedGraphControl PriceGraph = CreatePriceGraph(xData, yData);

            List<Tuple<string, double[]>> AdditionalGraphs = new List<Tuple<string,double[]>>();

            foreach(var TA in m_oIndicator.Where(t=>t.Item3 == true).ToList())
            {
                switch(TA.Item1)
                {
                    case TA_Indicator.Trend:
                        break;
                    case TA_Indicator.MA:
                        break;
                    case TA_Indicator.MA20_50_200:
                        break;
                    case TA_Indicator.EMA:
                        break;
                    case TA_Indicator.Bollinger:
                        GetBollinger(days).ForEach(x =>
                             AdditionalGraphs.Add(new Tuple<string, double[]>(TA_Indicator.Bollinger.ToString(), x.ToArray()
                            )));
                        break;
                    case TA_Indicator.Crossover:
                        break;
                    default:
                        break;
                }

            }
            foreach (var GS in m_oGraphSettings.Where(t => t.Item3 == true).ToList())
            {
                switch (GS.Item1)
                {
                    case TA_Settings.Volume:
                        var oVolumeInfoList = m_oSecAnalysis.GetRangeOfVolumeIndicator(DateTime.Now.AddDays(-days), DateTime.Now)
                        .Where(h => h.Item1.Date >= DateTime.Now.AddDays(-days).Date && h.Item1.Date <= DateTime.Now.Date)
                        .OrderBy(x => x.Item1)
                        .ToList();

                        if (oVolumeInfoList.Count() > 0)
                        {
                            foreach (VolumeIndicatorType VolumeType in Enum.GetValues(typeof(VolumeIndicatorType)))
                            {
                                if ((VolumeType != VolumeIndicatorType.Unknown && VolumeType != VolumeIndicatorType.VolumeClimaxPlusHighVolumeChurn) && oVolumeInfoList.Where(z => z.Item2.VolumeIndicatorType.Equals(VolumeType)).Count() > 0)
                                { 
                                    AdditionalGraphs.Add(new Tuple<string, double[]>(VolumeType.ToString(),
                                    yData.Select((s, i2) => new { i2, s })
                                    .Select(t => (t.i2 < oVolumeInfoList.Count()) ? oVolumeInfoList[t.i2].Item2.VolumeIndicatorType.Equals(VolumeType) ? t.s : 0 : 0).ToArray()));
                                }
                            }
                        }
                            break;
                    case TA_Settings.High_Low:
                        PriceGraph.GraphPane.GraphObjList.Add(DrawLine(yData.Max(), yData.Count(), GS.Item1.ToString()));
                        var text = new TextObj("High", 1, yData.Max()-10, CoordType.ChartFraction, AlignH.Left, AlignV.Top);
                        text.ZOrder = ZOrder.A_InFront;
                        PriceGraph.GraphPane.GraphObjList.Add(text);
                        PriceGraph.GraphPane.GraphObjList.Add(DrawLine(yData.Min(), yData.Count(), GS.Item1.ToString()));
                        var textlow = new TextObj("Low", 1, yData.Min() + 10, CoordType.ChartFraction, AlignH.Left, AlignV.Top);
                        textlow.ZOrder = ZOrder.A_InFront;
                        PriceGraph.GraphPane.GraphObjList.Add(textlow);
                        break;
                    default:
                        break;
                }
            }


            var pane = PriceGraph.GraphPane;
            
            AdditionalGraphs.ForEach(x =>
            pane.CurveList.Add(CreateLineItemFromData(x.Item2, x.Item1)
            ));
            if (AdditionalGraphs.Count() > 0)
            {
                pane.YAxis.Scale.Max = AdditionalGraphs.Select(x => x.Item2.Max()).Max() * 1.0005;
                pane.YAxis.Scale.Min = AdditionalGraphs.SelectMany(d=>d.Item2).Where(x=>x >0).Min() * 0.9995;
            }

            PriceGraph.Refresh();
            PriceGraph.Name = "StockChart";
            PriceGraph.Dock = DockStyle.Fill;
            if (!panel_chart.Controls.ContainsKey(PriceGraph.Name))
            {
                panel_chart.Controls.Add(PriceGraph);
                //if (AdditionalGraphs.Where(x=>x.Item1.Equals(TA_Settings.Volume.ToString())).Count() > 0)
                //if (Volume.Count() > 0 && ShowVolume)
                //    panel_vol.Controls.Add(oZGP_vol);
            }
            else
            {
                panel_chart.Controls.RemoveByKey(PriceGraph.Name);
                panel_chart.Controls.Add(PriceGraph);

               // panel_vol.Controls.RemoveByKey(oZGP_vol.Name);
                //if (Volume.Count() > 0 && ShowVolume)
                //    panel_vol.Controls.Add(oZGP_vol);
            }

            tableLayoutPanel_Graph.Visible = true;

        }
        private List<List<double>> GetBollinger(int days)
        {
           return m_oSecAnalysis.StandardBollingerForRange(DateTime.Now.AddDays(-days), DateTime.Now);
        }

        private ZedGraphControl CreatePriceGraph(string[] xData, double[] yData)
        {
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
            pane.XAxis.Type = AxisType.Text;

            var curve1 = new LineItem(null, yData.Select((s, i2) => new { i2, s })
                               .Select(t => Convert.ToDouble(t.i2)).ToArray(),
               yData, Color.Black, SymbolType.None);
            curve1.Line.IsAntiAlias = true;

            pane.CurveList.Add(curve1);
            pane.AxisChange();
            return oZGP;

        }

        private LineItem CreateLineItemFromData(double[] yData, string Identifier)
        {
            LineItem oRet = new LineItem(null, null, yData, Color.Gainsboro, SymbolType.None);
            if (Identifier.Equals(TA_Indicator.Bollinger.ToString()))
            {
                oRet = new LineItem(null, null, yData, Color.PaleVioletRed, SymbolType.None);
            }
            else if (Identifier.Equals(VolumeIndicatorType.VolumeClimaxUp.ToString()))
            {
                oRet = new LineItem(null, null, yData, Color.Red, SymbolType.Triangle);
                oRet.Line.IsVisible = false;
                oRet.Symbol.Fill = new Fill(Color.Red);
            }
            else if (Identifier.Equals(VolumeIndicatorType.VolumeClimaxDown.ToString()))
            {
                oRet = new LineItem(null, null, yData, Color.Green, SymbolType.TriangleDown);
                oRet.Line.IsVisible = false;
                oRet.Symbol.Fill = new Fill(Color.Green);
            }
            else if (Identifier.Equals(VolumeIndicatorType.HighVolumeChurn.ToString()))
            {
                oRet = new LineItem(null, null, yData, Color.Blue, SymbolType.Diamond);
                oRet.Line.IsVisible = false;
                oRet.Symbol.Fill = new Fill(Color.Green);
            }
            else if (Identifier.Equals(VolumeIndicatorType.LowVolume.ToString()))
            {
                oRet = new LineItem(null, null, yData, Color.YellowGreen, SymbolType.Circle);
                oRet.Line.IsVisible = false;
                oRet.Symbol.Fill = new Fill(Color.YellowGreen);
            }
 
            oRet.Line.IsAntiAlias = true;

            return oRet;

        }
        private LineObj DrawLine(double y, double xMax, string Identifier)
        {
             if (Identifier.Equals(TA_Settings.High_Low.ToString()))
            {
                LineObj threshHoldLine = new LineObj(
                    Color.Plum,
                    1,
                    y,
                    xMax,
                    y);
                
                return threshHoldLine;
            }
            return null;
        }
        private void ChangeGraph(int days, bool AddMovingAverage = false, bool AddVolume = false)
        {
            if (m_oSecAnalysis != null)
            {
                List<HistoricalQuoteDetails> oH = m_oSecAnalysis.GetDataForRange(DateTime.Now.AddDays(-days), DateTime.Now);
                string[] xData = days > 1 ?
                    oH.Select(h => h.Date.ToShortDateString()).ToArray()
                    :
                    oH.Select(h => h.Date.ToShortTimeString()).ToArray();

                List<List<double>> oM = new List<List<double>>();
                if (AddMovingAverage)
                {
                    oM = m_oSecAnalysis.StandardBollingerForRange(DateTime.Now.AddDays(-days), DateTime.Now);
                }
                var oVi = m_oSecAnalysis.GetRangeOfVolumeIndicator(DateTime.Now.AddDays(-days), DateTime.Now)
                                        .Where(h => h.Item1.Date >= DateTime.Now.AddDays(-days).Date && h.Item1.Date <= DateTime.Now.Date)
                    .OrderBy(x => x.Item1)
                    .ToList();


                CreateGraph(oH.Select(h => Convert.ToDouble(h.Close)).ToArray(), xData, (days == 1), AddMovingAverage ? oM : new List<List<double>>(), AddVolume ? oH.Select(x => Convert.ToDouble(x.Volume)).ToList() : new List<double>(),
                    AddVolume ? 
                    oVi
                    :
                    new List<Tuple<DateTime, VolumeIndicator>>()
                    );
            }
        }

        private void CreateGraph(double[] yData, string[] xData, bool ShowTimeInX, List<List<double>> movingaverage_yData, List<double> Volume, List<Tuple<DateTime, VolumeIndicator>> VI)
        {
            /*
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
                    this.tableLayoutPanel_Graph.Controls.Add(b, i, 2);
                }
                //add moving average
                Button bma = new Button();
                bma.Name = "b_movingaverage";
                bma.Text = "MA";
                bma.Click += BChartChange_Click;
                this.tableLayoutPanel_Graph.Controls.Add(bma, 5, 2);

                //add volume
                Button bvol = new Button();
                bvol.Name = "b_vol";
                bvol.Text = "VOL";
                bvol.Click += BChartChange_Click;
                this.tableLayoutPanel_Graph.Controls.Add(bvol, 6, 2);

                PresentationControls.CheckBoxComboBox TestCombo = new PresentationControls.CheckBoxComboBox();

                TestCombo.Items.Add("Item 1");
                TestCombo.Items.Add("Item 2");
                TestCombo.Items.Add("Item 3");
                TestCombo.Items.Add("Item 4");
                TestCombo.Items.Add("Item 5");
                TestCombo.Items.Add("Item 6");
                TestCombo.Items.Add("Item 7");
                TestCombo.Items.Add("Item 8");
                this.tableLayoutPanel_Graph.Controls.Add(TestCombo, 7, 2);

            }
            */
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
            pane.XAxis.Type = AxisType.Text;

            var curve1 = new LineItem(null, yData.Select((s, i2) => new { i2, s })
                               .Select(t => Convert.ToDouble(t.i2)).ToArray(),
               yData, Color.Black, SymbolType.None);
            curve1.Line.IsAntiAlias = true;

            pane.CurveList.Add(curve1);

            foreach (List<double> oM in movingaverage_yData)
            {

                var curve_MA = new LineItem(null, null,
                 oM.ToArray(), Color.PaleVioletRed, SymbolType.None);
                pane.CurveList.Add(curve_MA);
                curve_MA.Line.IsAntiAlias = true;


            }

            double MaxY = 0;
            double MinY = 0;
            if (movingaverage_yData.Count() > 0)
            {
                MaxY = movingaverage_yData.Select(x => x.Max()).Max();
                MinY = movingaverage_yData.Select(x => x.Min()).Min();
            }

            MaxY = (MaxY > yData.Max() && MaxY > 0) ? MaxY : yData.Max();
            MinY = (MinY < yData.Min() && MinY > 0) ? MinY : yData.Min();
            pane.XAxis.Scale.MaxAuto = true;
            pane.XAxis.Scale.MinAuto = true;
            pane.XAxis.Scale.Min = 1;
            pane.XAxis.Scale.Max = Convert.ToDouble(yData.Count());

            pane.YAxis.Scale.MaxAuto = true;
            pane.YAxis.Scale.MinAuto = true;
            pane.YAxis.Scale.Min = MinY * 0.998;
            pane.YAxis.Scale.Max = MaxY * 1.002;


            ZedGraphControl oZGP_vol = new ZedGraphControl();
            if (Volume.Count() > 0)
            {
                
                //generate pane
                var pane_vol = oZGP_vol.GraphPane;
                pane_vol.Title.Text = string.Format("{0} ({1})", m_oI.Name, m_oI.Symbol);
                pane_vol.YAxis.Title.Text = "Volume";
                pane_vol.XAxis.Scale.IsVisible = false;
                pane_vol.YAxis.Scale.IsVisible = true;
                pane_vol.XAxis.MajorGrid.IsVisible = false;
                pane_vol.YAxis.MajorGrid.IsVisible = false;
                pane_vol.XAxis.Type = AxisType.Text;


                BarItem VolBar = pane_vol.AddBar("", Volume.Select((s, i2) => new { i2, s })
                               .Select(t => Convert.ToDouble(t.i2)).ToArray(), Volume.ToArray(), Color.DeepSkyBlue);
                if (VI.Count() > 0)
                {
                    //always add bar
                    double[] ViIndication = VI.Select(x => x.Item2.VolumeIndicatorType.Equals(VolumeIndicatorType.VolumeClimaxUp) ? Volume.Max() : 0).ToArray();
                    pane_vol.AddBar("", Volume.Select((s, i2) => new { i2, s })
                    .Select(t => Convert.ToDouble(t.i2)).ToArray(), ViIndication, Color.Green);

                    double[] yDataVolumeClimaxUp = yData.Select((s, i2) => new { i2, s })
                    .Select(t => (t.i2 < VI.Count()) ?  VI[t.i2].Item2.VolumeIndicatorType.Equals(VolumeIndicatorType.VolumeClimaxUp) ? t.s : 0 : 0).ToArray();

                    double[] yDataVolumeVolumeClimaxDown = yData.Select((s, i2) => new { i2, s })
                    .Select(t => (t.i2 < VI.Count()) ? VI[t.i2].Item2.VolumeIndicatorType.Equals(VolumeIndicatorType.VolumeClimaxDown) ? t.s : 0 : 0).ToArray();


                    double[] yDataHighVolumeChurn = yData.Select((s, i2) => new { i2, s })
                    .Select(t => (t.i2 < VI.Count()) ? VI[t.i2].Item2.VolumeIndicatorType.Equals(VolumeIndicatorType.HighVolumeChurn) ? t.s : 0 : 0).ToArray();

                    double[] yDataLowVolume = yData.Select((s, i2) => new { i2, s })
                    .Select(t => (t.i2 < VI.Count()) ? VI[t.i2].Item2.VolumeIndicatorType.Equals(VolumeIndicatorType.LowVolume) ? t.s : 0 : 0).ToArray();


                    if (yDataVolumeClimaxUp.Max() > 0)
                    {
                        var curve_VolumeClimaxUp = new LineItem(null, yData.Select((s, i2) => new { i2, s })
                        .Select(t => Convert.ToDouble(t.i2)).ToArray(),
                        yDataVolumeClimaxUp, Color.Red, SymbolType.Triangle);
                        curve_VolumeClimaxUp.Line.IsVisible = false;
                        curve_VolumeClimaxUp.Symbol.Fill = new Fill(Color.Red);
                        curve_VolumeClimaxUp.Symbol.IsAntiAlias = true;

                        pane.CurveList.Add(curve_VolumeClimaxUp);
                    }
                    if (yDataVolumeVolumeClimaxDown.Max() > 0)
                    {
                        var curve_VolumeClimaxDown = new LineItem(null, yData.Select((s, i2) => new { i2, s })
                        .Select(t => Convert.ToDouble(t.i2)).ToArray(),
                        yDataVolumeVolumeClimaxDown, Color.Green, SymbolType.TriangleDown);
                        curve_VolumeClimaxDown.Line.IsVisible = false;
                        curve_VolumeClimaxDown.Symbol.Fill = new Fill(Color.Green);
                        curve_VolumeClimaxDown.Symbol.IsAntiAlias = true;
                        pane.CurveList.Add(curve_VolumeClimaxDown);
                    }

                    if (yDataHighVolumeChurn.Max() > 0)
                    {
                        var curve_DataHighVolumeChurn = new LineItem(null, yData.Select((s, i2) => new { i2, s })
                        .Select(t => Convert.ToDouble(t.i2)).ToArray(),
                        yDataHighVolumeChurn, Color.Blue, SymbolType.Diamond);
                        curve_DataHighVolumeChurn.Line.IsVisible = false;
                        curve_DataHighVolumeChurn.Symbol.Fill = new Fill(Color.Blue);
                        curve_DataHighVolumeChurn.Symbol.IsAntiAlias = true;

                        pane.CurveList.Add(curve_DataHighVolumeChurn);
                    }

                    if (yDataLowVolume.Max() > 0)
                    {
                        var curve_DataLowVolume = new LineItem(null, yData.Select((s, i2) => new { i2, s })
                        .Select(t => Convert.ToDouble(t.i2)).ToArray(),
                        yDataLowVolume, Color.YellowGreen, SymbolType.Circle);
                        curve_DataLowVolume.Line.IsVisible = false;
                        curve_DataLowVolume.Symbol.Fill = new Fill(Color.YellowGreen);
                        //curve_DataLowVolume.Symbol.Size = 0.8F;
                        curve_DataLowVolume.Symbol.IsAntiAlias = true;

                        pane.CurveList.Add(curve_DataLowVolume);
                    }



                    ViIndication = VI.Select(x => x.Item2.VolumeIndicatorType.Equals(VolumeIndicatorType.VolumeClimaxDown) ? Volume.Max() : 0).ToArray();
                    pane_vol.AddBar("", Volume.Select((s, i2) => new { i2, s })
                    .Select(t => Convert.ToDouble(t.i2)).ToArray(), ViIndication, Color.Red);
                }
                pane_vol.AxisChange();
                oZGP_vol.Refresh();
                oZGP_vol.Name = "Volume";
                oZGP_vol.Dock = DockStyle.Fill;
            }
            pane.AxisChange();
            oZGP.Refresh();
            oZGP.Name = "StockChart";
            oZGP.Dock = DockStyle.Fill;
            if (!panel_chart.Controls.ContainsKey(oZGP.Name))
            {
                panel_chart.Controls.Add(oZGP);
                if (Volume.Count() > 0 && ShowVolume)
                    panel_vol.Controls.Add(oZGP_vol);
            }
            else
            {
                panel_chart.Controls.RemoveByKey(oZGP.Name);
                panel_chart.Controls.Add(oZGP);

                panel_vol.Controls.RemoveByKey(oZGP_vol.Name);
                if (Volume.Count() > 0 && ShowVolume)
                    panel_vol.Controls.Add(oZGP_vol);
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

                ChangeGraph(m_oGraphSettingDays, ShowMovingAverage, ShowVolume);
                return;
            }
            if (ob.Name == "b_vol")
            {
                if (ShowVolume)
                {
                    ShowVolume = false;
                }
                else
                    ShowVolume = true;

                ChangeGraph(m_oGraphSettingDays, ShowMovingAverage, ShowVolume);
                return;
            }


            switch (ob.Text)
            {
                case "1d":
                    m_oGraphSettingDays = 1;
                    ChangeGraph(1, ShowMovingAverage, ShowVolume);
                    break;
                case "3d":
                    m_oGraphSettingDays = 3;
                    ChangeGraph(3, ShowMovingAverage, ShowVolume);
                    break;
                case "1w":
                    m_oGraphSettingDays = 7;
                    ChangeGraph(7, ShowMovingAverage, ShowVolume);
                    break;
                case "1m":
                    m_oGraphSettingDays = 30;
                    ChangeGraph(30, ShowMovingAverage, ShowVolume);
                    break;
                case "1y":
                    m_oGraphSettingDays = 360;
                    ChangeGraph(360, ShowMovingAverage, ShowVolume);
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
            //Instrument i;
            if (m_oAccountData != null && comboBox_Symbols.SelectedItem.ToString().Length > 0)
            {
                //i = ImperaturGlobal.Instruments.Where(ins => ins.Symbol.Equals(comboBox_Symbols.SelectedItem.ToString())).First();
                List<TradingRecommendation> oT =  ImperaturGlobal.GetTradingRecommendation(comboBox_Symbols.SelectedItem.ToString());
                if (oT != null && oT.Count() > 0) {
                    int BuyRecommendation = oT.Where(t => t.BuyPrice.Amount > 0 && t.SellPrice.Amount == 0).Count();
                    int SellRecommendation = oT.Where(t => t.BuyPrice.Amount == 0 && t.SellPrice.Amount < 0).Count();
                    int KeepRecommendation = oT.Where(t => t.TradingForecastMethod.Equals(TradingForecastMethod.Undefined)).Count();

                    if (BuyRecommendation.Equals(SellRecommendation) && SellRecommendation.Equals(KeepRecommendation))
                    {
                        MessageBox.Show("Keep");
                    }
                    else
                    {
                        MessageBox.Show(string.Format("buy: {0}, sell {1}, keep {2}", BuyRecommendation, SellRecommendation, KeepRecommendation));
                    }
                }
                else
                {
                    MessageBox.Show("Keep (no info)");
                }

            }
        }
    }
}
