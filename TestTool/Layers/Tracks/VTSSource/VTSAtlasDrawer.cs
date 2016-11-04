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
    public class VTSAtlasDrawer : TracksCanvasDrawer
    {
        AtlasReceiver _receiver = new AtlasReceiver();
        public VTSAtlasDrawer(LocatorAndBorder locator)
            : base(locator)
        {
            SetColor(null, System.Windows.Media.Brushes.AliceBlue);
            _receiver.OnReceivedData += _receiver_OnPositionEvent;
            _receiver.OnIdentificationEvent += _receiver_OnIdentificationEvent;
            TimeOutHide = new TimeSpan(0, 0, 30);
        }
        
        public override void SetConfig(string ip, int port)
        {
            if (_receiver != null)
                _receiver.Shutdown();
            else
                _receiver = new AtlasReceiver();
            _receiver.Startup(ip, port.ToString(), "100");
        }

        void _receiver_OnIdentificationEvent(int trackID, string name, string callSign, int imo)
        {
            lock (_tracksData)
                _tracksData.UpdateName(trackID, name);
        }

        void _receiver_OnPositionEvent(AtlasData data)
        {
            lock (_tracksData)
            {
                var target = new VTSAtlasTarget(data.TrackID);
                target.MMSI = data.MMSI;
                target.Update(data.Lon, data.Lat, data.SOG, data.COG, DateTime.Now);
                target.ReceiverTime = data.Time;
                _tracksData.UpdateDynamicEvent(target);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_receiver != null)
            {
                _receiver.OnReceivedData -= _receiver_OnPositionEvent;
                _receiver.OnIdentificationEvent -= _receiver_OnIdentificationEvent;
                _receiver.Shutdown();
            }
            _receiver = null;
        }
    }
}
