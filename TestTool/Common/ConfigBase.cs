using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace VTSCore.Data.Common
{
    public class ConfigBase
    {
        public string Ip;
        public int Port;

        public ConfigBase()
        {
            Ip = "127.0.0.1";
        }

        public override string ToString()
        {
            return string.Format("{0},{1}", Ip, Port);
        }
    }
}
