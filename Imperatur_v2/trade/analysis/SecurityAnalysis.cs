﻿using Imperatur_v2.securites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.shared;
using MathNet.Numerics;
using MathNet.Numerics.Interpolation;
using MathNet.Numerics.Statistics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearRegression;
using Imperatur_v2.monetary;
using MathNet.Numerics.LinearAlgebra.Complex;


namespace Imperatur_v2.trade.analysis
{



    public class SecurityAnalysis : ISecurityAnalysis
    {
        HistoricalQuote m_oH;
        ElliotWaveDefinition m_oED;
        /*
        private decimal[] m_oWave2Definition;
        private decimal[] m_oWave3Definition;
        private decimal m_oOffsetAllowed;
        */
        public SecurityAnalysis(Instrument Instrument)
        {
            m_oH = ImperaturGlobal.HistoricalQuote(Instrument);
            m_oED = new ElliotWaveDefinition();
            //m_oWave2Definition =new decimal[]{ 0.382m, 0.5m, 0.618m};
            //m_oWave3Definition = new decimal[] { 1.618m, 2.618m };
            //m_oOffsetAllowed = 0.1m;

        }

        public decimal StandardDeviation
        {
            get
            {
                //Double g = m_oH.HistoricalQuoteDetails.Select(h => Convert.ToDouble(h.Close)).StandardDeviation();
                return Convert.ToDecimal(m_oH.HistoricalQuoteDetails.Select(h => Convert.ToDouble(h.Close)).StandardDeviation());
            }
        }

        public bool HasValue
        {
            get
            {
                if (m_oH != null)
                    return true;
                return false;
            }
        }

        public decimal ChangeSince(DateTime From)
        {
            return (GetValueOfDate(DateTime.Now) / GetValueOfDate(From)) * 100;
        }

        public decimal GetValueOfDate(DateTime DateOfValue)
        {
            decimal AmountValue;
            //currency is correct!! No need to exchange
            //ICurrency USD = ImperaturGlobal.GetMoney(0, "USD").CurrencyCode;
            if (DateOfValue >= DateTime.Now.Date)
            {
                if (ImperaturGlobal.Quotes.Where(q => q.Symbol.Equals(m_oH.Instrument.Symbol)).Count() > 0)
                {
                    AmountValue = ImperaturGlobal.Quotes.Where(q => q.Symbol.Equals(m_oH.Instrument.Symbol)).First().LastTradePrice.Amount;
                        //.Divide(ImperaturGlobal.GetPriceForCurrencyToday(USD)).Amount;
                }
                else
                {
                    throw new Exception("TradePrice information not found!");
                }
            }
            else
            {
                DateTime ClosestDate = m_oH.HistoricalQuoteDetails.Where(h => h.Date >= DateOfValue.Date).Min(m => m.Date);
                AmountValue = m_oH.HistoricalQuoteDetails.Where(h => h.Date.Equals(ClosestDate)).First().Close;
            }
            return AmountValue;
        }

        public decimal ChangeBetween(DateTime From, DateTime To )
        {
            return (GetValueOfDate(To) / GetValueOfDate(From)) * 100;
        }


        #region ElliotWave
        /*
         * Wave
        Classical Relations between Waves
        1
        -
        2
        0.382, 0.5 or 0.618 of Wave 1 length
        3
        1.618 or 2.618 of Wave 1 length
        4
        0.382 or 0.5 of Wave 1 length
        5
        0.382, 0.5 or 0,618 of Wave 1 length
        A
        0.382, 0.5 or 0,618 of Wave 1 length
        B
        0.382 or 0.5 of Wave A length
        C
        1.618, 0.618 or 0.5 of Wave A length

Wave 2 cannot retrace more than 100% of Wave 1.
Wave 3 can never be the shortest of waves 1, 3, and 5.
Wave 4 can never overlap Wave 1.
         * 
         */
         /*
        private bool IsWaveElliot(decimal Wave1Lenght, decimal Wave2Lenght, decimal[] WaveDef)
        {
            decimal divider = Wave2Lenght / Wave1Lenght;
            foreach (decimal oDef in WaveDef)
            {
                if (divider >= Offset(oDef, m_oOffsetAllowed, false) && divider <= Offset(oDef, m_oOffsetAllowed, true))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsWaveElliot(double Wave1Lenght, double Wave2Lenght, double[] WaveDef)
        {
            double divider = Wave2Lenght / Wave1Lenght;
            foreach (double oDef in WaveDef)
            {
                if (divider >= Offset(oDef, Convert.ToDouble(m_oOffsetAllowed), false) && divider <= Offset(oDef, Convert.ToDouble(m_oOffsetAllowed), true))
                {
                    return true;
                }
            }
            return false;
        }

        private decimal Offset(decimal Offset, decimal Value, bool Add)
        {
            return Add ? Value - Value * Offset : Value + Value * Offset;
        }

        private double Offset(double Offset, double Value, bool Add)
        {
            return Add ? Value - Value * Offset : Value + Value * Offset;
        }
        */
        private List<decimal> GetRangeofDataPoints(DateTime From, int Take, int Skip)
        {
            return m_oH.HistoricalQuoteDetails
                .Where(h => h.Date >= From)
                .OrderBy(x => x.Date)
                .Select(h => h.Close)
                .Skip(Skip)
                .Take(Take).ToList();
        }

