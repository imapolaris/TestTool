using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTSCore.Data.Common;

namespace VTSCore.Layers.Tracks
{
    public class ShanDongGPSConfig: ConfigBase
    {
        public ShanDongGPSConfig():base()
        {
            //Ip = "198.15.1.75";//山东
            Port = 7001;
        }
    }
}
