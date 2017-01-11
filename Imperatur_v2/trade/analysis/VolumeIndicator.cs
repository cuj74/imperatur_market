using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_v2.trade.analysis
{
    public enum VolumeIndicatorType
    {
        /*
Volume Climax Up – high volume, high range, up bars (red)
Volume Climax Down – high volume, high range, down bars (white)
High Volume Churn – high volume, low range bars (green, PaintBar blue)
Low Volume – low volume bars (yellow)
Volume Climax plus High Volume Churn – both the above conditions (magenta)
*/
        VolumeClimaxUp,
        VolumeClimaxDown,
        HighVolumeChurn,
        LowVolume,
        VolumeClimaxPlusHighVolumeChurn,
        Unknown

    }

    public class VolumeIndicator
    {
        public VolumeIndicatorType VolumeIndicatorType;
        public int Strength;

    }
}