        public Momentum GetMomentumForRange(DateTime StartDate, DateTime EndDate)
        {
            List<double> oM = MovingAverageForRange(StartDate, EndDate);
            if (oM.First() < oM.Last())
            {
                return Momentum.Positive;
            }
            else if (oM.First() > oM.Last())
            {
                return Momentum.Negative;
            }
            return Momentum.Neutral;
        }
        private List<double> GetListOfPricesFromHistoricalQuoteDetails(HistoricalQuoteDetails oHD)
        {
            List<Double> oD = new List<double>();
            oD.Add(Convert.ToDouble(oHD.Open));
            oD.Add(Convert.ToDouble(oHD.Low));
            oD.Add(Convert.ToDouble(oHD.High));
            oD.Add(Convert.ToDouble(oHD.Close));
            return oD;
        }


        private List<double> GetRangeOfDataAsDoubleIncludingLowHigh(DateTime StartDate, DateTime EndDate)
        {
            List<Double> oD = new List<double>();

            m_oH.HistoricalQuoteDetails
                    .Where(h => h.Date >= StartDate.Date && h.Date <= EndDate.Date)
                    .OrderBy(x => x.Date)
                    .ToList().ForEach(p =>
                    oD.AddRange(GetListOfPricesFromHistoricalQuoteDetails(p)));
            //add the latest data from qoutes
            if (EndDate.Date.Equals(DateTime.Now.Date))
            {
                oD.Add(Convert.ToDouble(GetValueOfDate(EndDate)));
            }
            return oD;

        }
        /*
        public bool RangeConvergeWithElliotForBuy3(DateTime StartDate, DateTime EndDate, out decimal SaleValue)
        {
            SaleValue = 0m;
            List<double> oPriceData = GetRangeOfDataAsDoubleIncludingLowHigh(StartDate, EndDate);
            double[] xdata = oPriceData.Select((s, i2) => new { i2, s })
            .Select(t => Convert.ToDouble(t.i2)).ToArray();
            double[] ydata = oPriceData.ToArray();
            if (Fit.Line(xdata, ydata).Item2 >= 0)
            {
                //Positive/Neutral momement
                return false;
            }

            PolynomialRegression oP = new analysis.PolynomialRegression(new MathNet.Numerics.LinearAlgebra.Double.DenseVector(ydata), 20);

            List<Wave> oWaves = new List<Wave>();

            var WaveValues = oPriceData.Select((s, i2) => new { i2, s })
                .ToList().Select(f => oP.Calculate(Convert.ToDouble(f.i2))).ToArray();


            CubicSpline oSplineData = CubicSpline.InterpolateNaturalInplace(
                xdata,
                WaveValues
                );

            int i = 0;
            Momentum Current = Momentum.Neutral;
            Momentum Old = Momentum.Neutral;
            List<double> oCalcDoublesForWave = new List<double>();
            bool bFirst = true;
            foreach (var fvd in WaveValues)
            {
                double Diff = oSplineData.Differentiate(i);
                if (Diff < 0)
                    Current = Momentum.Negative;
                else if (Diff == 0)
                    Current = Momentum.Neutral;
                else
                    Current = Momentum.Positive;

                if (bFirst)
                {
                    oCalcDoublesForWave.Add(oSplineData.Interpolate(i));
                    bFirst = false;
                }
                else
                {
                    if (Current != Old && oCalcDoublesForWave.Count > 1)
                    {
                        oWaves.Add(new Wave
                        {
                            End = oCalcDoublesForWave.Last(),
                            Start = oCalcDoublesForWave.First(),
                            Length = oCalcDoublesForWave.Count,
                            Momentum = Old
                        });
                        oCalcDoublesForWave.Clear();
                    }
                    else if (Current != Old && oCalcDoublesForWave.Count <= 1)
                    {
                        oCalcDoublesForWave.Clear();
                    }
                    oCalcDoublesForWave.Add(oSplineData.Interpolate(i));
                }
                Old = Current;

                i++;
            }

            int ff = 0;

            return false;


        }*/

