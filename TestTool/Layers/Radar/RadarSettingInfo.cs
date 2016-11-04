using Common.Logging;
using RadarServiceNetCmds;
using Seecool.Radar;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using VTSCore.Data.Common;
using VTSCore.Layers.Plotting;

namespace VTSCore.Layers.Radar
{
    public class RadarSettingInfo : INotifyPropertyChanged
    {
        RadarConnection _radar;
        PlottingAreaSettingInfomation _regionSettingInfo;
        public RadarAddressEvent RadarAddressEve { get; private set; }
        ILog LogService { get { return LogManager.GetLogger(GetType()); } }
        public RadarSettingInfo(RadarConnection radar)
        {
            _radar = radar;
            RadarAddressEve = new RadarAddressEvent(RadarAddress);
            _regionSettingInfo = PlottingAreaSettingInfomation.Instance;
            _regionSettingInfo.PropertyChanged += _regionSettingInfo_PropertyChanged;
            IsEnable = radar.IsEnable;
        }

        public string RadarName { get { return RadarAddressEve != null ? RadarAddressEve.Heading : null; } }
        
        List<RadarChannelInfo> _radarChannelInfos = new List<RadarChannelInfo>();
        public RadarChannelInfo[] RadarChannelInfos { get { return _radarChannelInfos.ToArray(); } }

        public RadarChannelInfo GetRadarChannelInfo(int index)
        {
            if (index >= 0 && index < _radarChannelInfos.Count)
                return _radarChannelInfos[index];
            return null;
        }

        public int RadarChannelSelectedIndex
        {
            get
            {
                int index = _radar.Port;
                for (int i = 0; i < _radarChannelInfos.Count; i++)
                {
                    if (_radarChannelInfos[i].LegacyPort == index)
                        return i;
                }
                return -1;
            }
        }

