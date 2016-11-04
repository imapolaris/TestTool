using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VTSCore.Layers.Maps;

namespace VTSCore.Layers.Tracks
{
    public class SeecoolRadarTrackDrawer: TracksCanvasDrawer
    {
        SeecoolRadarReceiver _receiver = new SeecoolRadarReceiver();

        public SeecoolRadarTrackDrawer(LocatorAndBorder locator)
            : base(locator)
        {
            SetColor(null, System.Windows.Media.Brushes.Purple);
            EnumerateType = DynamicBaseCanvas.GeometryType.Rectangle;
            _receiver.TargetEvent += _receiver_TargetEvent;
            TimeOutHide = new TimeSpan(0, 0, 10);
        }
                
        public override void SetConfig(string ip, int port)
        {
            _receiver.SetConfig(ip, port);
        }
        void _receiver_TargetEvent(int id, double lat, double lon, double sog, double cog, int mmsi, string name)
        {
            ScRadarTarget target = new ScRadarTarget(id);
            target.Name = name;
            target.MMSI = mmsi;
            target.Update(lon, lat, sog, cog, DateTime.Now);
            lock (_tracksData)
                _tracksData.UpdateDynamicEvent(target);
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
