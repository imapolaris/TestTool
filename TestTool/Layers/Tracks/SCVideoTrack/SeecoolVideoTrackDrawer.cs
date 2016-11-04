using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTSCore.Layers.Maps;

namespace VTSCore.Layers.Tracks
{
    public class SeecoolVideoTrackDrawer: TracksCanvasDrawer
    {
        SeecoolVideoReceiver _receiver = new SeecoolVideoReceiver();

        public SeecoolVideoTrackDrawer(LocatorAndBorder locator)
            : base(locator)
        {
            SetColor(null, System.Windows.Media.Brushes.OrangeRed);
            EnumerateType = DynamicBaseCanvas.GeometryType.Circle;
            _receiver.TargetEvent += _receiver_TargetEvent;
            TimeOutHide = new TimeSpan(0, 0, 3);
        }
        
        public override void SetConfig(string ip, int port)
        {
            _receiver.SetConfig(ip, port);
        }

        private void _receiver_TargetEvent(int id, double lat, double lon, double sog, double cog)
        {
            lock(_tracksData)
            {
                var target = new ScVideoTarget(id);
                target.Update(lon, lat, sog, cog, DateTime.Now);
                _tracksData.UpdateDynamicEvent(target);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_receiver != null)
                _receiver.Stop();
            _receiver = null;
        }
    }
}
