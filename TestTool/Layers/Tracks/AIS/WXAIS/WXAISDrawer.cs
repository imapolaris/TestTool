using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTSCore.Layers.Maps;

namespace VTSCore.Layers.Tracks
{
    public class WXAISDrawer : TracksCanvasDrawer
    {
        WXAISReceiver _receiver = new WXAISReceiver();

        public WXAISDrawer(LocatorAndBorder locator):base(locator)
        {
            _receiver.OnDynamicEvent += OnDynamicEvent;
            TimeOutHide = new TimeSpan(0, 10, 0);
        }
        
        public override void SetConfig(string ip, int port)
        {
            if (_receiver != null)
                _receiver.Shutdown();
            else
                _receiver = new WXAISReceiver();
            _receiver.Startup(ip, port.ToString(), "100");
        }

        private void OnDynamicEvent(WXAISTarget data)
        {
            lock (_tracksData)
            {
                _tracksData.UpdateDynamicEvent(data);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_receiver != null)
                _receiver.Shutdown();
            _receiver = null;
        }
    }
}
