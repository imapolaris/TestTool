using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTSCore.Layers.Common;

namespace VTSCore.Layers.Radar
{
    public class RadarConnection: ConfigConnection
    {
        public RadarConnection():base()
        {
            Port = 60008;
            RpcEndPoint = @"tcp://127.0.0.1:38047";
        }

        public RadarConnection(ConfigConnection config)
        {
            this.Ip = config.Ip;
            this.Port = config.Port;
            this.RpcEndPoint = config.RpcEndPoint;
            this.ColorTableIndex = config.ColorTableIndex;
            this.IsEnable = config.IsEnable;
        }

        public override ConfigConnection Clone()
        {
            return (ConfigConnection)MemberwiseClone();
        }
    }
}
