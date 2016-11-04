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
    public class VTSSofrelogDrawer : TracksCanvasDrawer
    {
        SofrelogReceiver _receiver = new SofrelogReceiver();
        public VTSSofrelogDrawer(LocatorAndBorder locator)
            : base(locator)
        {
            SetColor(null, System.Windows.Media.Brushes.AliceBlue);
            _receiver.OnReceivedData += _receiver_OnReceivedData;
            _receiver.OnResyncName += _receiver_OnResyncName;
            _receiver.OnDropData += _receiver_OnDropData;
            TimeOutHide = new TimeSpan(0, 0, 30);
        }
        
        public override void SetConfig(string ip, int port)
        {
            if (_receiver != null)
                _receiver.Shutdown();
            else
                _receiver = new SofrelogReceiver();
            _receiver.Startup(ip, port.ToString(), "100");
        }

        void _receiver_OnReceivedData(SofrelogData data)
        {
            lock (_tracksData)
            {
                var target = new VTSSofrelogTarget(data.TrackID);
                target.MMSI = data.MMSI;
                target.Update(data.Lon, data.Lat, data.SOG, data.COG, DateTime.Now);
                target.Name = data.Name;
                target.ReceiverTime = data.Time;
                _tracksData.UpdateDynamicEvent(target);
            }
        }

        void _receiver_OnResyncName(int trackID, string name)
        {
            lock (_tracksData)
                _tracksData.UpdateName(trackID, name);
        }

        private void _receiver_OnDropData(int trackID)
        {
            _tracksData.Remove(trackID.ToString());
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_receiver != null)
            {
                _receiver.OnReceivedData -= _receiver_OnReceivedData;
                _receiver.OnResyncName -= _receiver_OnResyncName;
                _receiver.OnDropData -= _receiver_OnDropData;
                _receiver.Shutdown();
            }
            _receiver = null;
        }
    }
}
