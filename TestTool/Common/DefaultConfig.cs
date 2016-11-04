using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTSCore.Data.Common
{
    public class DefaultConfig
    {
        public double Lon { get; set; }
        public double Lat { get; set; }
        public double Scale { get; set; }
    }

    public class DefaultConfigInfo
    {
        public DefaultConfig Default { get; private set; }
        string _path;
        public static DefaultConfigInfo Instance{get;private set;}
        static DefaultConfigInfo()
        {
            Instance = new DefaultConfigInfo();
        }

        private DefaultConfigInfo()
        {
            string path = System.IO.Directory.GetParent(System.Windows.Forms.Application.LocalUserAppDataPath).FullName;
            _path = System.IO.Path.Combine(path, "Default.xml");
            Default = ConfigFile<DefaultConfig>.FromFile(_path);
            if (Default == null)
                Default = new DefaultConfig() { Lon = 121.4887, Lat = 31.2114, Scale = 50000 };
        }

        public bool SaveDefault(double lon, double lat, double scale)
        {
            Default = new DefaultConfig() { Lon = lon, Lat = lat, Scale = scale };
            return ConfigFile<DefaultConfig>.SaveToFile(_path, Default);
        }

    }
}
