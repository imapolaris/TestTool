using Seecool.Common.Run;
using Seecool.Common.Util;
using Seecool.Messaging.Core;
using Seecool.Messaging.Receiver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTSCore.Layers.Tracks
{
    public class AisReceiver : IConsumable<IData>
    {
        public string Ip { get; private set; }
        public int Port { get; private set; }

        private TcpClientReceiver _receiver;
        private ReceiverSourceRunner<IData> _recvRunner;
        private AutoRepeatRunner _autoRepeatRunner;
        private USNT.DataParser.AISNMEA.Parser _nmeaParser = new USNT.DataParser.AISNMEA.Parser();

        public void SetConfig(string ip, int port)
        {
            if (Ip != ip || Port != port)
            {
                Stop();
                Ip = ip;
                Port = port;
                Start();
            }
        }

        public void Start()
        {
            if (!string.IsNullOrWhiteSpace(Ip))
            {
                _receiver = new TcpClientReceiver(Ip, Port);
                _recvRunner = new ReceiverSourceRunner<IData>(_receiver, this);
                _autoRepeatRunner = new AutoRepeatRunner(_recvRunner);

                _autoRepeatRunner.Start();
                Runner.ThreadRun(_autoRepeatRunner);
            }
        }

        public void Stop()
        {
            if (_autoRepeatRunner != null)
                _autoRepeatRunner.Stop();
            _autoRepeatRunner = null;
        }

        public delegate void OnDynamic(int mmsi, double lat, double lon, double sog, double cog, int heading);
        public event OnDynamic DynamicEvent;
        private void fireOnDynamic(int mmsi, double lat, double lon, double sog, double cog, int heading)
        {
            OnDynamic callback = DynamicEvent;
            if (callback != null)
                callback(mmsi, lat, lon, sog, cog, heading);
        }

        public delegate void OnStatic(int mmsi, string name, int type, int length);
        public event OnStatic StaticEvent;
        private void fireOnStatic(int mmsi, string name, int type, int length)
        {
            OnStatic callback = StaticEvent;
            if (callback != null)
                callback(mmsi, name, type, length);
        }

        public void Consume(IData data)
        {
            var targets = _nmeaParser.Parse(data.Bytes, data.Offset, data.Length);
            foreach (USNT.DataParser.AISNMEA.Msg msg in targets)
            {
                {
                    var tar = msg as USNT.DataParser.AISNMEA.Tele123;
                    if (tar != null)
                        fireOnDynamic(tar.MMSI, tar.Latitude, tar.Longitude, tar.SOG, tar.COG, tar.TrueHeading);
                }

                {
                    var tar = msg as USNT.DataParser.AISNMEA.Tele5;
                    if (tar != null)
                        fireOnStatic(tar.MMSI, tar.Name, tar.ShipCargoType, Math.Max(tar.Width, tar.Length));
                }

                {
                    var tar = msg as USNT.DataParser.AISNMEA.Tele18;
                    if (tar != null)
                        fireOnDynamic(tar.MMSI, tar.Latitude, tar.Longitude, tar.SOG, tar.COG, tar.TrueHeading);
                }

                {
                    var tar = msg as USNT.DataParser.AISNMEA.Tele19;
                    if (tar != null)
                    {
                        fireOnDynamic(tar.MMSI, tar.Latitude, tar.Longitude, tar.SOG, tar.COG, tar.TrueHeading);
                        fireOnStatic(tar.MMSI, tar.Name, tar.ShipCargoType, Math.Max(tar.Width, tar.Length));
                    }
                }
            }
        }
    }
}
