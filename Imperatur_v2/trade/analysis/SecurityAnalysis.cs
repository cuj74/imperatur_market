using Imperatur_v2.securites;
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
using Imperatur_v2.trade.recommendation;

namespace Imperatur_v2.trade.analysis
{



    public class SecurityAnalysis : ISecurityAnalysis
    {
        private HistoricalQuote m_oH;
        private ElliottWaveDefinition m_oED;
        private List<Tuple<DateTime, List<HistoricalQuoteDetails>, int>> m_oCache;
        private const int CacheSeconds = 180;

        public SecurityAnalysis(Instrument Instrument)
        {
            m_oH = ImperaturGlobal.HistoricalQuote(Instrument);
            m_oED = new ElliottWaveDefinition();
            m_oCache = new List<Tuple<DateTime, List<HistoricalQuoteDetails>, int>>();
        }

        public decimal StandardDeviation
        {
            get
            {
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

        public Instrument Instrument
        {
            get
            {
                return this.m_oH.Instrument;
            }
        }

        public decimal ChangeSince(DateTime From)
        {
            return (GetValueOfDate(DateTime.Now) / GetValueOfDate(From)) * 100;
        }

        public decimal GetValueOfDate(DateTime DateOfValue)
        {
            decimal AmountValue;

            if (DateOfValue >= DateTime.Now.Date)
            {
                if (ImperaturGlobal.Quotes.Where(q => q.Symbol.Equals(m_oH.Instrument.Symbol)).Count() > 0)
                {
                    AmountValue = ImperaturGlobal.Quotes.Where(q => q.Symbol.Equals(m_oH.Instrument.Symbol)).First().LastTradePrice.Amount;
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

        public List<TradingRecommendation> GetTradingRecommendations()
        {
            List<TradingRecommendation> Recommendations = new List<TradingRecommendation>(); ;
            //Recommendations.Add(get)
            //start with Elliot
            bool bReccomend;
            int[] Intervals = { 20, 50, 100, 180 };

            TradingRecommendation Recommendation = new TradingRecommendation();
            try
            {
                decimal tp = QuoteFromInstrument.LastTradePrice.Amount;

            }
            catch(Exception ex)
            {
                int gg = 0;
                return Recommendations;
            }
            /*
            foreach (int Interval in Intervals)
            {
                bReccomend = RangeConvergeWithElliotForBuy(Interval, out Recommendation);
                if (bReccomend)
                {
                   // Recommendations.Add(Recommendation);
                    break;
                }
            }*/
            Recommendations.Add(GetTradingRecommendationForBollinger());
            return Recommendations;
        }

        private enum BollingerBandIndication
        {

        }

        private TradingRecommendation GetTradingRecommendationForBollinger()
        {
            int[] Intervals = { 20, 50, 100, 180 };
            double[] Multiplies = new double[] { 2};

                
            TradingRecommendation Recommendation = new TradingRecommendation();
            foreach (int Interval in Intervals)
            {
                List<List<double>> oB = BollingerForRange(DateTime.Now.AddDays(-Interval), DateTime.Now, 20, Multiplies);
                //Now create the pricerange list as well
                var PriceInfo = GetDataForRange(DateTime.Now.AddDays(-Interval), DateTime.Now);
                if (PriceInfo.Count() < 2)
                {
                    continue;
                }
                var BDiffVariable =  oB.Where(bc=>bc.Count() > 1).Select(b => b[0] - b[2]).ToArray();
                if (BDiffVariable == null)
                {
                    continue;
                }

                double SlopeTrend = Fit.Line(
                    PriceInfo.Select((s, i2) => new { i2, s }).Select(t => Convert.ToDouble(t.i2)).ToArray(),
                     PriceInfo.Select(y => Convert.ToDouble(y.Close)).ToArray()
                    ).Item2;

                double StandarDevForPercentB = BDiffVariable.StandardDeviation();
                if (BDiffVariable[BDiffVariable.Length-1] < StandarDevForPercentB && SlopeTrend > 0 && Convert.ToDouble(QuoteFromInstrument.LastTradePrice.Amount) <= oB[2][oB.Count()-1])
                {
                    return new TradingRecommendation(
                        Instrument,
                        QuoteFromInstrument.LastTradePrice,
                        ImperaturGlobal.GetMoney(0, Instrument.CurrencyCode),
                        DateTime.Now,
                        DateTime.Now,
                        TradingForecastMethod.Bollinger
                    );
                }
                if (BDiffVariable[BDiffVariable.Length - 1] > StandarDevForPercentB && SlopeTrend < 0 && Convert.ToDouble(QuoteFromInstrument.LastTradePrice.Amount) > oB[2][oB.Count() - 1])
                {
                    return new TradingRecommendation(
                        Instrument,
                        ImperaturGlobal.GetMoney(0, Instrument.CurrencyCode),
                        QuoteFromInstrument.LastTradePrice,
                        DateTime.Now,
                        DateTime.Now,
                        TradingForecastMethod.Bollinger
                    );
                }

            }
            return new TradingRecommendation();
        }


        public Quote QuoteFromInstrument
        {
            get
            {
                return ImperaturGlobal.Quotes.Where(i => i.Symbol.Equals(Instrument.Symbol)).First();
            }
        }
        /// <summary>
        /// Returns an array of the indexes(integer) of an list
        /// </summary>
        /// <param name="L"></param>
        /// <returns>array of ints</returns>
        private int[] GetIntIndexFromList(List<object> L) 
        {
            return L.Select((s, i2) => new { i2, s }).Select(t => t.i2).ToArray();
        }
        /// <summary>
        /// Returns an array of the indexes converted to double of an list
        /// </summary>
        /// <param name="L"></param>
        /// <returns>array of doubles</returns>
        private double[] GetDoubleIndexFromList(List<object> L)
        {
            return L.Select((s, i2) => new { i2, s }).Select(t => Convert.ToDouble(t.i2)).ToArray();
        }

        private TradingRecommendation GetTradingRecommendationFromWaves(List<ConfirmedElliottWave> ConfirmedWaves, DateTime Start, DateTime End )
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
                ConfirmedElliottWave MaxConfirmedWaveObject = ConfirmedWaves.Where(f => f.Wave.SourceIndex.Equals(
                    ConfirmedWaves.Where(cw => cw.WaveNumber.Equals(MaxConfirmedWave)).Select(e => e.Wave.SourceIndex).Max()
                    )).First();
                double x = MaxConfirmedWaveObject.Wave.SourceIndex / 4;
                //DateTime ActionDateToTrade = Start.AddDays((int)x);
                //Last wave for buy!
                return new TradingRecommendation(
                    Instrument,
                    ImperaturGlobal.GetMoney(Convert.ToDecimal(MaxConfirmedWaveObject.Wave.End), Instrument.CurrencyCode),
                    ImperaturGlobal.GetMoney(0, Instrument.CurrencyCode),
                    Start.AddDays((int)x),
                    Start.AddDays((int)x),
                    TradingForecastMethod.Elliott
                );
            }

                

            ElliottWave oE = m_oED.ElliotWaveDefinitions.Where(ed => ed.WaveNumber.Equals(MaxConfirmedWave + 1)).First();

            List<Wave> ToCompare = new List<Wave>();

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

            return new TradingRecommendation(Instrument,
                ImperaturGlobal.GetMoney(
                oE.Momentum.Equals(Momentum.Negative) ? Convert.ToDecimal(ConfirmedWaves.Where(c => c.WaveNumber.Equals(MaxConfirmedWave)).Last().Wave.End + (PredictedLenght * SlopeAvg)) : 0, Instrument.CurrencyCode),
                ImperaturGlobal.GetMoney(
                oE.Momentum.Equals(Momentum.Positive) ? Convert.ToDecimal(ConfirmedWaves.Where(c => c.WaveNumber.Equals(MaxConfirmedWave)).Last().Wave.End + (PredictedLenght * SlopeAvg)) : 0, Instrument.CurrencyCode),
                ActionDateToTrade,
                ActionDateToTrade,
                TradingForecastMethod.Elliott
            );

        }


        public bool RangeConvergeWithElliotForBuy(DateTime StartDate, DateTime EndDate, out TradingRecommendation TradingRecommendation)
        {
            List<Wave> oWaves = GetListOfWavesFromRange(StartDate, EndDate);
            List<ConfirmedElliottWave> oConfirmedWaves;
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
            List<double> oSample = GetDataForRange(Start, End)
                .Select(h => Convert.ToDouble(h.Close)).ToList();

            return Statistics.MovingAverage(oSample, oSample.Count()).ToList();
        }

        private List<double> MovingAverage(List<double> Sample)
        {
            return Statistics.MovingAverage(Sample, Sample.Count()).ToList();
        }

        private List<HistoricalQuoteDetails> GetExternalDataForRange(DateTime Start, DateTime End, int Interval)
        {
            //fix the dates, since google keeps sending to much
            GoogleHistoricalDataInterpreter g = new GoogleHistoricalDataInterpreter();
            return g.GetHistoricalDataWithInterval(m_oH.Instrument, m_oH.Exchange, Start, Interval).HistoricalQuoteDetails
                .Where(h => h.Date >= Start && h.Date <= End)
                .OrderBy(x => x.Date)
                .ToList();
        }

        private List<HistoricalQuoteDetails> GetCachedDataForRange(DateTime Start, DateTime End, int Interval)
        {
            /*
            //start by removing obselete cacheobjects
            m_oCache = m_oCache.Where(x => x.Item1.AddSeconds(CacheSeconds).CompareTo(DateTime.Now) > 0).ToList();
            //find matching daterange and interval
            //if range exists in cached range and interval exists return this
            if (
                m_oCache.Exists(x => x.Item2.Where(h=>h.Date>= Start).Count() > 0
                && x.Item2.Where(h => h.Date <= End).Count() > 0
                && x.Item3.Equals(Interval)))
            {
                return m_oCache.Where(x => x.Item2.Where(h => h.Date >= Start).Count() > 0
                && x.Item2.Where(h => h.Date <= End).Count() > 0
                && x.Item3.Equals(Interval)).First().Item2;
            }
            */
            m_oCache.Add(new Tuple<DateTime, List<HistoricalQuoteDetails>, int>(DateTime.Now, GetExternalDataForRange(Start, End, Interval), Interval));
            return m_oCache.Last().Item2;

        }

        private int GetIntervalFromDateRange(DateTime Start, DateTime End)
        {
            int Interval = 0;

            if ((int)(End - Start).TotalDays <= 1)
            {
                Interval = GoogleHistoricalDataInterpreter.MINUTEINSECONDS * 10;
            }
            else if ((int)(End - Start).TotalDays <= 31)
            {
                Interval = GoogleHistoricalDataInterpreter.HOURINSECONDS;
            }
            return Interval;

        }

        public List<HistoricalQuoteDetails> GetDataForRange(DateTime Start, DateTime End)
        {
            int Interval = GetIntervalFromDateRange(Start, End);
            
            if (Interval > 0)
            {
                return GetCachedDataForRange(Start, End, Interval);
            }
                       

            List<HistoricalQuoteDetails> oH = m_oH.HistoricalQuoteDetails
                    .Where(h => h.Date.Date >= Start.Date && h.Date.Date <= End.Date)
                    .OrderBy(x => x.Date)
                    .ToList();

            return oH;
        }
        /// <summary>
        /// Returns a list of lists of double:
        /// List of doubles acting as a centerline with a windowsize of variable Period.
        /// List of doubles containing points for the upperband, with a standard deviation for each point plus times the Mulitply.
        /// List of doubles containing points for the lowerband, with a standard deviation for each point minus times the Mulitply.
        /// </summary>
        /// <param name="Start">The start of the range to get data from.</param>
        /// <param name="End">The end of the range to get data from.</param>
        /// <param name="Period">The period to calculate the moving average window. Default is 20.</param>
        /// <param name="Multiply">The multiply for the upper and lower points. Default is 2.</param>
        /// <returns>List of List of doubles, first the upper, then the center and last is the lower</returns>
        public List<List<double>> StandardBollingerForRange(DateTime Start, DateTime End, int Period =20, double Multiply = 2)
        {
            return BollingerForRange(Start, End, Period, new double[] { Multiply});
        }
        private List<List<double>> BollingerForRange(DateTime Start, DateTime End, int Period, double[] Multiplies)
        {
            //DateTime StartCorrectedForSma = Start.AddDays(-20);
            List<List<double>> BollingerBands = new List<List<double>>();
            double[] PriceArray;// = GetDataForRange(StartCorrectedForSma, End).Select(q => Convert.ToDouble(q.Close)).ToArray();
            //get system calculated interval
            
            int Interval = GetIntervalFromDateRange(Start, End);
            int skip = 0;
            if (Interval > 0)
            {
                //small intervalls add the desired amount of time and then skip at the end
                PriceArray = GetCachedDataForRange(Start.AddSeconds(-(Interval * Period*2)), End, Interval).Select(q => Convert.ToDouble(q.Close)).ToArray();
                //double[] PriceArray2 = GetCachedDataForRange(Start, End, Interval).Select(q => Convert.ToDouble(q.Close)).ToArray();
                skip = PriceArray.Count()- GetCachedDataForRange(Start, End, Interval).Select(q => Convert.ToDouble(q.Close)).Count();
            }
            else
            {
                int ExtendedPeriod = 0;
                int AddDays = 0;
                while (ExtendedPeriod < 20)
                {
                    AddDays++;

                    ExtendedPeriod = ImperaturGlobal.BusinessDaysUntil(Start.AddDays(-AddDays), Start);
                }
                
                PriceArray = GetDataForRange(Start.AddDays(-AddDays), End).Select(q => Convert.ToDouble(q.Close)).ToArray();
                //double[] PriceArray2 = GetDataForRange(Start, End).Select(q => Convert.ToDouble(q.Close)).ToArray();
                skip = PriceArray.Count() - GetDataForRange(Start, End).Select(q => Convert.ToDouble(q.Close)).Count();
            }
            
            //cant compute std on small amounts of data
            if (PriceArray.Count() < 2)
            {
                return BollingerBands;
            } 

            double[] CenterLine = Statistics.MovingAverage(
                PriceArray
                , Period).ToArray();

            double[] CenterLine2 = MymMoving(PriceArray, Period);

            double[] StandardDevation = GetStandardDeviationPointsOfCenterLine(PriceArray, Period);
            
            BollingerBands.Add(CenterLine.Skip(skip).ToList());
            for (int i = 0; i < Multiplies.Length; i++)
            {
                BollingerBands.Add(GetExtendedBandOfCenterLine(CenterLine, StandardDevation.ToArray(), Multiplies[i], true).Skip(skip).ToList());
                BollingerBands.Add(GetExtendedBandOfCenterLine(CenterLine, StandardDevation.ToArray(), Multiplies[i], false).Skip(skip).ToList());
            }
            return BollingerBands;

        }
        private double[] MymMoving(double[] Input, int period)
        {
            List<double> sma = new List<double>();
            int offset = 0;
            foreach (var Close in Input)
            {
                if (offset >= period)
                {
                    int NewPeriod = (Input.Count() - offset < period) ? Input.Count() - offset : period;
                    sma.Add(Convert.ToDouble(Input.Skip(offset-period).Take(NewPeriod).Sum() / NewPeriod));
                }
                else
                { 
                    sma.Add(double.NaN);
                }
                offset++;
            }

            //foreach (var Close in Input)
            //{
            //    int divider = (offset > Input.Count() - period) ? Input.Count() - offset : period;
            //    sma.Add(Convert.ToDouble(Input.Skip(offset).Take(period).Sum() / divider));
            //    offset++;
            //}
            return sma.ToArray();
        }

        /// <summary>
        /// Calculates either the upper or lowerband according to the boolean direction
        /// </summary>
        /// <param name="CenterLine">The centerline to work from</param>
        /// <param name="StandardDevationPoints">The standard deviation points from the centerlines data</param>
        /// <param name="Multiply">The multiplier of the standard devation to add</param>
        /// <param name="Direction">true is upper, false is lower</param>
        /// <returns>Array of double</returns>
        private double[] GetExtendedBandOfCenterLine(double[] CenterLine, double[] StandardDevationPoints, double Multiply, bool Direction)
        {
            
            return CenterLine.Select((s, i2) => new { i2, s })
                    .Select(t => t.s + (!double.IsNaN(StandardDevationPoints[t.i2]) ?
                    ((
                    Direction? 
                    StandardDevationPoints[t.i2] : -StandardDevationPoints[t.i2]
                    )
                    * Multiply
                    )
                    :
                    1))
                    .ToArray();
        }
        /// <summary>
        /// Returns a list of doubles containing the standard devation for each point of CenterLine, based on the Window size
        /// </summary>
        /// <param name="CenterLine">Array of doubles</param>
        /// <param name="Window">The window size to calculate the standard deviation from</param>
        /// <returns>Array of double</returns>
        private double[] GetStandardDeviationPointsOfCenterLine(double[] CenterLine, int Window)
        {
            List<double> StdList = new List<double>();
            int skip = 0;
            int take = 1;
            for (int i = 1; i < CenterLine.Length; i++)
            {
                take = i;
                if (take > Window)
                    take = Window;
                StdList.Add(CenterLine.Skip(skip).Take(take).StandardDeviation());
                skip++;
            }
            //add items where there are no possibility to calculate the standard deviation
            //remove the NaN
            double FirstRealValue = StdList.Where(x => !double.IsNaN(x)).First();
            /*
            for (int i = 0; i < StdList.Count-1; i++)
            {
                if (!double.IsNaN(StdList[i]))
                {

                }
            }*/

            while (CenterLine.Length > StdList.Count && StdList.Count > 0)
            {
                StdList.Insert(0, StdList[0]);
            }
            return StdList.Select(x=> !double.IsNaN(x) ? x : FirstRealValue).ToArray();
        }

        private double[] GetStandardDeviationPointsOfCenterLine2(double[] CenterLine, int Window)
        {
            List<double> StdList = new List<double>();
            int skip = 0;
            int take = 1;
            for (int i = 1; i < CenterLine.Length; i++)
            {
                take = i;
                if (take > Window)
                    take = Window;
                StdList.Add(CenterLine.Skip(skip).Take(take).StandardDeviation());
                skip++;
            }
            //add items where there are no possibility to calculate the standard deviation
            //remove the NaN
            double FirstRealValue = StdList.Where(x => !double.IsNaN(x)).First();
            /*
            for (int i = 0; i < StdList.Count-1; i++)
            {
                if (!double.IsNaN(StdList[i]))
                {

                }
            }*/

            while (CenterLine.Length > StdList.Count && StdList.Count > 0)
            {
                StdList.Insert(0, StdList[0]);
            }
            return StdList.Select(x => !double.IsNaN(x) ? x : FirstRealValue).ToArray();
        }
        public List<Tuple<DateTime, VolumeIndicator>> GetRangeOfVolumeIndicator(DateTime Start, DateTime End)
        {
            List<Tuple<DateTime, VolumeIndicator>> oVolumeIndicatorData = new List<Tuple<DateTime, VolumeIndicator>>();
            
            DateTime StartWithOffset = Start;
            if ((int)(End - Start).TotalDays < 20)
            {
                StartWithOffset = Start.AddDays(-(20 - (int)(End - Start).TotalDays));
            }
            var HistData = GetDataForRange(StartWithOffset, End);

            double[] VolumeList = HistData.Select(x => Convert.ToDouble(x.Volume)).ToArray();
            double StdForVolume = VolumeList.StandardDeviation();
            double StdForPrice = HistData.Select(x => Convert.ToDouble(x.Close)).ToArray().StandardDeviation();

            CubicSpline oCSTotalData = CubicSpline.InterpolateNaturalSorted(
            VolumeList.Select((s, i2) => new { i2, s })
            .Select(t => Convert.ToDouble(t.i2)).ToArray(),
             VolumeList);

            
            var ListOfDiff = VolumeList.Select((s, i2) => new { i2, s })
                .ToList().Select(f => oCSTotalData.Differentiate(Convert.ToDouble(f.i2))).ToArray();

            int i = 0;
            foreach (var hqd in HistData)
            {

                VolumeIndicator oVi = new VolumeIndicator();
                bool bHighVolume = (ListOfDiff[i] > 0 && VolumeList[i] > StdForVolume);
                bool bLowVolume = (ListOfDiff[i] <= 0 && VolumeList[i] < StdForVolume);
                bool bHighRange = (Convert.ToDouble(hqd.Close - hqd.Open) > StdForPrice);
                bool bLowRange = (-Convert.ToDouble(hqd.Open - hqd.Close) > StdForPrice);
                bool bUpBars = (hqd.Close > hqd.Open);
                bool bNeutralBars = (hqd.Close.Equals(hqd.Open));

                if (bHighVolume && bHighRange && bUpBars && !bNeutralBars)
                {
                    oVi.VolumeIndicatorType = VolumeIndicatorType.VolumeClimaxUp;
                }
                else if(bHighVolume && bHighRange && !bUpBars && !bNeutralBars)
                {
                    oVi.VolumeIndicatorType = VolumeIndicatorType.VolumeClimaxDown;
                }
                else if (bHighVolume && bHighRange && bNeutralBars)
                {
                    oVi.VolumeIndicatorType = VolumeIndicatorType.VolumeClimaxPlusHighVolumeChurn;
                }
                else if (bHighVolume && !bHighRange )
                {
                    oVi.VolumeIndicatorType = VolumeIndicatorType.HighVolumeChurn;
                }
                else if(!bHighVolume && bLowVolume)
                {
                    oVi.VolumeIndicatorType = VolumeIndicatorType.LowVolume;
                }
                else
                {
                    oVi.VolumeIndicatorType = VolumeIndicatorType.Unknown;
                }
                oVi.Strength = 100;
                oVolumeIndicatorData.Add(new Tuple<DateTime, VolumeIndicator>(hqd.Date, oVi));

                i++;
            }
            return oVolumeIndicatorData;
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

    /*
    public struct TradingRecommendation
    {
        public decimal? SellAtPrice;
        public decimal? BuyAtPrice;
        public DateTime? PredictedBuyDate;
        public DateTime? PredictedSellDate;
    }*/
    
    //public class ElliotWaveDefinition
    //{
    //    /*
    //    * Wave
    //   Classical Relations between Waves
    //   1
    //   -
    //   2
    //   0.382, 0.5 or 0.618 of Wave 1 length
    //   3
    //   1.618 or 2.618 of Wave 1 length
    //   4
    //   0.382 or 0.5 of Wave 1 length
    //   5
    //   0.382, 0.5 or 0,618 of Wave 1 length
    //   A
    //   0.382, 0.5 or 0,618 of Wave 1 length
    //   B
    //   0.382 or 0.5 of Wave A length
    //   C
    //   1.618, 0.618 or 0.5 of Wave A length

    //    Wave 2 cannot retrace more than 100% of Wave 1.
    //    Wave 3 can never be the shortest of waves 1, 3, and 5.
    //    Wave 4 can never overlap Wave 1.
    //    * 
    //    */


    //    private List<ElliotWave> m_oElliotWaveDefintion;
    //    private decimal m_oOffsetAllowed;

    //    public ElliotWaveDefinition()
    //    {
    //        m_oOffsetAllowed = 0.1m;
    //        m_oElliotWaveDefintion = new List<ElliotWave>();
    //        //first
    //        m_oElliotWaveDefintion.Add(
    //            new ElliotWave
    //            {
    //                Momentum = Momentum.Negative,
    //                WaveNumber = 1
    //            }
    //            );
    //        //second
    //        m_oElliotWaveDefintion.Add(
    //        new ElliotWave
    //        {
    //            Momentum = Momentum.Positive,
    //            WaveNumber = 2,
    //            RatioToWave = new List<Tuple<int, double>>
    //            {
    //                new Tuple<int, double>(1, 0.382),
    //                new Tuple<int, double>(1, 0.5),
    //                new Tuple<int, double>(1, 0.618),
    //            }
    //        }
    //        );
    //        //third
    //        m_oElliotWaveDefintion.Add(
    //            new ElliotWave
    //            {
    //                Momentum = Momentum.Negative,
    //                WaveNumber = 3,
    //                RatioToWave = new List<Tuple<int, double>>
    //                {
    //                                new Tuple<int, double>(1, 2.618),
    //                                new Tuple<int, double>(1, 1.618),
    //                }
    //            }
    //            );
    //        //Fourth
    //        m_oElliotWaveDefintion.Add(
    //        new ElliotWave
    //        {
    //            Momentum = Momentum.Positive,
    //            WaveNumber = 4,
    //            RatioToWave = new List<Tuple<int, double>>
    //            {
    //                                        new Tuple<int, double>(1, 0.382),
    //                                        new Tuple<int, double>(1, 0.5),
    //            }
    //        }
    //        );
    //        //Fifth
    //        m_oElliotWaveDefintion.Add(
    //        new ElliotWave
    //        {
    //            Momentum = Momentum.Negative,
    //            WaveNumber = 5,
    //            RatioToWave = new List<Tuple<int, double>>
    //            {
    //                                        new Tuple<int, double>(1, 0.382),
    //                                        new Tuple<int, double>(1, 0.5),
    //                                        new Tuple<int, double>(1, 0.618),
    //            }
    //        }
    //        );
    //    }

    //    public List<ElliotWave> ElliotWaveDefinitions
    //    {
    //        get { return m_oElliotWaveDefintion; }
    //    }

    //    private bool EvaluteWave(List<Wave> Waves, int WaveIndexToEvalute, ElliotWave ElliotWaveToUse)
    //    {
    //        bool isElliotWaveDef;
    //        if (!ElliotWaveToUse.Momentum.Equals(Waves[WaveIndexToEvalute].Momentum))
    //        {
    //            return false;
    //        }
    //        if (ElliotWaveToUse.RatioToWave != null && ElliotWaveToUse.RatioToWave.Count() > 0)
    //        {
    //            bool bRatioPassed = false;
    //            foreach (Tuple<int, double> WaveDef in ElliotWaveToUse.RatioToWave)
    //            {
    //                if (WaveIndexToEvalute - ElliotWaveToUse.WaveNumber - WaveDef.Item1 < 0)
    //                {
    //                    return false;
    //                }
    //                Wave ToCompare = Waves[WaveIndexToEvalute - ElliotWaveToUse.WaveNumber - WaveDef.Item1];
    //                if (IsWaveElliot(ToCompare.Length, Waves[WaveIndexToEvalute].Length, new double[] { WaveDef.Item2 }))
    //                {
    //                    bRatioPassed = true;
    //                    break;
    //                }
    //            }
    //            isElliotWaveDef = bRatioPassed;
    //        }
    //        else
    //        {
    //            isElliotWaveDef =  true;
    //        }
    //        return isElliotWaveDef;
    //    }
    //    private bool IsWaveElliot(decimal Wave1Lenght, decimal Wave2Lenght, decimal[] WaveDef)
    //    {
    //        decimal divider = Wave2Lenght / Wave1Lenght;
    //        foreach (decimal oDef in WaveDef)
    //        {
    //            if (divider >= Offset(oDef, m_oOffsetAllowed, false) && divider <= Offset(oDef, m_oOffsetAllowed, true))
    //            {
    //                return true;
    //            }
    //        }
    //        return false;
    //    }

    //    public bool IsWaveElliot(double Wave1Lenght, double Wave2Lenght, double[] WaveDef)
    //    {
    //        double divider = Wave2Lenght / Wave1Lenght;
    //        foreach (double oDef in WaveDef)
    //        {
    //            if (divider >= Offset(Convert.ToDouble(m_oOffsetAllowed), oDef, false) && divider <= Offset(Convert.ToDouble(m_oOffsetAllowed), oDef, true))
    //            {
    //                return true;
    //            }
    //        }
    //        return false;
    //    }

    //    private decimal Offset(decimal Offset, decimal Value, bool Add)
    //    {
    //        return Add ? Value - Value * Offset : Value + Value * Offset;
    //    }

    //    private double Offset(double Offset, double Value, bool Add)
    //    {
    //        return Add ? Value + (Value * Offset) : Value - (Value * Offset);
    //    }


    //    public bool FindElliottDefinitioninWaves(List<Wave> Waves, out int LastWaveNumber, out List<ConfirmedElliotWave> ConfirmedElliotWaves)
    //    {
    //        List<Wave> oElliotConfirmedWaves = new List<Wave>();
    //        ConfirmedElliotWaves = new List<ConfirmedElliotWave>();
    //        LastWaveNumber = 0;
    //        foreach (var ElliotDef in m_oElliotWaveDefintion)
    //        {
                
    //            oElliotConfirmedWaves = Waves.Where(wl => EvaluteWave(Waves, Waves.FindIndex(a => a.Equals(wl)), ElliotDef)).ToList();
    //            if (oElliotConfirmedWaves.Count() == 0)
    //            {
    //                return false;
    //            }
    //            else
    //            {
    //                LastWaveNumber = ElliotDef.WaveNumber;
    //                ConfirmedElliotWaves.AddRange(oElliotConfirmedWaves.Select(w => new ConfirmedElliotWave
    //                {
    //                    Wave = w,
    //                    WaveNumber = ElliotDef.WaveNumber
    //                }).ToList());
    //            }
    //        }
    //        return true;
            
    //    }
    //}
   
}
