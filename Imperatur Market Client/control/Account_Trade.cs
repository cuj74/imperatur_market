﻿using System;
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

                    Imperatur_v2.trade.analysis.SecurityAnalysis oS = new Imperatur_v2.trade.analysis.SecurityAnalysis(ImperaturGlobal.Instruments.Where(i => i.Symbol.Equals(comboBox_Symbols.SelectedItem.ToString())).First());
                    label3.Text = oS.StandardDeviationForRange(DateTime.Now.AddDays(-7), DateTime.Now).ToString();
                    label3.Text += " | " + oS.StandardDeviation.ToString();
                    List<HistoricalQuoteDetails> oH = oS.GetDataForRange(DateTime.Now.AddMonths(-1), DateTime.Now);
                    CreateGraph(oH.Select(h => Convert.ToDouble(h.Close)).ToArray(), oH.Select(h => h.Date.ToString("yy-m-d")).ToArray());
                }
                else
                    label_instrument_info.Text = "N/A";
            }
        }

        private void CreateGraph(double[] yData, string[] xData)
        {
            // generate some fake data
            //double[] yData = { 1, 2, 3, 9, 1, 15, 3, 7, 2 };
            //string[] schools = { "A", "B", "C", "D", "E", "F", "G", "H", "J" };

            ZedGraphControl oZGP = new ZedGraphControl();
            //generate pane
            var pane = oZGP.GraphPane;


            pane.XAxis.Scale.IsVisible = true;
            pane.YAxis.Scale.IsVisible = true;

            pane.XAxis.MajorGrid.IsVisible = true;
            pane.YAxis.MajorGrid.IsVisible = true;

            pane.XAxis.Scale.TextLabels = xData;
            pane.XAxis.Type = AxisType.Text;


            //var pointsCurve;

            LineItem pointsCurve = pane.AddCurve("", null, yData, Color.Black);
            pointsCurve.Line.IsVisible = true;
            pointsCurve.Line.Width = 3.0F;
            //Create your own scale of colors.

            pointsCurve.Symbol.Fill = new Fill(new Color[] { Color.Blue, Color.Green, Color.Red });
            pointsCurve.Symbol.Fill.Type = FillType.Solid;
            pointsCurve.Symbol.Type = SymbolType.Circle;
            pointsCurve.Symbol.Border.IsVisible = true;



            pane.AxisChange();
            oZGP.Refresh();
            oZGP.Name = "StockChart";
            oZGP.Dock = DockStyle.Fill;





            if (!tableLayoutPanel_Trade.Controls.ContainsKey(oZGP.Name))
            {
                tableLayoutPanel_Trade.Controls.Add(oZGP, 1, 0);
            }
            else
            {
                tableLayoutPanel_Trade.Controls.RemoveByKey(oZGP.Name);
                tableLayoutPanel_Trade.Controls.Add(oZGP, 1, 0);
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

                int[] Intervals = new int[200];
                for (int inv = 0; inv < 199; inv++)
                {
                    Intervals[inv] = inv + 1 + 2;
                }
                SecurityAnalysis oSA = new SecurityAnalysis(i);
                if (!oSA.HasValue)
                {
                    return;
                }
                decimal SaleValue;
                foreach (int Interval in Intervals)
                {
                    if (m_oAccountData.GetAvailableFunds(new List<ICurrency> { ImperaturGlobal.GetMoney(0, i.CurrencyCode).CurrencyCode }).Count > 0 && m_oAccountData.GetAvailableFunds(new List<ICurrency> { ImperaturGlobal.GetMoney(0, i.CurrencyCode).CurrencyCode }).First().Amount < 1000m)
                    {
                        break;
                    }
                    
                    if (oSA.RangeConvergeWithElliotForBuy(Interval, out SaleValue))
                    {
                        MessageBox.Show("Yes + " + SaleValue.ToString());

                        /*
                                            int Quantity = (int)(oA.GetAvailableFunds(new List<ICurrency> { GetMoney(0, i.CurrencyCode).CurrencyCode }).First().Amount
                                                /
                                                ImperaturGlobal.Quotes.Where(q => q.Symbol.Equals(i.Symbol)).First().LastTradePrice.Amount);


                                            //ImperaturGlobal.Quotes.Where(q => q.Symbol.Equals(comboBox_Symbols.SelectedItem.ToString())).First().LastTradePrice.Multiply(Convert.ToDecimal(QuantityToBuy))
                                            m_oAccountHandler.Accounts()[0].AddHoldingToAccount(
                                                Quantity - 1,
                                                i.Symbol,
                                                m_oTradeHandler
                                                );
                                                */


                    }
                }


                //try other dateranges
                int[] IntervalsToStartfrom = new int[200];
                for (int inv = 0; inv < 199; inv++)
                {
                    Intervals[inv] = inv;
                }
                //decimal SaleValue2;
                int IntervalMultiplier = 10;
                //always from todaysdate
                foreach (int Interval in IntervalsToStartfrom)
                {
                    if (oSA.RangeConvergeWithElliotForBuy(DateTime.Now.AddDays(-(Interval + IntervalMultiplier)), DateTime.Now, out SaleValue))
                    {
                        MessageBox.Show("Yes + " + SaleValue.ToString());
                    }

                }
                //moving range
                foreach (int Interval in IntervalsToStartfrom)
                {
                    if (oSA.RangeConvergeWithElliotForBuy(DateTime.Now.AddDays(-(Interval + IntervalMultiplier)), DateTime.Now.AddDays(-Interval), out SaleValue))
                    {
                        MessageBox.Show("Yes + " + SaleValue.ToString());
                    }

                }
            }
        }
    }
}
