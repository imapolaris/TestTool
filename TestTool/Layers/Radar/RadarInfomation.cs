using RadarServiceNetCmds;
using Seecool.Radar;
using Seecool.RemoteCall;
using Seecool.RemoteCall.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VTSCore.Layers.Radar
{
    public class RadarInfomation: IDisposable
    {
        ZmqRemoteCallClient _zmqClient = null;
        IConfigService _configClient = null;
        IRadarChannelMaintainer _radarChannelsMaintainer = null;
        IRadarRegionMaintainer _regionClient = null;

        public RadarInfomation(string rpcEndPoint)
        {
            start(rpcEndPoint);
        }
        
        ~RadarInfomation()
        {
            Dispose();
        }

        public void Dispose()
        {
            stop();
        }

        public RadarStatus GetRadarInfo()
        {
            return _configClient.GetRadarInfo();
        }

        public void SetRadarInfo(RadarConfig radarConfig)
        {
            if (radarConfig != null)
                _configClient.SetRadarConfig(radarConfig);
        }

        public void RemoveRegion(string name)
        {
            if (!string.IsNullOrEmpty(name))
                _regionClient.RemoveRegion(name);
        }

        public RadarRegion[] RadarRegions
        {
            get { return _regionClient.GetRegions(); }
            set
            {
                _regionClient.SetRegions(value);
            }
        }

        public RadarChannel[] RadarChannels
        {
            get { return _radarChannelsMaintainer.GetChannels(); }
            set
            {
                if (_radarChannelsMaintainer != null)
                    _radarChannelsMaintainer.SetChannels(value);
            }
        }

        public double[] Rates
        {
            get
            {
                return _radarChannelsMaintainer.GetChannelRates();
            }
        }

        void start(string rpcEndPoint)
        {
            IFormatter formatter = new JsonFormatter();
            _zmqClient = new ZmqRemoteCallClient(rpcEndPoint, formatter, TimeSpan.FromSeconds(15));
            _configClient = InterfaceProxy.CreateObject<IConfigService>(_zmqClient, "RadarConfig");
            InterfaceProxy<IRadarChannelMaintainer> proxy = new InterfaceProxy<IRadarChannelMaintainer>(_zmqClient, "RadarChannels");
            _radarChannelsMaintainer = proxy.Object;
            _radarChannelsMaintainer = InterfaceProxy.CreateObject<IRadarChannelMaintainer>(_zmqClient, "RadarChannels");
            _regionClient = InterfaceProxy.CreateObject<IRadarRegionMaintainer>(_zmqClient, "RadarRegions");
        }
        
        async void stop()
        {
            await Task.Yield();
            _zmqClient = null;
            _configClient = null;
            _regionClient = null;
            _radarChannelsMaintainer = null;
        }
    }
}