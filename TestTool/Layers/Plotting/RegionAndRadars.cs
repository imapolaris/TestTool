using Seecool.Radar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTSCore.Layers.Radar;

namespace VTSCore.Layers.Plotting
{
    public class RegionAndRadars : RadarRegion
    {
        public string[] RadarNames;
        public RegionAndRadars()
        {
        }

        public RegionAndRadars(RadarRegion region, string[] radarNames)
        {
            Name = region.Name;
            IsMask = region.IsMask;
            ManualIdenfity = region.ManualIdenfity;
            PassThrough = region.PassThrough;
            Polygon = region.Polygon;
            RadarNames = radarNames;
        }
    }
}
