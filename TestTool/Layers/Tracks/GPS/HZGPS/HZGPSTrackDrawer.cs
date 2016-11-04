using SeeCool.GISFramework.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTSCore.Layers.Maps;

namespace VTSCore.Layers.Tracks
{
    public class HZGPSTrackDrawer : TracksCanvasDrawer
    {
        protected HZGPSDataReceiver _receiver;
        public HZGPSTrackDrawer(LocatorAndBorder locator)
            : base(locator)
        {
            base.SetColor(null, System.Windows.Media.Brushes.Black);
            _receiver = new HZGPSDataReceiver();
            startReceiver();
            _receiver.OnReceivedData += _receiver_OnReceivedData;
            TimeOutHide = new TimeSpan(0, 10, 0);
        }
                
        protected void startReceiver()
        {
            if (_receiver != null)
            {
                _receiver.OnLoad();
                _receiver.Startup();
            }
        }
        public override void SetConfig(string url)
        {
            if (_receiver != null && _receiver.IsRunning)
            {
                if (_receiver.Url == url)
                    return;
                _receiver.Shutdown();
            }
            else
                _receiver = new HZGPSDataReceiver();
            _receiver.Url = url;
            startReceiver();
        }

        protected void _receiver_OnReceivedData(ShipObj data)
        {
            lock (_tracksData)
            {
                var target = new HZGPSTarget();
                target.MMSI = data.MMSI;
                target.Update(data.Lon, data.Lat, data.SOG, data.COG, DateTime.Now);
                target.Name = data.Name;
                target.ReceiverTime = data.Time;
                _tracksData.UpdateDynamicEvent(target);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_receiver != null)
            {
                _receiver.OnReceivedData -= _receiver_OnReceivedData;
                if (_receiver.IsRunning)
                    _receiver.Shutdown();
            }
            _receiver = null;
        }
    }
}
