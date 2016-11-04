using Common.Logging;
using System;
using VTSCore.Layers.Maps;

namespace VTSCore.Layers.Tracks
{
    public class SDGPSDrawer : TracksCanvasDrawer
    {
        private SDGPSDataReceiver _receiver;
        ILog LogService { get { return LogManager.GetLogger(GetType()); } }
        public SDGPSDrawer(LocatorAndBorder locator)
            : base(locator)
        {
            SetColor(null, System.Windows.Media.Brushes.SandyBrown);
            _receiver.OnReceivedData += onReceivedData;
            TimeOutHide = new TimeSpan(0, 24, 0);
            _receiver = new SDGPSDataReceiver();
        }

        public override void SetConfig(string ip, int port)
        {
            if (_receiver != null)
                _receiver.Shutdown();
            else
                _receiver = new SDGPSDataReceiver();
            LogService.InfoFormat("山东GPS...");
            _receiver.Startup(ip, port.ToString(), "100");
            LogService.InfoFormat("山东GPS");
        }

        private void onReceivedData(SDGPSData data)
        {
            lock (_tracksData)
            {
                var target = new SDGPSTarget();
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