        private TradingRecommendation GetTradingRecommendationFromWaves(List<ConfirmedElliotWave> ConfirmedWaves, DateTime Start, DateTime End )
        {
            //only calc on 4 and up
            int MaxConfirmedWave = ConfirmedWaves.Max(c => c.WaveNumber);
            //titta på senaste index som har confirmed wave > 3
            int MaxIndex =  ConfirmedWaves.Where(cf=>cf.WaveNumber > 3).Max(c => c.Wave.SourceIndex);
            if (Start.AddDays((int)(MaxIndex/4)).Date < DateTime.Now.Date.AddDays(-2)) //just to test...
            {
                return new TradingRecommendation();
            }

            int WaveNumber = ConfirmedWaves.Where(cf => cf.Wave.SourceIndex.Equals(MaxIndex)).First().WaveNumber;
            if (MaxConfirmedWave > WaveNumber)
                MaxConfirmedWave = WaveNumber;

            if (MaxConfirmedWave < 4 || MaxConfirmedWave.Equals(m_oED.ElliotWaveDefinitions.Select(e=>e.WaveNumber).Max()))
            {
                ConfirmedElliotWave MaxConfirmedWaveObject = ConfirmedWaves.Where(f => f.Wave.SourceIndex.Equals(
                    ConfirmedWaves.Where(cw => cw.WaveNumber.Equals(MaxConfirmedWave)).Select(e => e.Wave.SourceIndex).Max()
                    )).First();
                double x = MaxConfirmedWaveObject.Wave.SourceIndex / 4;
                //DateTime ActionDateToTrade = Start.AddDays((int)x);
                //Last wave for buy!
                return new TradingRecommendation()
                {
                    BuyAtPrice = Convert.ToDecimal(MaxConfirmedWaveObject.Wave.End),
                    PredictedBuyDate = Start.AddDays((int)x)
                };
            }

                

            ElliotWave oE = m_oED.ElliotWaveDefinitions.Where(ed => ed.WaveNumber.Equals(MaxConfirmedWave + 1)).First();

            List<Wave> ToCompare = new List<Wave>();
           // List<decimal> BuyPoints = new List<decimal>();
            //get the new lowpoint, find the ElliotWave to compare to of this wave

            foreach (var oCW in ConfirmedWaves.Where(c=>c.WaveNumber.Equals(MaxConfirmedWave)))
            {
                foreach (var t in oE.RatioToWave)
                {
                    ToCompare.AddRange(ConfirmedWaves.Where(c => c.WaveNumber.Equals(t.Item1) && m_oED.IsWaveElliot(Convert.ToDouble(c.Wave.Length), Convert.ToDouble(oCW.Wave.Length), new double[] { t.Item2 })).Select(g=>g.Wave).ToList());
                }
            }

            //loop through the max waves detected(ToCompare) and get the new point according to oE
            //just to be on the safe side, pick the smallest value, largets value if we are excepting a raise
            double PredictedLenght = 0;
            if (oE.Momentum.Equals(Momentum.Negative))
                PredictedLenght = ToCompare.Select(s => s.Length * oE.RatioToWave.Select(t => t.Item2).Min()).Min();
            else
                PredictedLenght = ToCompare.Select(s => s.Length * oE.RatioToWave.Select(t => t.Item2).Min()).Max();

            //nu måste vi interpola vilket värde som det skulle kunna vara, tror att vi kan räkna med samma lutning som den vi jämför med.
            //räkna ut slope baserat på alla värden i tocompare
            // double SlopeAvg = ToCompare.Select(x => Fit.Line(Enumerable.Range(0, (int)x.Length - 1).Select(y=>Convert.ToDouble(y)).ToArray(), new double[] { x.Start, x.End }).Item2).Average();
            double SlopeAvg = ToCompare.Select(x => Fit.Line(new double[] { 0, 1 }, new double[] { x.Start, x.End }).Item2/x.Length).Average();

            //every day consists of four values (open low high close)
            //List<double> oPriceData = GetRangeOfDataAsDoubleIncludingLowHigh(Start, End);
            double XWaveIndex  = (ToCompare.Select(x => x.SourceIndex).Max() + PredictedLenght)/4;
            DateTime ActionDateToTrade = Start.AddDays((int)XWaveIndex);

            return new TradingRecommendation
            {
                BuyAtPrice = oE.Momentum.Equals(Momentum.Negative) ? Convert.ToDecimal(ConfirmedWaves.Where(c => c.WaveNumber.Equals(MaxConfirmedWave)).Last().Wave.End + (PredictedLenght * SlopeAvg)) : (decimal?)null,
                SellAtPrice = oE.Momentum.Equals(Momentum.Positive) ? Convert.ToDecimal(ConfirmedWaves.Where(c => c.WaveNumber.Equals(MaxConfirmedWave)).Last().Wave.End + (PredictedLenght * SlopeAvg)) : (decimal?)null,
                PredictedBuyDate = oE.Momentum.Equals(Momentum.Negative) ? ActionDateToTrade : (DateTime?)null,
                PredictedSellDate = oE.Momentum.Equals(Momentum.Positive) ? ActionDateToTrade : (DateTime?)null
            };

        }


