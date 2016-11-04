using SeeCool.GISFramework.Object;
using SeeCool.GISFramework.SvrFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VTSCore.Layers.Maps;

namespace VTSCore.Layers.Tracks
{
    public class VTSNorcontrolTrackDrawer : TracksCanvasDrawer
    {
        NorcontrolReceiver _receiver = new NorcontrolReceiver();
        public VTSNorcontrolTrackDrawer(LocatorAndBorder locator)
            : base(locator)
        {
            SetColor(null, System.Windows.Media.Brushes.Navy);
            _receiver.OnReceivedData += onReceivedData;
            TimeOutHide = new TimeSpan(0, 0, 30);
        }
        
        public override void SetConfig(string ip, int port)
        {
            if (_receiver != null)
                _receiver.Shutdown();
            else
                _receiver = new NorcontrolReceiver();
            _receiver.Startup(ip, port.ToString(), "100");
        }

        private void onReceivedData(NorcontrolData data)
        {
            lock (_tracksData)
            {
                var target = new VTSNorcontrolTarget(data.Id);
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
                _receiver.Shutdown();
                _receiver.OnReceivedData -= onReceivedData;
            }
            _receiver = null;
        }
    }
}
