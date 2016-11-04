using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using VTSCore.Data.Common;
using VTSCore.Layers.Maps;

namespace VTSCore.Layers.Tracks
{
    public class AISTrackDrawer : TracksCanvasDrawer
    {
        AisReceiver _aisReceiver = new AisReceiver();

        public AISTrackDrawer(LocatorAndBorder locator)
            : base(locator)
        {
            SetColor(null, System.Windows.Media.Brushes.Black);
            _aisReceiver.DynamicEvent += _aisReceiver_DynamicEvent;
            _aisReceiver.StaticEvent += _aisReceiver_StaticEvent;
            TimeOutHide = new TimeSpan(0, 10, 0);
        }

        public override void SetConfig(string ip, int port)
        {
            _aisReceiver.SetConfig(ip, port);
        }

        private void _aisReceiver_StaticEvent(int mmsi, string name, int type, int length)
        {
            lock (_tracksData)
            {
                AisTarget target = new AisTarget(mmsi);
                target.Name = name;
                target.Type = type;
                target.Length = length;
                _tracksData.UpdateStaticEvent(target);
            }
        }

        private void _aisReceiver_DynamicEvent(int mmsi, double lat, double lon, double sog, double cog, int heading)
        {
            lock (_tracksData)
            {
                AisTarget target = new AisTarget(mmsi);
                target.Update(lon, lat, sog, cog, DateTime.Now);
                target.Heading = heading;
                _tracksData.UpdateDynamicEvent(target);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            _aisReceiver.Stop();
        }
    }
}