        void _regionSettingInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "更新雷达区域")
                saveRadarRegions();
            else if(e.PropertyName == "SetRegionsIfChanged")
            {
                saveRadarRegionsIfChanged();
            }
        }

        RadarRegion[] _radarRegionsBase = new RadarRegion[0];
        RadarRegion[] _radarRegions = new RadarRegion[0];
        public RadarRegion[] RadarRegions
        {
            get { return _radarRegions; }
            set
            {
                if (value == null)
                    value = new RadarRegion[0];
                _radarRegionsBase = RadarRegionsInfo.Copy(value);
                _radarRegions = value;
                _regionSettingInfo.UpdateRadarRegions(RadarName, RadarRegions);
            }
        }

        #region Status

        RadarStatus _radarStatus;
        RadarStatus _radarStatusBase;
        public RadarStatus RadarStatusBase
        {
            get { return _radarStatusBase; }
            set
            {
                if (value == null || (_radarStatusBase != null && CompareTypeValue.AreEqual(_radarStatusBase, value)))
                    return;
                _radarStatusBase = value;
                _regionSettingInfo.ChangedRadarName(RadarAddressEve.Heading, value.Name);
                RadarAddressEve.Heading = value.Name;
                FirePropertyChanged("RadarStatusBaseChanged");
                if (RadarStatus == null)
                    resetRadarStatus(value);
            }
        }

        public void ResetRadarStatus()
        {
            resetRadarStatus(RadarStatusBase);
        }

        public RadarStatus RadarStatus { get { return _radarStatus != null ? _radarStatus.Clone() : null; } }
        
        public RadarImageProjection.RadarInfo RadarInfo
        {
            get
            {
                RadarImageProjection.RadarInfo info = new RadarImageProjection.RadarInfo();
                if (_radar != null && RadarStatus != null)
                {
                    info.Latitiude = RadarStatus.Latitude;
                    info.Longitude = RadarStatus.Longitude;
                    info.SampleCount = RadarStatus.SampleCount;
                    info.ScanlineCount = RadarStatus.ScanLineCount;
                    info.StartRange = RadarStatus.StartRange;
                    info.Range = RadarStatus.Range;
                }
                return info;
            }
        }

        public void SetRadarConfig(RadarConfig radarConfig)
        {
            if (CompareTypeValue.AreEqual(radarConfig, RadarStatus))
                return;
            resetRadarStatusFromRadarConfigChanged(radarConfig);
            FirePropertyChanged("RadarConfigChanged");
        }

        public void SetRadarConfigFromClient(RadarConfig radarConfig)
        {
            resetRadarStatusFromRadarConfigChanged(radarConfig);
            FirePropertyChanged("RadarConfigChangedFromClient");
        }

        public void SaveRadarConfig()
        {
            RadarStatusBase = RadarStatus;
            
            FirePropertyChanged(nameof(SaveRadarConfig));
        }
        public bool IsSameRadarConfig()
        {
            return CompareTypeValue.AreEqual(RadarStatusBase as RadarConfig, RadarStatus as RadarConfig);
        }

        private void resetRadarStatus(RadarServiceNetCmds.RadarStatus value)
        {
            if (value == null || (RadarStatus != null && CompareTypeValue.AreEqual(RadarStatus, value)))
                return;
            _radarStatus = value.Clone();
            FirePropertyChanged("RadarConfigChanged");
        }

        #endregion Status
        public int ColorTableIndex
        {
            get { return _radar.ColorTableIndex; }
            set
            {
                if (_radar.ColorTableIndex != value)
                {
                    _radar.ColorTableIndex = value;
                    FirePropertyChanged("ColorTableIndex");
                }
            }
        }

        public RadarConnection RadarAddress
        {
            get { return _radar.Clone() as RadarConnection; }
            set
            {
                if (_radar.Ip != value.Ip || _radar.Port != value.Port)
                {
                    _radar.Ip = value.Ip;
                    _radar.Port = value.Port;
                    RadarAddressEve.Ip = value.Ip;
                    RadarAddressEve.Port = value.Port;
                    FirePropertyChanged("RadarIpPort");
                }
                if (_radar.RpcEndPoint != value.RpcEndPoint)
                {
                    _radar.RpcEndPoint = value.RpcEndPoint;
                    RadarAddressEve.RpcEndPoint = value.RpcEndPoint;
                    FirePropertyChanged("EndPoint");
                }
                if(_radar.ColorTableIndex != value.ColorTableIndex)
                {
                    _radar.ColorTableIndex = value.ColorTableIndex;
                    RadarAddressEve.ColorTableIndex = value.ColorTableIndex;
                    FirePropertyChanged("ColorTableIndex");
                }
            }
        }

        public void GotoPositioning() 
        {
            FirePropertyChanged("GotoPositioning");
        }

        public MapPoint Position { 
            get {
                if (RadarStatus != null)
                    return new MapPoint(RadarStatus.Longitude, RadarStatus.Latitude);
                else
                    return new MapPoint(-181, 91);
            } 
        }

        #region Channels

        RadarChannel[] RadarChannelsBase { get; set; }
        public RadarChannel[] RadarChannels
        {
            get
            {
                int length = _radarChannelInfos != null ? _radarChannelInfos.Count : 0;
                var channels = new RadarChannel[length];
                for (int i = 0; i < length; i++)
                {
                    channels[i] = new RadarChannel()
                    {
                        Name = _radarChannelInfos[i].Name,
                        Filters = _radarChannelInfos[i].Filter,
                        LegacyPort = _radarChannelInfos[i].LegacyPort
                    };
                }
                return channels;
            }
            set
            {
                RadarChannelsBase = value;
                _radarChannelInfos = new List<RadarChannelInfo>();
                if (value == null)
                    return;
                for (int i = 0; i < value.Length; i++)
                {
                    var channel = new RadarChannelInfo()
                    {
                        Name = value[i].Name,
                        Filter = value[i].Filters,
                        LegacyPort = value[i].LegacyPort,
                        Rate = "0bps"
                    };
                    _radarChannelInfos.Add(channel);
                }
                FirePropertyChanged("RadarChannels");
            }
        }

        public bool AreSameRadarChannels()
        {
            return RadarChannelsInfo.AreEqual(RadarChannelsBase, RadarChannels);
        }
        public void SetRadarChannals()
        {
            try
            {
                FirePropertyChanged("SaveRadarChannals");
                ResetRadarChannals();
                System.Windows.MessageBox.Show("雷达通道保存完毕！");
            }
            catch
            {
                System.Windows.MessageBox.Show("雷达通道保存失败！");
            }
        }

        public void AddRadarChannals(RadarChannel channel)
        {
            _radarChannelInfos.Add(new RadarChannelInfo() { Name = channel.Name, LegacyPort = channel.LegacyPort, Filter = channel.Filters });
            FirePropertyChanged("RadarChannels");
        }

        public void DeleteRadarChannals(int index)
        {
            if (index < 0 || index >= _radarChannelInfos.Count)
                return;
            _radarChannelInfos.RemoveAt(index);
            FirePropertyChanged("RadarChannels");
        }
        public void ResetRadarChannals()
        {
            FirePropertyChanged("ResetRadarChannals");
        }

        public void InitRadarChannelsRate(double[] rate)
        {
            if (_radarChannelInfos == null || rate == null || _radarChannelInfos.Count != rate.Length)
                return;
            for (int i = 0; i < _radarChannelInfos.Count; i++)
            {
                string rateString;
                if (rate[i] < 1024)
                    rateString = rate[i].ToString("F3");
                else if (rate[i] < 1024 * 1024)
                    rateString = (rate[i] / 1024).ToString("F3") + "K";
                else
                    rateString = (rate[i] / 1024 / 1024).ToString("F3") + "M";
                _radarChannelInfos[i].Rate = rateString + "bps";
            }
        }

        #endregion Channels

        #region Regions

        public void ResetRadarRegions()
        {
            FirePropertyChanged("ResetRadarRegions");
        }

        public void SaveRadarRegions(RadarRegion[] region)
        {
            RadarRegions = region;
            FirePropertyChanged("SaveRadarRegions");
        }

        private void resetRadarStatusFromRadarConfigChanged(RadarConfig radarConfig)
        {
            if (radarConfig == null)
                return;
            var radarStatusInfo = RadarStatus;
            radarStatusInfo.Name = radarConfig.Name;
            radarStatusInfo.Longitude = radarConfig.Longitude;
            radarStatusInfo.Latitude = radarConfig.Latitude;
            radarStatusInfo.Altitude = radarConfig.Altitude;
            radarStatusInfo.StartScanAngle = radarConfig.StartScanAngle;
            radarStatusInfo.StartRange = radarConfig.StartRange;
            radarStatusInfo.Range = radarConfig.Range;
            radarStatusInfo.OffsetAngle = radarConfig.OffsetAngle;
            _radarStatus = radarStatusInfo;
        }

        private void saveRadarRegions()
        {
            if (IsEnable)
                SaveRadarRegions(_regionSettingInfo.GetRadarRegionsFromName(RadarName));
            else
                LogService.InfoFormat("雷达 " + RadarName + " 未启用，相关区域无法更新。");
        }

        private void saveRadarRegionsIfChanged()
        {
            if (IsEnable)
            {
                var regions = _regionSettingInfo.GetRadarRegionsFromName(RadarName);
                if (!RadarRegionsInfo.AreEqual(_radarRegionsBase, regions))
                {
                    DialogResult msgBox = System.Windows.Forms.MessageBox.Show("雷达“" + RadarName + "”对应的雷达区域参数发生变更,是否保存?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    if (msgBox == DialogResult.Yes)
                        SaveRadarRegions(regions);
                }
            }
        }

        #endregion Regions

        bool _isEnable = false;
        public bool IsEnable 
        {
            get { return _isEnable; }
            set
            {
                if (IsEnable != value)
                {
                    _isEnable = value;
                    if (_radar != null)
                        _radar.IsEnable = value;
                    FirePropertyChanged("IsEnable");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void FirePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}