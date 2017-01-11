using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_v2.trade.analysis
{
    public struct ConfirmedElliottWave
    {
        public Wave Wave;
        public int WaveNumber;
    }
    public struct ElliottWave
    {
        public List<Tuple<int, double>> RatioToWave;
        public Momentum Momentum;
        public int WaveNumber;
    }

    public class ElliottWaveDefinition : IElliottWaveDefinition
    {
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


        private List<ElliottWave> m_oElliotWaveDefintion;
        private decimal m_oOffsetAllowed;

        public ElliottWaveDefinition()
        {
            m_oOffsetAllowed = 0.1m;
            m_oElliotWaveDefintion = new List<ElliottWave>();
            //first
            m_oElliotWaveDefintion.Add(
                new ElliottWave
                {
                    Momentum = Momentum.Negative,
                    WaveNumber = 1
                }
                );
            //second
            m_oElliotWaveDefintion.Add(
            new ElliottWave
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
                new ElliottWave
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
            new ElliottWave
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
            new ElliottWave
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

        public List<ElliottWave> ElliotWaveDefinitions
        {
            get { return m_oElliotWaveDefintion; }
        }

        private bool EvaluteWave(List<Wave> Waves, int WaveIndexToEvalute, ElliottWave ElliotWaveToUse)
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
                isElliotWaveDef = true;
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
        }


        public bool FindElliottDefinitioninWaves(List<Wave> Waves, out int LastWaveNumber, out List<ConfirmedElliottWave> ConfirmedElliotWaves)
        {
            List<Wave> oElliotConfirmedWaves = new List<Wave>();
            ConfirmedElliotWaves = new List<ConfirmedElliottWave>();
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
                    ConfirmedElliotWaves.AddRange(oElliotConfirmedWaves.Select(w => new ConfirmedElliottWave
                    {
                        Wave = w,
                        WaveNumber = ElliotDef.WaveNumber
                    }).ToList());
                }
            }
            return true;

        }
    }
}
