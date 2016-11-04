using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using SeeCool.GISFramework.Object;
using SeeCool.GISFramework.Net;
using SeeCool.GISFramework.Util;

namespace SeeCool.GISFramework.SvrFramework
{
    public class AtlasReceiver : VTSPlugin.TcpipReceiver
    {
        StringBuilder _buffer = new StringBuilder(1024);

        public event Action<AtlasData> OnReceivedData;

        public delegate void OnIdentification(int trackID, string name, string callSign, int imo);
        public event OnIdentification OnIdentificationEvent;

        protected override void onConnected()
        {
            base.onConnected();
            _buffer.Length = 0;
        }

        protected override void onRecv(byte[] buf, int len)
        {
            _buffer.Append(Encoding.Default.GetString(buf, 0, len));
            const int maxBufferSize = 0x10000000;
            if (_buffer.Length > maxBufferSize)
                _buffer.Remove(0, _buffer.Length);

            int index = 0;
            while (index < _buffer.Length)
            {
                while (index + 3 <= _buffer.Length && _buffer[index] != '$')
                    index++;

                if (index + 3 <= _buffer.Length)
                {
                    index++;
                    int end = index;
                    while (end + 1 < _buffer.Length && !(_buffer[end] == '\r' && _buffer[end + 1] == '\n'))
                        end++;

                    if (end + 1 < _buffer.Length)
                    {
                        parseMessage(_buffer.ToString(index, end - index));
                        index = end + 2;
                        continue;
                    }
                }
                break;
            }

            _buffer.Remove(0, index);
        }

        private void parseMessage(string msg)
        {
            int index = msg.LastIndexOf('*');
            if (index >= 0)
            {
                string checksumString = msg.Substring(index + 1);
                int checksum = 0;
                if (checksumString.Length >= 2 && hex2number(checksumString.Substring(0, 2), out checksum))
                {
                    string data = msg.Substring(0, index);
                    byte[] bytes = Encoding.Default.GetBytes(data);
                    byte checksum2 = 0;
                    foreach (byte b in bytes)
                        checksum2 ^= b;
                    if (((int)checksum2) == checksum)
                        parseTelegram(data.Split(','));
                }
            }
        }

        private void parseTelegram(string[] comps)
        {
            int telegramID = 0;
            if (comps.Length >= 2 && comps[0] == "PVTS" && hex2number(comps[1], out telegramID))
            {
                switch (telegramID)
                {
                    case 0x02:
                        {
                            int trackID;
                            int lat;
                            int lon;
                            int sog;
                            int cog;
                            int timeStamp;
                            if (hex2number(comps[2], out trackID)
                                && hex2number(comps[5], out lat)
                                && hex2number(comps[6], out lon)
                                && hex2number(comps[7], out sog)
                                && hex2number(comps[8], out cog)
                                && hex2number(comps[10], out timeStamp))
                            {
                                onPositionData(trackID, lat, lon, sog, cog, timeStamp);
                            }
                        }
                        break;
                    case 0x0E:
                        if (comps.Length >= 6)
                        {
                            int trackID;
                            int imo;
                            if (hex2number(comps[2], out trackID) && hex2number(comps[5], out imo))
                                onIdentificationData(trackID, nmeaString(comps[3]), nmeaString(comps[4]), imo);
                        }
                        break;
                }
            }
        }

        private string nmeaString(string str)
        {
            return str.Trim('@').Replace('@', ' ');
        }

        private void onIdentificationData(int trackID, string name, string callSign, int imo)
        {
            if(OnIdentificationEvent != null)
                OnIdentificationEvent(trackID, name, callSign, imo);
        }

        private void onPositionData(int trackID, int lat, int lon, int sog, int cog, int timeStamp)
        {
            AtlasData ad = new AtlasData(trackID);
            ad.Time = DateTime.Now;
            ad.Shape = new GeoPointShape(lon / 600000.0, lat / 600000.0);
            ad.SOG = sog / 10.0;
            ad.COG = cog / 10.0;
            ad.TimeStamp = timeStamp;
            ad.Time = DateTime.Now;
            if(OnReceivedData != null)
                OnReceivedData(ad);
        }

        private bool hex2number(string str, out int result)
        {
            return int.TryParse(str, System.Globalization.NumberStyles.HexNumber, null, out result);
        }
    }
}