        public bool RangeConvergeWithElliotForBuy(DateTime StartDate, DateTime EndDate, out TradingRecommendation TradingRecommendation)
        {
            List<Wave> oWaves = GetListOfWavesFromRange(StartDate, EndDate);
            List<ConfirmedElliotWave> oConfirmedWaves;
            TradingRecommendation = new TradingRecommendation();
            int LastWaveNumberHit;
            if (oWaves.Count > 1 && m_oED.FindElliottDefinitioninWaves(oWaves, out LastWaveNumberHit, out oConfirmedWaves))
            {
                if (LastWaveNumberHit > 0)
                {
                    TradingRecommendation = GetTradingRecommendationFromWaves(oConfirmedWaves, StartDate, EndDate);
                    if (TradingRecommendation.Equals(new TradingRecommendation()))
                        return false;
                    else
                        return true;
                }
                else
                    return false;
            }
            return false;
        }
        //public TradingRecommendation GetRecommendationFromElliotWawes
        private List<Wave> GetListOfWavesFromRange(DateTime StartDate, DateTime EndDate)
        {
            List<double> oPriceData = GetRangeOfDataAsDoubleIncludingLowHigh(StartDate, EndDate);
            if (oPriceData.Count < 5)
                return new List<Wave>();

            CubicSpline oCSTotalData = CubicSpline.InterpolateAkimaSorted(

                        oPriceData.Select((s, i2) => new { i2, s })
                        .Select(t => Convert.ToDouble(t.i2)).ToArray(),

                         oPriceData.Select(s => Convert.ToDouble(s)).ToArray());


            //create list of Waves
            List<Wave> oWaves = new List<Wave>();

            var FirstVarDiff = oPriceData.Select((s, i2) => new { i2, s })
                .ToList().Select(f => oCSTotalData.Differentiate(Convert.ToDouble(f.i2))).ToArray();

           // List<double> MovingAverageFirst = MovingAverage(oPriceData);
            int i = 0;
            Momentum Current = Momentum.Neutral;
            Momentum Old = Momentum.Neutral;
            List<double> oCalcDoublesForWave = new List<double>();
            bool bFirst = true;
            foreach (var fvd in FirstVarDiff)
            {
                if (fvd < 0)
                    Current = Momentum.Negative;
                else if (fvd == 0)
                    Current = Momentum.Neutral;
                else
                    Current = Momentum.Positive;

                if (bFirst)
                {
                    oCalcDoublesForWave.Add(oCSTotalData.Interpolate(i));
                    bFirst = false;
                }
                else
                {
                    if (Current != Old && oCalcDoublesForWave.Count > 1)
                    {
                        oWaves.Add(new Wave
                        {
                            End = oCalcDoublesForWave.Last(),
                            Start = oCalcDoublesForWave.First(),
                            Length = oCalcDoublesForWave.Count,
                            Momentum = Old,
                            SourceIndex = i

                        });
                        oCalcDoublesForWave.Clear();
                    }
                    else if (Current != Old && oCalcDoublesForWave.Count <= 1)
                    {
                        oCalcDoublesForWave.Clear();
                    }
                    oCalcDoublesForWave.Add(oCSTotalData.Interpolate(i));
                }
                Old = Current;

                i++;
            }
            return oWaves;
        }
        /*

        public bool RangeConvergeWithElliotForBuy2(DateTime StartDate, DateTime EndDate, out decimal SaleValue)
        {
            SaleValue = 0m;
            Momentum oCurrentMomentum = GetMomentumForRange(StartDate, EndDate);

            if (oCurrentMomentum.Equals(Momentum.Positive) || oCurrentMomentum.Equals(Momentum.Neutral))
            {
                return false;
            }

            int IntervalInDays = (int)(EndDate - StartDate).TotalDays;
            List<double> oPriceData = GetRangeOfDataAsDoubleIncludingLowHigh(StartDate, EndDate);

            double[] xdata = oPriceData.Select((s, i2) => new { i2, s })
                        .Select(t => Convert.ToDouble(t.i2)).ToArray();
            double[] ydata = oPriceData.ToArray();

            double[] PolynomialRegression =  Fit.Polynomial(xdata, ydata, 1);

            Func<int, double> func_GetYdata= (fx) => ydata[fx];
            var yVector = Vector<double>.Build.Dense(xdata.Count()-1, func_GetYdata);
            System.Numerics.Complex[] oComp = oPriceData.Select(c => (System.Numerics.Complex)c).ToArray();
            //Vector<System.Numerics.Complex> oDV = DenseVector.Build.DenseOfArray(oComp);
            MathNet.Numerics.LinearAlgebra.Double.DenseVector oDD = new MathNet.Numerics.LinearAlgebra.Double.DenseVector(ydata);

            PolynomialRegression oP = new analysis.PolynomialRegression(oDD, 1);

            foreach (double x in xdata)
            {
                double xres = oP.Calculate(x);
            }





            //Trend
            Tuple<double, double> LineVector = Fit.Line(xdata, ydata);
            double a = LineVector.Item1; // intercept
            double b = LineVector.Item2; // slope

            CubicSpline oCSTotalData = CubicSpline.InterpolateAkimaSorted(

                        oPriceData.Select((s, i2) => new { i2, s })
                        .Select(t => Convert.ToDouble(t.i2)).ToArray(),

                         oPriceData.Select(s => Convert.ToDouble(s)).ToArray());


            //create list of Waves
            List<Wave> oWaves = new List<Wave>();
            
            var FirstVarDiff = oPriceData.Select((s, i2) => new { i2, s })
                .ToList().Select(f => oCSTotalData.Differentiate(Convert.ToDouble(f.s))).ToArray();

            List<double> MovingAverageFirst = MovingAverage(oPriceData);
            int i = 0;
            Momentum Current = Momentum.Neutral;
            Momentum Old = Momentum.Neutral;
            List<double> oCalcDoublesForWave = new List<double>();
            bool bFirst = true;
            foreach (var fvd in FirstVarDiff)
            {
                if (fvd < 0)
                    Current = Momentum.Negative;
                else if (fvd == 0)
                    Current = Momentum.Neutral;
                else
                    Current = Momentum.Positive;

                if (bFirst)
                {
                    oCalcDoublesForWave.Add(oCSTotalData.Interpolate(i));
                    bFirst = false;
                }
                else
                {
                    if (Current != Old && oCalcDoublesForWave.Count > 1)
                    {
                        oWaves.Add(new Wave
                        {
                            End = oCalcDoublesForWave.Last(),
                            Start = oCalcDoublesForWave.First(),
                            Length = oCalcDoublesForWave.Count,
                            Momentum = Old
                        });
                        oCalcDoublesForWave.Clear();
                    }
                    else if (Current != Old && oCalcDoublesForWave.Count <= 1)
                    {
                        oCalcDoublesForWave.Clear();
                    }
                    oCalcDoublesForWave.Add(oCSTotalData.Interpolate(i));
                }
                Old = Current;

                i++;
            }

            //sort out the waves that are below 2% in lenght of total
            double MinLenght = (oPriceData.Count() / 100) * 5;

            List<Wave> oCleanedWaves = new List<Wave>();
            Wave oCleanedWave = new Wave();
            bool bFirstWave = true;
            foreach (Wave oW in oWaves)
            {
                if (bFirstWave)
                {
                    oCleanedWave = oW;
                    //oCleanedWaves.Add(oW);
                    bFirstWave = false;
                    continue;
                }
                if (oW.Length < MinLenght)
                {
                    oCleanedWave.Length += oW.Length;
                    oCleanedWave.End = oW.End;

                }
                else
                {
                    if (oCleanedWave.End < oCleanedWave.Start)
                    {
                        oCleanedWave.Momentum = Momentum.Negative;
                    }
                    else if (oCleanedWave.End == oCleanedWave.Start)
                    {
                        oCleanedWave.Momentum = Momentum.Neutral;
                    }
                    else
                        oCleanedWave.Momentum = Momentum.Positive;



                    oCleanedWaves.Add(oCleanedWave);
                    oCleanedWave.Start = oW.Start;
                    oCleanedWave.End = oW.End;


                }
            }
             if (oCleanedWaves.Count() < 6)
            {
                return false;
            }

            //find the elliotwave definition
            int wi = 1;
            bool isElliotWaveDef = true;
            ElliotWaveDefinition oED = new ElliotWaveDefinition();
            foreach (Wave oW in oCleanedWaves)
            {
                if (wi > 5 && isElliotWaveDef)
                {
                    break;
                }
                ElliotWave oEw = oED.ElliotWaveDefinitions.Where(w => w.WaveNumber.Equals(wi)).First();
                if (!oEw.Momentum.Equals(oW.Momentum))
                {
                    isElliotWaveDef = false;
                    break;
                }
                if (oEw.RatioToWave != null && oEw.RatioToWave.Count() > 0)
                {
                    bool bRatioPassed = false;
                    foreach(Tuple<int, double> WaveDef in oEw.RatioToWave)
                    {
                        Wave ToCompare = oCleanedWaves[WaveDef.Item1-1];
                        if (IsWaveElliot(ToCompare.Length, oW.Length, new double[] {WaveDef.Item2}))
                        {
                            bRatioPassed = true;
                            break;
                        }
                    }
                    isElliotWaveDef = bRatioPassed;
                }

                wi++;
            }

                //oElliotWaveDefintion
            int ggfgf = 0;
            return oCleanedWaves.Count() > 0 ? isElliotWaveDef : false;

        }
        */
        /*
        public bool RangeConvergeWithElliotForBuy(DateTime StartDate, DateTime EndDate, out decimal SaleValue)
        {
            SaleValue = -1m;
            //up since interval start? If yes the answer is no.
            
            try
            {
                if (ChangeBetween(StartDate, EndDate) > 0)
                    return false;
            }
            catch
            {
                return false;
            }
            int IntervalInDays = (int)(EndDate - StartDate).TotalDays;

            //OLD CODE FROM HERE!
            //split the range into three parts and find the standard deviation for each
            decimal ThreeParts = IntervalInDays / 3m;
            List<decimal> FirstPart = GetRangeofDataPoints(StartDate, (int)Math.Truncate(ThreeParts), 0);
            //add the last datapoint
            FirstPart.Add(
                PlotValue(m_oH.HistoricalQuoteDetails
                .Where(h => h.Date >= StartDate)
                .OrderBy(x => x.Date)
                .Select(h => h.Close)
                .ToArray(), ThreeParts));

            List<decimal> SecondPart = GetRangeofDataPoints(StartDate, (int)Math.Truncate(ThreeParts) * 2, (int)Math.Truncate(ThreeParts));
            //add the last datapoint
            SecondPart.Add(
                PlotValue(m_oH.HistoricalQuoteDetails
                .Where(h => h.Date >= StartDate)
                .OrderBy(x => x.Date)
                .Select(h => h.Close)
                .ToArray(), ThreeParts * 2));

            List<decimal> ThirdPart = GetRangeofDataPoints(StartDate, (int)Math.Truncate(ThreeParts) * 3, (int)Math.Truncate(ThreeParts) * 2);
            //add the last datapoint
            ThirdPart.Add(
                PlotValue(m_oH.HistoricalQuoteDetails
                .Where(h => h.Date >= StartDate)
                .OrderBy(x => x.Date)
                .Select(h => h.Close)
                .ToArray(), IntervalInDays)); //instead of ThreeParts * 3

            //analyze first part, looking for the lowest value and calcualate the lenght
            //do this for now, maybe use cubicspline it this is not ok
            decimal Wave1Value = FirstPart.Min();
            decimal Wave1Lenght = FirstPart.Select((s, i) => new { i, s })
                                .Where(t => t.s == Wave1Value)
                                .Select(t => t.i)
                                .ToList().Last();

            CubicSpline oCsFirstPart = CubicSpline.InterpolateNatural(

                    FirstPart.Select((s, i2) => new { i2, s })
                    .Select(t => Convert.ToDouble(t.i2)).ToArray(),

                     FirstPart.Select(s => Convert.ToDouble(s)).ToArray());


            //analyze second part, looking for the positive trends and analyze the lenght, compare it to first wave

            CubicSpline oCs = CubicSpline.InterpolateNatural(

                                SecondPart.Select((s, i2) => new { i2, s })
                                .Select(t => Convert.ToDouble(t.i2)).ToArray(),

                                 SecondPart.Select(s => Convert.ToDouble(s)).ToArray());

            bool bIsWave2Elliot = false;
            int index = 0;
            int LargestLenght = 0;
            decimal Wave2Value = 0;
            decimal Wave2Lenght = Wave1Lenght;
            foreach (decimal dPoint in SecondPart)
            {
                if (oCs.Differentiate((double)dPoint) > 0)
                {
                    index++;
                }
                else
                {
                    if (index > LargestLenght)
                    {
                        //see if the lenght of the wave is 0.382, 0.5 or 0.618 of Wave1Lenght, allow 10% diff
                        //also the dpoint must be higher than Wave1Value
                        if (dPoint > Wave1Value && IsWaveElliot(Wave1Lenght, Convert.ToDecimal(LargestLenght), m_oWave2Definition))
                        {
                            bIsWave2Elliot = true;
                            Wave2Lenght = Convert.ToDecimal(index);
                            break;
                        }

                        LargestLenght = index;
                        Wave2Value = dPoint;
                    }
                    index = 0;
                }
            }

            if (!bIsWave2Elliot)
            {
                return false;
            }

            //Third is more easy
            if (ThirdPart[ThirdPart.Count - 1] < Wave2Value &&
                 IsWaveElliot(Wave2Lenght, Convert.ToDecimal(ThirdPart.Count), m_oWave3Definition)
                )
            {
                //calculate the SaleValue, when should it be sold, before goin down again?
                //be safe, return the lowest value
                SaleValue = m_oH.HistoricalQuoteDetails.OrderByDescending(h => h.Date).Select(h => h.Close).First() * m_oWave2Definition.Min();

                //end 
                return true;
            }


            //nope, try another
            return false;
        }
        */
        public bool RangeConvergeWithElliotForBuy(int IntervalInDays, out TradingRecommendation TradingRecommendation)
        {
            return RangeConvergeWithElliotForBuy(DateTime.Now.AddDays(-IntervalInDays), DateTime.Now, out TradingRecommendation);
        }

