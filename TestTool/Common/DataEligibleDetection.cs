using System;

namespace VTSCore.Data.Common
{
    public class DataEligibleDetection
    {
        public static bool GetIpEndPoint(string value)
        {
            try
            {
                string begin = value.Substring(0, 6);
                if (begin == "tcp://")
                {
                    value = value.Substring(6, value.Length - 6);
                    string[] data = value.Split(':');
                    if (data.Length == 2)
                    {
                        getPort(data[1]);
                        if (IsEffectIp(data[0]))
                            return true;
                    }
                }
            }
            catch { }
            return false;
        }

        public static bool GetIpPort(string setting, out string ip, out int port)
        {
            try
            {
                char[] separator = new char[] { ',' };
                string[] data = setting.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                if (data.Length == 2)
                {
                    ip = data[0];
                    port = getPort(data[1]);
                    return true;
                }
            }
            catch { }
            ip = "";
            port = 0;
            return false;
        }

        public static string[] GetStringArray(string setting)
        {
            char[] separator = new char[] { ',' };
            return setting.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        }

        public static bool IsEffectIp(string ip)
        {
            System.Net.IPAddress address;
            if(System.Net.IPAddress.TryParse(ip, out address))
            {
                string[] data = ip.Split('.');
                if(data.Length == 4)
                    return true;
            }
            return false;
        }

        public static bool IsEffectPort(int port)
        {
            return port > 0 && port < 65535;
        }

        static int getPort(string portString)
        {
            int iPort = int.Parse(portString);
            if (!IsEffectPort(iPort))
                throw new System.InvalidOperationException("端口配置异常!");
            return iPort;
        }
    }
}