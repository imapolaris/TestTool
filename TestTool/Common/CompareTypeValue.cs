using Seecool.Radar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTSCore.Data.Common
{
    public class CompareTypeValue
    {
        public static bool AreEqual(RadarServiceNetCmds.RadarConfig radarConfig1, RadarServiceNetCmds.RadarConfig radarConfig2)
        {
            if (radarConfig1 == null || radarConfig2 == null)
                return (radarConfig1 == null && radarConfig2 == null);
            return radarConfig1.Name == radarConfig2.Name &&
                radarConfig1.Longitude == radarConfig2.Longitude && radarConfig1.Latitude == radarConfig2.Latitude &&
                radarConfig1.Altitude == radarConfig2.Altitude && radarConfig1.StartScanAngle == radarConfig2.StartScanAngle
                && radarConfig1.StartRange == radarConfig2.StartRange && radarConfig1.Range == radarConfig2.Range && radarConfig1.OffsetAngle == radarConfig2.OffsetAngle;
        }

        public static bool AreEqual(RadarServiceNetCmds.RadarStatus radarStatus1, RadarServiceNetCmds.RadarStatus radarStatus2)
        {
            if (radarStatus1 == null || radarStatus2 == null)
                return (radarStatus1 == null && radarStatus2 == null);
            if(AreEqual(radarStatus1 as RadarServiceNetCmds.RadarConfig, radarStatus2 as RadarServiceNetCmds.RadarConfig))
            {
                return (radarStatus1.RadarCardType == radarStatus2.RadarCardType && radarStatus1.ADBits == radarStatus2.ADBits 
                    && radarStatus1.ScanLineCount == radarStatus2.ScanLineCount && radarStatus1.SampleCount == radarStatus2.SampleCount 
                    && radarStatus1.BearingPulseStatus == radarStatus2.BearingPulseStatus && radarStatus1.ShipHeadingMarkerStatus == radarStatus2.ShipHeadingMarkerStatus
                    && radarStatus1.TriggerStatus == radarStatus2.TriggerStatus && radarStatus1.InterruptStatus == radarStatus2.InterruptStatus
                    && radarStatus1.RoundPerMinute == radarStatus2.RoundPerMinute && radarStatus1.PulseRepetitionRate == radarStatus2.PulseRepetitionRate
                    && radarStatus1.BPPerSweep == radarStatus2.BPPerSweep
                    );
            }
            return false;
        }

    }
}