        public bool RangeConvergeWithElliotForSell(int IntervalInDays)
        {
            return false;
        }
        #endregion

        public decimal StandardDeviationForRange(DateTime Start, DateTime End)
        {
            return Convert.ToDecimal(m_oH.HistoricalQuoteDetails
                .Where(h => h.Date >= Start.Date && h.Date.Date <= End)
                .Select(h => Convert.ToDouble(h.Close)).StandardDeviation());
        }

        #region private methods

        private decimal PlotValue(decimal[] DataPoints, decimal Position)
        {
            CubicSpline oCs = CubicSpline.InterpolateNatural(

                               DataPoints.Select((s, i2) => new { i2, s })
                               .Select(t => Convert.ToDouble(t.i2)).ToArray(),

                               DataPoints.Select(s => Convert.ToDouble(s)).ToArray());
            return Convert.ToDecimal(oCs.Interpolate(Convert.ToDouble(Position)));

        }

        public List<double> MovingAverageForRange(DateTime Start, DateTime End)
        {
            

            List<double> oSample = m_oH.HistoricalQuoteDetails
                .Where(h => h.Date >= Start.Date && h.Date.Date <= End)
                .Select(h => Convert.ToDouble(h.Close)).ToList();

            if (End.Date.Equals(DateTime.Now.Date))
            {
                oSample.Add(Convert.ToDouble(GetValueOfDate(End)));
            }


            return Statistics.MovingAverage(oSample, oSample.Count()).ToList();
        }

