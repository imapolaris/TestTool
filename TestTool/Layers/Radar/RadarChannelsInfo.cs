using RadarServiceNetCmds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTSCore.Layers.Radar
{
    class RadarChannelsInfo
    {
        public static bool AreEqual(RadarChannel[] resource1, RadarChannel[] resource2)
        {
            if (resource1 == null || resource2 == null)
                return (resource1 == null || resource1.Length == 0) && (resource2 == null || resource2.Length == 0);
            if(resource1.Length != resource2.Length)
                return false;
            for(int i = 0; i < resource1.Length; i++)
            {
                if (!AreEqual(resource1[i], resource2[i]))
                    return false;
            }
            return true;
        }

        public static bool AreEqual(RadarChannel channel1, RadarChannel channel2)
        {
            if (channel1 == null || channel2 == null)
                return channel1 == channel2;
            return channel1.Name == channel2.Name && channel1.LegacyPort == channel2.LegacyPort && channel1.Filters == channel2.Filters;
        }
        public static RadarChannel[] Clone(RadarChannel[] resource)
        {
            if (resource == null) 
                return null;
            RadarChannel[] channels = new RadarChannel[resource.Length];
            for (int i = 0; i < resource.Length; i++)
                channels[i] = Clone(resource[i]);
            return channels;
        }

        public static RadarChannel Clone(RadarChannel channel)
        {
            return new RadarChannel() { Name = channel.Name, Filters = channel.Filters, LegacyPort = channel.LegacyPort };
        }
    }
}
