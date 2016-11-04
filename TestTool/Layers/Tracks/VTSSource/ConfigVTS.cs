using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTSCore.Data.Common;

namespace VTSCore.Layers.Tracks
{
    public class NorcontrolVTSConfig : ConfigBase
    {
        public NorcontrolVTSConfig()
            : base()
        {
            //Ip = "192.168.9.89";//天津Norcontrol VTS
            Port = 45312;
        }
    }

    public class AtlasVTSConfig : ConfigBase
    {
        public AtlasVTSConfig()
            : base()
        {
            //Ip = "192.168.9.106";//（广州ATL1）
            Port = 8205;
        }
    }

    public class Atlas2VTSConfig : ConfigBase
    {
        public Atlas2VTSConfig()
            : base()
        {
            //Ip = "172.29.100.38";//上海ATLAS VTS
            Port = 9983;
        }
    }

    public class HittVTSConfig : ConfigBase
    {
        public HittVTSConfig()
            : base()
        {
            //Ip = "192.168.9.89";//洋山港HITT VTS 数据接入
            Port = 38889;
        }
    }

    public class SofrelogVTSConfig : ConfigBase
    {
        public SofrelogVTSConfig()
            : base()
        {
            //Ip = "192.168.9.106";//天津SOF
            Port = 21603;
        }
    }
}
