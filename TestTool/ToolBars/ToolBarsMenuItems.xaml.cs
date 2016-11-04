using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TestTool.Layers.Maps;
using VTSCore.Data.Common;
using VTSCore.Layers.Plotting;

namespace VTSCore.ToolBars
{
    /// <summary>
    /// ToolBarsMenuItems.xaml 的交互逻辑
    /// </summary>
    public partial class ToolBarsMenuItems : UserControl
    {
        VTSCore.Data.Common.MenuBarsBaseInfo _menuBars;
        VTSCore.Layers.Radar.RadarsSettingInfo _radar;
        VTSCore.Layers.Radar.RadarSettingInfo _radarSelected;
        ActivatingStatus _activatingStatus;
        PlottingAreaSettingInfomation _radarAreaInfo;
        public ToolBarsMenuItems()
        {
            InitializeComponent();
            _menuBars = VTSCore.Data.Common.MenuBarsBaseInfo.Instance;
            _menuBars.PropertyChanged += menuBars_PropertyChanged;
            _activatingStatus = ActivatingStatus.Instance;
            _activatingStatus.PropertyChanged += activatingStatus_PropertyChanged;

            _radar = Layers.Radar.RadarsSettingInfo.Instance;
            _radar.PropertyChanged += _radar_PropertyChanged;
            loadRadarSelectData();

            _radarAreaInfo = PlottingAreaSettingInfomation.Instance;
            loadRadarAreaInfo();
        }

        void _radar_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ReadAllConfig" || e.PropertyName == "AddRadar" || e.PropertyName == "RemoveRadar")
            {
                loadRadarSelectData();
            }
            else if(e.PropertyName=="SelectedIndex")
            {
                if (cbRadarSelect.SelectedIndex != _radar.SelectedIndex)
                {
                    cbRadarSelect.SelectedIndex = _radar.SelectedIndex;
                }
            }
        }