        private List<double> MovingAverage(List<double> Sample)
        {
            return Statistics.MovingAverage(Sample, Sample.Count()).ToList();
        }

        public List<HistoricalQuoteDetails> GetDataForRange(DateTime Start, DateTime End)
        {
            List<HistoricalQuoteDetails> oH = m_oH.HistoricalQuoteDetails
                    .Where(h => h.Date >= Start.Date && h.Date <= End.Date)
                    .OrderBy(x => x.Date)
                    .ToList();

            return oH;
        }
        #endregion


    }
    public enum Momentum
    {
        Positive,
        Negative,
        Neutral
    }
    public struct Wave
    {
        public double Start;
        public double End;
        public double Length;
        public DateTime StartDate;
        public int SourceIndex;
        public Momentum Momentum;
    }
    public struct ConfirmedElliotWave 
    {
        public Wave Wave;
        public int WaveNumber;
    }
    public struct ElliotWave
    {
        public List<Tuple<int, double>> RatioToWave;
        public Momentum Momentum;
        public int WaveNumber;
    }
    public struct TradingRecommendation
    {
        public decimal? SellAtPrice;
        public decimal? BuyAtPrice;
        public DateTime? PredictedBuyDate;
        public DateTime? PredictedSellDate;
    }

    public class ElliotWaveDefinition
    {
        private List<ElliotWave> m_oElliotWaveDefintion;
        private decimal m_oOffsetAllowed;

