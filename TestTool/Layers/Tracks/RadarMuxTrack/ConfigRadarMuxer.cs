using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTSCore.Data.Common;
using VTSCore.Layers.Common;

namespace VTSCore.Layers.Tracks
{
    public class RadarMuxerConnection
    {
        public string SubEndpoint { get; set; }
        public string RpcEndpoint { get; set; }
        public RadarMuxerConnection()
        {
            SubEndpoint = @"tcp://127.0.0.1:16701";
            RpcEndpoint = @"tcp://127.0.0.1:16702";
        }
    }
}