        void activatingStatus_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ChartsStatus")
            {
                switch(_activatingStatus.ChartStatus)
                {
                    case ChartsStatus.移动海图:
                        SeaChartMove.IsChecked = true;
                        break;
                    case ChartsStatus.移动雷达:
                        RadarMove.IsChecked = true;
                        break;
                    case ChartsStatus.拖拽雷达:
                        RadarDrag.IsChecked = true;
                        break;
                }
            }
        }

        void menuBars_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LockAll")
                SetButtonBackground(LockAll, _menuBars.LockAll);
            else if (e.PropertyName == "Tracking")
                SetButtonBackground(Tracking, _menuBars.Tracking);
            else if (e.PropertyName == "DistanceMeasurement")
                SetButtonBackground(DistanceMeasurement, _menuBars.DistanceMeasurement);
        }

        private void MoveSeaChart_Click(object sender, RoutedEventArgs e)
        {
            _activatingStatus.ChartStatus = ChartsStatus.移动海图;
        }

        private void MoveRadarChart_Click(object sender, RoutedEventArgs e)
        {
            if(haveAnyRadars())
                _activatingStatus.ChartStatus = ChartsStatus.移动雷达;
        }

        private void DragRadarChart_Click(object sender, RoutedEventArgs e)
        {
            if(haveAnyRadars())
                _activatingStatus.ChartStatus = ChartsStatus.拖拽雷达;
        }

        private void LockAll_Click(object sender, RoutedEventArgs e)
        {
            _menuBars.LockAll = !_menuBars.LockAll;
        }

        private void Tracking_Click(object sender, RoutedEventArgs e)
        {
            _menuBars.Tracking = !_menuBars.Tracking;
        }

        private void DistanceMeasurement_Click(object sender, RoutedEventArgs e)
        {
            _menuBars.DistanceMeasurement = !_menuBars.DistanceMeasurement;
        }

        private void RadarParameterSettings_Click(object sender, RoutedEventArgs e)
        {
            _menuBars.RadarParameterSetting();
        }

        private void PlottingArea_Click(object sender, RoutedEventArgs e)
        {
            _menuBars.PlottingAreaParameterSetting();
        }

        private void ClearCache_Click(object sender, RoutedEventArgs e)
        {
            _menuBars.ClearCache();
        }

        private void CCTVConfig_Click(object sender, RoutedEventArgs e)
        {
            _menuBars.CCTVConfigSetting();
        }
        
        private void SetButtonBackground(Button button, bool status)
        {
            if (status)
                button.Background = new SolidColorBrush(Colors.LightSkyBlue);
            else
                button.Background = new SolidColorBrush(Colors.WhiteSmoke);
        }

        private void OpenPath_Click(object sender, RoutedEventArgs e)
        {
            string path = System.IO.Directory.GetParent(System.Windows.Forms.Application.LocalUserAppDataPath).FullName;
            System.Diagnostics.Process.Start("Explorer.exe", path);
        }
        
        private void radarAreaIsMask_Click(object sender, RoutedEventArgs e)
        {
            _radarAreaInfo.IsMask = radarAreaIsMask.IsChecked.Value;
        }

        private void radarAreaManualExtract_Click(object sender, RoutedEventArgs e)
        {
            _radarAreaInfo.ManualExtract = radarAreaManualExtract.IsChecked.Value;
        }

        private void radarAreaSimuVoyage_Click(object sender, RoutedEventArgs e)
        {
            _radarAreaInfo.SimuVoyage = radarAreaSimuVoyage.IsChecked.Value;
        }

        private void loadRadarAreaInfo()
        {
            radarAreaIsMask.IsChecked = _radarAreaInfo.IsMask;
            radarAreaManualExtract.IsChecked = _radarAreaInfo.ManualExtract;
            radarAreaSimuVoyage.IsChecked = _radarAreaInfo.SimuVoyage;
        }

        private void cbRadarSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbRadarSelect.SelectedIndex >= 0)
            {
                _radar.SelectedIndex = cbRadarSelect.SelectedIndex;
            }
        }

        void loadRadarSelectData()
        {
            initSelectedRadar();
            lock (cbRadarSelect)
            {
                cbRadarSelect.Items.Clear();
                for (int i = 0; i < _radar.Count; i++)
                {
                    cbRadarSelect.Items.Add(_radar.Radars[i].RadarAddressEve);
                }
                if (cbRadarSelect.SelectedIndex != _radar.SelectedIndex)
                    cbRadarSelect.SelectedIndex = _radar.SelectedIndex;
            }
            updateSelectedRadar();
        }
        
        private void initSelectedRadar()
        {
            _radarSelected = null;
        }

        private void updateSelectedRadar()
        {
            _radarSelected = _radar.RadarPrev;
        }

        private void CCTVTreeView_Click(object sender, RoutedEventArgs e)
        {
            _menuBars.CCTVTreeView();
        }

        private void SignalSource_Click(object sender, RoutedEventArgs e)
        {
            _menuBars.SignalSourceSetting();
        }

        private void cbRadarSelect_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _mouseDownCount++;
            _timeDown = DateTime.Now;
        }

        private void cbRadarSelect_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _mouseDownCount = 0;
            _timeDown = DateTime.Now;
        }
        int _mouseDownCount = 0;
        DateTime _timeDown;
        private void cbRadarSelect_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_radar != null && _mouseDownCount > 1 && _timeDown.AddSeconds(1) > DateTime.Now)
                _radar.CenteredRadar();
        }

        private void RadarColorClient_Click(object sender, RoutedEventArgs e)
        {
            _menuBars.RadarColorSchemes();
        }

        private void TrackLength_Click(object sender, RoutedEventArgs e)
        {
            _menuBars.TrackLength();
            double timeoutSpan = TargetInfomation.MovableTarget.TimeOutSpan.TotalMinutes;
            var client = new VTSCore.Layers.Base.TimeOutSettingClient(timeoutSpan.ToString());
            if (client.ShowDialog().Value)
            {
                TargetInfomation.MovableTarget.TimeOutSpan = TimeSpan.FromMinutes(client.Data);
                GC.Collect();
            }
        }

        private void StartUpPosition_Click(object sender, RoutedEventArgs e)
        {
            _menuBars.StartUpPosition();
        }

        private bool haveAnyRadars()
        {
            if (_radar.Count > 0)
            {
                if (_radar.SelectedIndex < 0)
                    _radar.SelectedIndex = 0;
                return true;
            }
            MessageBox.Show("未配置任何雷达参数，无法进行雷达参数修改！", "异常", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            return false;
        }

        private void cbSCVTSVerification_Click(object sender, RoutedEventArgs e)
        {
            VTSCore.Layers.Tracks.RadarMuxerTarget.IsStartUpVerify = cbSCVTSVerification.IsChecked.Value;
        }

        private void cbOnlyShowIdentifiedTrack_Click(object sender, RoutedEventArgs e)
        {
            _menuBars.OnlyShowIdentifiedTrack = cbOnlyShowIdentifiedTrack.IsChecked.Value;
        }

        private void cbShowHistoryTrackLine_Click(object sender, RoutedEventArgs e)
        {
            _menuBars.ShowAllHistoryTrackLine = cbShowHistoryTrackLine.IsChecked.Value;
        }
    }
}