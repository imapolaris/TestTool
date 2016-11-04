using Common.Logging;
using RadarServiceNetCmds;
using Seecool.Radar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VTSCore.Layers.Plotting;

namespace VTSCore.Layers.Radar
{
    class RadarSettingControl
    {
        PlottingAreaSettingInfomation _radarRegions;
        RadarInfomation _radarInfomation;
        ILog LogService { get { return LogManager.GetLogger(GetType()); } }
        public RadarSettingControl()
        {
            _radarRegions = PlottingAreaSettingInfomation.Instance;
            _radarRegions.PropertyChanged += _radarRegions_PropertyChanged;
        }

        void _radarRegions_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PresPlottingName")
            {
                removeRadarRegion(_radarRegions.LastName);
            }
        }
        public string RpcEndpoint { get; private set; }
        public void Start(string rpcEndpoint)
        {
            Stop();
            _radarInfomation = new RadarInfomation(rpcEndpoint);
            if (_radarInfomation != null)
                IsLinking = true;
            RpcEndpoint = rpcEndpoint;
        }
        public bool IsLinking { get; private set; }
        public void Stop()
        {
            RpcEndpoint = null;
            IsLinking = false;
            if (_radarInfomation != null)
                _radarInfomation.Dispose();
            _radarInfomation = null;
        }

        private void removeRadarRegion(string name)
        {
            try
            {
                if (_radarInfomation != null)
                {
                    _radarInfomation.RemoveRegion(name);
                    IsLinking = true;
                }
            }
            catch
            {
                IsLinking = false;
            }
        }

        public void LoadData(RadarSettingInfo radar)
        {
            try
            {
                if (_radarInfomation != null)
                {
                    radar.RadarChannels = _radarInfomation.RadarChannels;
                    radar.InitRadarChannelsRate(_radarInfomation.Rates);
                    radar.RadarStatusBase = _radarInfomation.GetRadarInfo();
                    radar.RadarRegions = _radarInfomation.RadarRegions;
                    radar.ResetRadarStatus();
                    IsLinking = true;
                }
            }
            catch(Exception ex)
            {
                LogManager.GetLogger(this.GetType()).Warn(ex.ToString());
                IsLinking = false;
            }
        }

        public void ResetRadarStatus(RadarSettingInfo radar)
        {
            try
            {
                if (_radarInfomation != null)
                {
                    radar.RadarStatusBase = _radarInfomation.GetRadarInfo();
                    IsLinking = true;
                }
            }
            catch
            {
                IsLinking = false;
            }
        }

        public void ResetRadarChannels(RadarSettingInfo radar)
        {
            try
            {
                if (_radarInfomation != null)
                {
                    radar.RadarChannels = _radarInfomation.RadarChannels;
                    IsLinking = true;
                }
            }
            catch
            {
                IsLinking = false;
            }
        }

        public void ResetRadarRegions(RadarSettingInfo radar)
        {
            try
            {
                if (_radarInfomation != null)
                {
                    radar.RadarRegions = _radarInfomation.RadarRegions;
                    IsLinking = true;
                }
            }
            catch
            {
                IsLinking = false;
            }
        }

        public double[] Rates
        {
            get
            {
                try
                {
                    if (_radarInfomation != null)
                    {
                        var rates = _radarInfomation.Rates;
                        IsLinking = true;
                        return rates;
                    }
                }
                catch
                {
                    IsLinking = false;
                }
                return null;
            }
        }

        public void SetRadarRegions(RadarRegion[] regions)
        {
            try
            {
                if (_radarInfomation != null)
                {
                    _radarInfomation.RadarRegions = regions;
                    IsLinking = true;
                }
            }
            catch
            {
                IsLinking = false;
            }
        }

        public void SetRadarConfig(RadarConfig radarConfig)
        {
            try
            {
                if (_radarInfomation != null)
                {
                    _radarInfomation.SetRadarInfo(radarConfig);
                    IsLinking = true;
                }
            } 
            catch
            {
                IsLinking = false;
            }
        }

        public void SetRadarChannels(RadarChannel[] channels)
        {
            try
            {
                if (_radarInfomation != null)
                {
                    _radarInfomation.RadarChannels = channels;
                    IsLinking = true;
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex.ToString());
                IsLinking = false;
                throw ex;
            }
        }
    }
}