        public ElliotWaveDefinition()
        {
            m_oOffsetAllowed = 0.1m;
            m_oElliotWaveDefintion = new List<ElliotWave>();
            //first
            m_oElliotWaveDefintion.Add(
                new ElliotWave
                {
                    Momentum = Momentum.Negative,
                    WaveNumber = 1
                }
                );
            //second
            m_oElliotWaveDefintion.Add(
            new ElliotWave
            {
                Momentum = Momentum.Positive,
                WaveNumber = 2,
                RatioToWave = new List<Tuple<int, double>>
                {
                    new Tuple<int, double>(1, 0.382),
                    new Tuple<int, double>(1, 0.5),
                    new Tuple<int, double>(1, 0.618),
                }
            }
            );
            //third
            m_oElliotWaveDefintion.Add(
                new ElliotWave
                {
                    Momentum = Momentum.Negative,
                    WaveNumber = 3,
                    RatioToWave = new List<Tuple<int, double>>
                    {
                                    new Tuple<int, double>(1, 2.618),
                                    new Tuple<int, double>(1, 1.618),
                    }
                }
                );
            //Fourth
            m_oElliotWaveDefintion.Add(
            new ElliotWave
            {
                Momentum = Momentum.Positive,
                WaveNumber = 4,
                RatioToWave = new List<Tuple<int, double>>
                {
                                            new Tuple<int, double>(1, 0.382),
                                            new Tuple<int, double>(1, 0.5),
                }
            }
            );
            //Fifth
            m_oElliotWaveDefintion.Add(
            new ElliotWave
            {
                Momentum = Momentum.Negative,
                WaveNumber = 5,
                RatioToWave = new List<Tuple<int, double>>
                {
                                            new Tuple<int, double>(1, 0.382),
                                            new Tuple<int, double>(1, 0.5),
                                            new Tuple<int, double>(1, 0.618),
                }
            }
            );
        }

        public List<ElliotWave> ElliotWaveDefinitions
        {
            get { return m_oElliotWaveDefintion; }
        }

