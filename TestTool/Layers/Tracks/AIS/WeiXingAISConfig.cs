using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTSCore.Data.Common;

namespace VTSCore.Layers.Tracks
{
    public class WeiXingAISConfig: ConfigBase
    {
        public WeiXingAISConfig()
            : base()
        {
            //Ip = "198.15.1.75";//山东卫星AIS接入
            Port = 6001;
        }
    }
}
