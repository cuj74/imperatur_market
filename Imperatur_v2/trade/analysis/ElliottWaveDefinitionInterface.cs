using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_v2.trade.analysis
{
    interface IElliottWaveDefinition
    {
        List<ElliottWave> ElliotWaveDefinitions { get; }
        bool IsWaveElliot(double Wave1Lenght, double Wave2Lenght, double[] WaveDef);
        bool FindElliottDefinitioninWaves(List<Wave> Waves, out int LastWaveNumber, out List<ConfirmedElliottWave> ConfirmedElliotWaves);
    }
}
