using Imperatur_v2.securites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.shared;
using MathNet.Numerics.Interpolation;

namespace Imperatur_v2.trade.analysis
{
    public class SecurityAnalysis : ISecurityAnalysis
    {
        HistoricalQuote m_oH;
        private decimal[] m_oWave2Definition;
        private decimal[] m_oWave3Definition;
        private decimal m_oOffsetAllowed;

        public SecurityAnalysis(Instrument Instrument)
        {
            m_oH = ImperaturGlobal.HistoricalQuote(Instrument);
            m_oWave2Definition =new decimal[]{ 0.382m, 0.5m, 0.618m};
            m_oWave3Definition = new decimal[] { 1.618m, 2.618m };
            m_oOffsetAllowed = 0.1m;
        }

        public decimal StandardDeviation
        {
            get
            {
                Double g = m_oH.HistoricalQuoteDetails.Select(h => Convert.ToDouble(h.Close)).StdDev();
                return Convert.ToDecimal(m_oH.HistoricalQuoteDetails.Select(h => Convert.ToDouble(h.Close)).StdDev());
            }
        }

        public decimal ChangeSince(DateTime From)
        {
            DateTime ClosestDate = m_oH.HistoricalQuoteDetails.Where(h => h.Date >= From).Min(m => m.Date);
            decimal StartValue = m_oH.HistoricalQuoteDetails.Where(h => h.Date.Equals(ClosestDate)).First().Close;
            decimal EndValue = ImperaturGlobal.Quotes.Where(q => q.Symbol.Equals(m_oH.Instrument.Symbol)).First().LastTradePrice.Amount;
            return (EndValue / StartValue) * 100;
        }

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


         /*
        private bool IsWave1AndWave2Elliot(decimal Wave1Lenght, decimal Wave2Lenght)
        {
            //0.382, 0.5 or 0.618 of Wave 1 length
            decimal OffsetAllowed = 0.10m; //10 percent
            decimal divider = Wave2Lenght / Wave1Lenght;
            if (divider >= Offset(0.382m, OffsetAllowed, false) && divider <= Offset(0.382m, OffsetAllowed, true))
            {
                return true;
            }


        }*/
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

        private decimal Offset(decimal Offset, decimal Value, bool Add)
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
        public bool RangeConvergeWithElliotForBuy(int IntervalInDays, out decimal SaleValue)
        {
            SaleValue = -1m;
            //up since interval start? If yes the answer is no.
            if (ChangeSince(DateTime.Now.AddDays(-IntervalInDays)) > 0)
                return false;

            DateTime StartDate = DateTime.Now.AddDays(-IntervalInDays);

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

            List<decimal> SecondPart = GetRangeofDataPoints(StartDate, (int)Math.Truncate(ThreeParts)*2, (int)Math.Truncate(ThreeParts));
            //add the last datapoint
            SecondPart.Add(
                PlotValue(m_oH.HistoricalQuoteDetails
                .Where(h => h.Date >= StartDate)
                .OrderBy(x => x.Date)
                .Select(h => h.Close)
                .ToArray(), ThreeParts*2));

            List<decimal> ThirdPart = GetRangeofDataPoints(StartDate, (int)Math.Truncate(ThreeParts) * 3, (int)Math.Truncate(ThreeParts)*2);
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

            //analyze second part, looking for the positive trends and analyze the lenght, compare it to first wave
            CubicSpline oCs = CubicSpline.InterpolateNatural( 
                                 
                                SecondPart.Select((s, i2) => new { i2, s })
                                .Where(t => t.s == Wave1Value)
                                .Select(t => Convert.ToDouble(t.i2)).ToArray(),

                                 SecondPart.Select(s => Convert.ToDouble(s)).ToArray());

            bool bIsWave2Elliot = false;
            int index = 0;
            int LargestLenght = 0;
            decimal Wave2Value = 0;
            decimal Wave2Lenght = Wave1Lenght;
            foreach (decimal dPoint in SecondPart)
            {
                if (oCs.Differentiate((double) dPoint) > 0)
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
            if (ThirdPart[ThirdPart.Count-1] < Wave2Value &&
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

        public bool RangeConvergeWithElliotForSell(int IntervalInDays)
        {
            return false;
        }

        public decimal StandardDeviationForRange(DateTime Start, DateTime End)
        {
            return Convert.ToDecimal(m_oH.HistoricalQuoteDetails
                .Where(h => h.Date >= Start && h.Date <= End)
                .Select(h => Convert.ToDouble(h.Close)).StdDev());
        }

        #region private methods
        /*private decimal PlotValue(decimal[] DataPoints, decimal Position)
        {
            decimal OriginalPosition = Position;
            if (Position - Math.Truncate(Position) == OriginalPosition)
            {
                return DataPoints[(int)Math.Truncate(Position)];
            }

            if (Convert.ToDecimal(DataPoints.Count()) <= Position || (int)Math.Truncate(Position)==DataPoints.Count() - 1)
            {
                //return the last
                return DataPoints[DataPoints.Count() - 1];
            }
                

            //Find the lowest closes value
            decimal Low = DataPoints[(int)Math.Truncate(Position)];
            decimal High = DataPoints[(int)Math.Truncate(Position)+1];
            return ((High - Low) * Position - Math.Truncate(Position)) + Low;

        }
    */
        private decimal PlotValue(decimal[] DataPoints, decimal Position)
        {
            CubicSpline oCs = CubicSpline.InterpolateNatural(

                               DataPoints.Select((s, i2) => new { i2, s })
                               .Select(t => Convert.ToDouble(t.i2)).ToArray(),

                               DataPoints.Select(s => Convert.ToDouble(s)).ToArray());
            return Convert.ToDecimal(oCs.Interpolate(Convert.ToDouble(oCs)));

        }
        #endregion
    }
}
