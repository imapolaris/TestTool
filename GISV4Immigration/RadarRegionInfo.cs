using Seecool.Radar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GISV4Immigration
{
    public class RadarRegionInfo
    {
        public RadarRegionInfo(RadarRegion region, string relateRadar)
        {
            Region = region;
            RelateRadar = relateRadar;
        }

        public string Name { get { return Region.Name; } }

        public RadarRegion Region;
        public string RelateRadar;

        public bool AllRadar { get { return string.IsNullOrEmpty(RelateRadar); } }
    }
}
