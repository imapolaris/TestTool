using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTSCore.Data.Common;

namespace VTSCore.Layers.Tracks
{
    public class SeecoolRadarConfig: ConfigBase
    {
        public SeecoolRadarConfig()
            : base()
        {
            Port = 60003;
        }
    }
}
