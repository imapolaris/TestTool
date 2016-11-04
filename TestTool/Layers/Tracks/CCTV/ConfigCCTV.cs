using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using VTSCore.Data.Common;

namespace VTSCore.Layers.Tracks.CCTV
{
    public class ConfigCCTV
    {
        public string Ip { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
        public int Bandwidth { get; set; }
        public ConfigCCTV()
        {
            Ip = "127.0.0.1";
            User = "usnt";
            Pass = "usnt";
            Bandwidth = 2000000;
        }
        public bool Equal(ConfigCCTV cctv)
        {
            return cctv != null && cctv.Ip == Ip && cctv.Bandwidth == cctv.Bandwidth;
        }

        public ConfigCCTV Clone()
        {
            return (ConfigCCTV)MemberwiseClone();
        }
    }
}
