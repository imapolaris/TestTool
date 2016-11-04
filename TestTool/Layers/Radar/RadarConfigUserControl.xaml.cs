using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VTSCore.Data.Common;
using VTSCore.Layers.Base;

namespace VTSCore.Layers.Radar
{
    /// <summary>
    /// RadarConfigUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class RadarConfigUserControl : UserControl, IDisposable
    {
        RadarsSettingInfo _radarsInfo;
        RadarSettingInfo _radarInfo;
        List<DataEditUnitObj> _radarConfig = new List<DataEditUnitObj>();
        ActivatingStatus _activatingStatus;
        ILog LogService { get { return LogManager.GetLogger(GetType()); } }
        public RadarConfigUserControl()
        {
            InitializeComponent();
            this.Visibility = System.Windows.Visibility.Collapsed;
            _activatingStatus = VTSCore.Data.Common.ActivatingStatus.Instance;
            _activatingStatus.PropertyChanged += _activatingStatus_PropertyChanged;
        }

        void _activatingStatus_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(_activatingStatus.ChartStatus == ChartsStatus.拖拽雷达 || _activatingStatus.ChartStatus == ChartsStatus.移动雷达)
            {
                if (this.Visibility != System.Windows.Visibility.Visible)
                {
                    this.Visibility = System.Windows.Visibility.Visible;
                    Init();
                }
            }
            else
            {
                this.Visibility = System.Windows.Visibility.Collapsed;
                Dispose();
            }
        }

        private void Init()
        {
            _radarsInfo = RadarsSettingInfo.Instance;
            loadListView();
            _radarsInfo.PropertyChanged += radarsInfo_PropertyChanged;
        }

        private void radarsInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ReadAllConfig" || e.PropertyName == "AddRadar" || e.PropertyName == "RemoveRadar" || e.PropertyName == "SelectedIndex")
                loadListView();
        }

        private void _radarInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "RadarConfigChanged")
            {
                setRadarConfigFromEditing(_radarInfo);
                loadRadarConfigListView();
            }
        }

        private void loadListView()
        {
            removeSelectedRadar();
            updateSelectedRadar();
            loadRadarConfigListView();
        }

        void loadRadarConfigData()
        {
            _radarInfo = _radarsInfo.RadarPrev;
            if (_radarInfo != null)
            {
                _radarInfo.PropertyChanged += _radarInfo_PropertyChanged;
                setRadarConfigFromEditing(_radarInfo);
            }
        }

        void loadRadarConfigListView()
        {
            lock (radarConfigListView)
            {
                int selectedIndex = radarConfigListView.SelectedIndex;
                radarConfigListView.Items.Clear();
                for (int i = 0; i < _radarConfig.Count; i++)
                {
                    radarConfigListView.Items.Add(_radarConfig[i]);
                }
                radarConfigListView.SelectedIndex = selectedIndex;
            }
        }

        private void removeSelectedRadar()
        {
            if (_radarInfo != null)
                _radarInfo.PropertyChanged -= _radarInfo_PropertyChanged;
            _radarInfo = null;
        }

        private void updateSelectedRadar()
        {
            _radarInfo = _radarsInfo.RadarPrev;
            if (_radarInfo != null)
            {
                _radarInfo.PropertyChanged += _radarInfo_PropertyChanged;
                setRadarConfigFromEditing(_radarInfo);
            }
        }

        private void setRadarConfigFromEditing(RadarSettingInfo radar)
        {
            _radarConfig.Clear();
            if (radar != null && radar.RadarStatusBase != null && radar.RadarStatus != null)
            {
                var baseStatus = radar.RadarStatusBase;
                var radarStatus = radar.RadarStatus;
                _radarConfig.Add(new DataEditUnitObj() { Heading = "雷达名称", EditingValue = radarStatus.Name });
                _radarConfig.Add(new DataEditUnitObj() { Heading = "经度", EditingValue = radarStatus.Longitude.ToString() });
                _radarConfig.Add(new DataEditUnitObj() { Heading = "纬度", EditingValue = radarStatus.Latitude.ToString() });
                _radarConfig.Add(new DataEditUnitObj() { Heading = "高度", EditingValue = radarStatus.Altitude.ToString() });
                _radarConfig.Add(new DataEditUnitObj() { Heading = "量程", EditingValue = radarStatus.Range.ToString() });
                _radarConfig.Add(new DataEditUnitObj() { Heading = "量程开始", EditingValue = radarStatus.StartRange.ToString() });
                _radarConfig.Add(new DataEditUnitObj() { Heading = "起始角度", EditingValue = radarStatus.StartScanAngle.ToString() });
                _radarConfig.Add(new DataEditUnitObj() { Heading = "偏移角度", EditingValue = radarStatus.OffsetAngle.ToString() });
            }
        }

        private void btSaveRadarStatus_Click(object sender, RoutedEventArgs e)
        {
            if (_radarInfo != null)
                _radarInfo.SaveRadarConfig();
        }

        private void btResetRadarStatus_Click(object sender, RoutedEventArgs e)
        {
            if (_radarInfo != null)
                _radarInfo.ResetRadarStatus();
        }
        private void EditBox_SourceUpdated(object sender, DataTransferEventArgs e)//参数修改时时生效
        {
            updateRadarInfoFromClient();
        }

        private void updateRadarInfoFromClient()
        {
            if (_radarInfo != null && _radarConfig.Count > 0)
            {
                try
                {
                    var radarConfig = getRadarConfigFromEditing();
                    _radarInfo.SetRadarConfigFromClient(radarConfig);
                }
                catch (Exception ex)
                {
                    LogService.Error(ex.ToString());
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private RadarServiceNetCmds.RadarConfig getRadarConfigFromEditing()
        {
            var radarConfig = new RadarServiceNetCmds.RadarConfig();
            foreach (var config in _radarConfig)
            {
                switch (config.Heading)
                {
                    case "雷达名称":
                        radarConfig.Name = config.EditingValue;
                        break;
                    case "经度":
                        radarConfig.Longitude = doubleParse(config.EditingValue);
                        break;
                    case "纬度":
                        radarConfig.Latitude = doubleParse(config.EditingValue);
                        break;
                    case "高度":
                        radarConfig.Altitude = doubleParse(config.EditingValue);
                        break;
                    case "量程":
                        radarConfig.Range = doubleParse(config.EditingValue);
                        break;
                    case "量程开始":
                        radarConfig.StartRange = doubleParse(config.EditingValue);
                        break;
                    case "起始角度":
                        radarConfig.StartScanAngle = doubleParse(config.EditingValue);
                        break;
                    case "偏移角度":
                        radarConfig.OffsetAngle = doubleParse(config.EditingValue);
                        break;
                }
            }
            return radarConfig;
        }

        double doubleParse(string value)
        {
            if(string.IsNullOrEmpty(value))
                return 0;
            return double.Parse(value);
        }

        public void Dispose()
        {
            removeSelectedRadar();
            if (_radarsInfo != null)
                _radarsInfo.PropertyChanged -= radarsInfo_PropertyChanged;
            _radarsInfo = null;
        }
    }
}
