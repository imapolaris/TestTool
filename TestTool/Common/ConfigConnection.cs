using RadarServiceNetCmds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using VTSCore.Data.Common;

namespace VTSCore.Layers.Common
{
    public class ConfigConnection
    {
        public virtual string Ip { get; set; }
        public virtual int Port { get; set; }
        public virtual string RpcEndPoint { get; set; }
        public virtual int ColorTableIndex { get; set; }

        public virtual bool IsEnable { get; set; }

        public ConfigConnection()
        {
            Ip = "127.0.0.1";
            Port = 60000;
            RpcEndPoint = @"tcp://127.0.0.1:60001";
            ColorTableIndex = 0;
            IsEnable = true;
        }
        
        public virtual bool IsSquels(ConfigConnection radar)
        {
            if (Ip == radar.Ip && Port == radar.Port && RpcEndPoint == radar.RpcEndPoint)
                return true;
            else
                return false;
        }

        public virtual ConfigConnection Clone()
        {
            return (ConfigConnection)MemberwiseClone();
        }
    }
}
