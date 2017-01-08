using Imperatur_v2.securites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.shared;
using MathNet.Numerics.Interpolation;
using MathNet.Numerics.Statistics;
using Imperatur_v2.monetary;

namespace Imperatur_v2.trade.analysis
{



    public class SecurityAnalysis : ISecurityAnalysis
    {
        HistoricalQuote m_oH;
        private decimal[] m_oWave2Definition;
        private decimal[] m_oWave3Definition;
        private decimal m_oOffsetAllowed;
        private List<ElliotWave> oElliotWaveDefintion;

        public SecurityAnalysis(Instrument Instrument)
        {
            m_oH = ImperaturGlobal.HistoricalQuote(Instrument);
            m_oWave2Definition =new decimal[]{ 0.382m, 0.5m, 0.618m};
            m_oWave3Definition = new decimal[] { 1.618m, 2.618m };
            m_oOffsetAllowed = 0.1m;
            oElliotWaveDefintion = new List<ElliotWave>();
            //first
            oElliotWaveDefintion.Add(
                new ElliotWave
                {
                    Momentum = Momentum.Negative,
                    WaveNumber = 1
                }
                );
            //second
            oElliotWaveDefintion.Add(
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
            oElliotWaveDefintion.Add(
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
            oElliotWaveDefintion.Add(
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
            oElliotWaveDefintion.Add(
            new ElliotWave
            {
                Momentum = Momentum.Negative,
                WaveNumber = 4,
                RatioToWave = new List<Tuple<int, double>>
                {
                                            new Tuple<int, double>(1, 0.382),
                                            new Tuple<int, double>(1, 0.5),
                                            new Tuple<int, double>(1, 0.618),
                }
            }
            );
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
            ICurrency USD = ImperaturGlobal.GetMoney(0, "USD").CurrencyCode;
            if (DateOfValue >= DateTime.Now.Date)
            {
                if (ImperaturGlobal.Quotes.Where(q => q.Symbol.Equals(m_oH.Instrument.Symbol)).Count() > 0)
                {
                    AmountValue = ImperaturGlobal.Quotes.Where(q => q.Symbol.Equals(m_oH.Instrument.Symbol)).First().LastTradePrice
                        .Divide(ImperaturGlobal.GetPriceForCurrencyToday(USD)).Amount;
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
         * 
         */

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
            return oD;
            
        }


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
            foreach (Wave oW in oCleanedWaves)
            {
                if (wi > 5 && isElliotWaveDef)
                {
                    break;
                }
                ElliotWave oEw = oElliotWaveDefintion.Where(w => w.WaveNumber.Equals(wi)).First();
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
            /*
            CubicSpline oCs = CubicSpline.InterpolateNatural(

                                SecondPart.Select((s, i2) => new { i2, s })
                                .Where(t => t.s > Wave1Value)
                                .Select(t => Convert.ToDouble(t.i2)).ToArray(),

                                 SecondPart.Select(s => Convert.ToDouble(s)).ToArray());
                                 */
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

        public bool RangeConvergeWithElliotForBuy(int IntervalInDays, out decimal SaleValue)
        {
            return RangeConvergeWithElliotForBuy(DateTime.Now.AddDays(-IntervalInDays), DateTime.Now, out SaleValue);
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
            

            Double[] oSample = m_oH.HistoricalQuoteDetails
                .Where(h => h.Date >= Start.Date && h.Date.Date <= End)
                .Select(h => Convert.ToDouble(h.Close)).ToArray();

            return Statistics.MovingAverage(oSample, oSample.Count()).ToList();
        }

        private List<double> MovingAverage(List<double> Sample)
        {
            return Statistics.MovingAverage(Sample, Sample.Count()).ToList();
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
        public Momentum Momentum;
    }
    public struct ElliotWave
    {
        public List<Tuple<int, double>> RatioToWave;
        public Momentum Momentum;
        public int WaveNumber;
    }


}