        private bool EvaluteWave(List<Wave> Waves, int WaveIndexToEvalute, ElliotWave ElliotWaveToUse)
        {
            bool isElliotWaveDef;
            if (!ElliotWaveToUse.Momentum.Equals(Waves[WaveIndexToEvalute].Momentum))
            {
                return false;
            }
            if (ElliotWaveToUse.RatioToWave != null && ElliotWaveToUse.RatioToWave.Count() > 0)
            {
                bool bRatioPassed = false;
                foreach (Tuple<int, double> WaveDef in ElliotWaveToUse.RatioToWave)
                {
                    if (WaveIndexToEvalute - ElliotWaveToUse.WaveNumber - WaveDef.Item1 < 0)
                    {
                        return false;
                    }
                    Wave ToCompare = Waves[WaveIndexToEvalute - ElliotWaveToUse.WaveNumber - WaveDef.Item1];
                    if (IsWaveElliot(ToCompare.Length, Waves[WaveIndexToEvalute].Length, new double[] { WaveDef.Item2 }))
                    {
                        bRatioPassed = true;
                        break;
                    }
                }
                isElliotWaveDef = bRatioPassed;
            }
            else
            {
                isElliotWaveDef =  true;
            }
            return isElliotWaveDef;
        }
        private bool IsWaveElliot(decimal Wave1Lenght, decimal Wave2Lenght, decimal[] WaveDef)
        {
            decimal divider = Wave2Lenght / Wave1Lenght;
            foreach (decimal oDef in WaveDef)
            {
                if (divider >= Offset(oDef, m_oOffsetAllowed, false) && divider <= Offset(oDef, m_oOffsetAllowed, true))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsWaveElliot(double Wave1Lenght, double Wave2Lenght, double[] WaveDef)
        {
            double divider = Wave2Lenght / Wave1Lenght;
            foreach (double oDef in WaveDef)
            {
                if (divider >= Offset(Convert.ToDouble(m_oOffsetAllowed), oDef, false) && divider <= Offset(Convert.ToDouble(m_oOffsetAllowed), oDef, true))
                {
                    return true;
                }
            }
            return false;
        }

        private decimal Offset(decimal Offset, decimal Value, bool Add)
        {
            return Add ? Value - Value * Offset : Value + Value * Offset;
        }

        private double Offset(double Offset, double Value, bool Add)
        {
            return Add ? Value + (Value * Offset) : Value - (Value * Offset);
            //return Add ? Value + Value * Offset : Value - Value * Offset;
        }


        public bool FindElliottDefinitioninWaves(List<Wave> Waves, out int LastWaveNumber, out List<ConfirmedElliotWave> ConfirmedElliotWaves)
        {
            List<Wave> oElliotConfirmedWaves = new List<Wave>();
            ConfirmedElliotWaves = new List<ConfirmedElliotWave>();
            LastWaveNumber = 0;
            foreach (var ElliotDef in m_oElliotWaveDefintion)
            {
                
                oElliotConfirmedWaves = Waves.Where(wl => EvaluteWave(Waves, Waves.FindIndex(a => a.Equals(wl)), ElliotDef)).ToList();
                if (oElliotConfirmedWaves.Count() == 0)
                {
                    return false;
                }
                else
                {
                    LastWaveNumber = ElliotDef.WaveNumber;
                    ConfirmedElliotWaves.AddRange(oElliotConfirmedWaves.Select(w => new ConfirmedElliotWave
                    {
                        Wave = w,
                        WaveNumber = ElliotDef.WaveNumber
                    }).ToList());
                }
            }
            return true;
            /*
            //var index = myList.FindIndex(a => a.Prop == oProp);
            //return false;

            //First find all waves thats corresponds to the first wave definition
            List<Wave> FirstWaves = new List<Wave>();
            for (int i = 0; i < Waves.Count()-1; i++)
            {
                if (EvaluteWave(Waves, i, m_oElliotWaveDefintion.Where(e=>e.WaveNumber == 1).First()))
                {
                    FirstWaves.Add(Waves[i]);
                }
            }

            //find second waves that match
            foreach (Wave FirstWave in FirstWaves)
            {
                //find the index in the original list
                var index = Waves.FindIndex(a => a.Equals(FirstWave));
                foreach (ElliotWave oEW in m_oElliotWaveDefintion.Where(ew=>ew.WaveNumber >1).ToArray())
                {
                   
                    //find next to look at
                    bool bUseAsFirst = false; 
                    foreach (Wave oW in FirstWaves)
                    {
                        //find the 

                        //need to fix!!! We don't know the index of the wave in the list!
                        if (bUseAsFirst)
                        {
                            if (EvaluteWave(Waves, i, m_oElliotWaveDefintion.Where(e => e.WaveNumber == 1).First())))
                            bUseAsFirst = false;
                        }
                        if (oW.Equals(FirstWave))
                        {
                            bUseAsFirst = true;
                        }
                    }
                       
                }
                

            }
        

            ElliotWave oEw = oED.ElliotWaveDefinitions.Where(w => w.WaveNumber.Equals(wi)).First();
            if (!oEw.Momentum.Equals(oW.Momentum))
            {
                isElliotWaveDef = false;
                break;
            }
            if (oEw.RatioToWave != null && oEw.RatioToWave.Count() > 0)
            {
                bool bRatioPassed = false;
                foreach (Tuple<int, double> WaveDef in oEw.RatioToWave)
                {
                    Wave ToCompare = oCleanedWaves[WaveDef.Item1 - 1];
                    if (IsWaveElliot(ToCompare.Length, oW.Length, new double[] { WaveDef.Item2 }))
                    {
                        bRatioPassed = true;
                        break;
                    }
                }
                isElliotWaveDef = bRatioPassed;
            }
                */
        }
    }
   
}
