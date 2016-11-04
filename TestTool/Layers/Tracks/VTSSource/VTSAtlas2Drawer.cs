using SeeCool.GISFramework.Object;
using SeeCool.GISFramework.SvrFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTSCore.Layers.Maps;

namespace VTSCore.Layers.Tracks
{
    public class VTSAtlas2Drawer : TracksCanvasDrawer
    {
        Atlas2Receiver _receiver = new Atlas2Receiver();

        public VTSAtlas2Drawer(LocatorAndBorder locator)
            : base(locator)
        {
            SetColor(null, System.Windows.Media.Brushes.AliceBlue);
            _receiver.OnReceivedData += _receiver_OnPositionEvent;
            TimeOutHide = new TimeSpan(0, 0, 30);
        }
        
        public override void SetConfig(string ip, int port)
        {
            if (_receiver != null)
                _receiver.Shutdown();
            else
                _receiver = new Atlas2Receiver();
            _receiver.Startup(ip, port.ToString(), "100");
        }

        private void _receiver_OnPositionEvent(Atlas2Data data)
        {
            VTSAtlas2Target target = new VTSAtlas2Target(data.TrackId);
            target.MMSI = data.MMSI;
            target.Update(data.Lon, data.Lat, data.SOG, data.COG, DateTime.Now);
            target.Name = data.Name;
            target.ReceiverTime = data.Time;
            lock (_tracksData)
            {
                _tracksData.UpdateDynamicEvent(target);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_receiver != null)
            {
                _receiver.Shutdown();
                _receiver.OnReceivedData -= _receiver_OnPositionEvent;
            }
            _receiver = null;
        }
    }
}
