using Common.Logging;
using RadarServiceNetCmds;
using Seecool.Common.Configuration;
using Seecool.Radar.Filters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml.Linq;
using VTSCore.Layers.Base;

namespace VTSCore.Layers.Radar
{
    /// <summary>
    /// RadarSettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RadarSettingClient : Window
    {
        RadarsSettingInfo _radarsInfo;
        RadarSettingInfo _radarInfo;
        List<DataEditUnitObj> _radarConfig = new List<DataEditUnitObj>();
        List<DataEditUnitObj> _radarStatus = new List<DataEditUnitObj>();

        List<RadarAddressEvent> _radarAddrList = new List<RadarAddressEvent>();

        private Collection<string> camboBoxColorTableSources = new Collection<string>();
        public Collection<string> Sources { get { return camboBoxColorTableSources; } }
        ILog LogService { get { return LogManager.GetLogger(GetType()); } }
        
        public RadarSettingClient()
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            _radarsInfo = RadarsSettingInfo.Instance;
            loadListView();
            _radarsInfo.PropertyChanged += radarsInfo_PropertyChanged;
            initCamboBox();
            initRadarChannels();
            radarsListView.DataContext = this;
        }

        void _radarInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "RadarConfigChanged")
            {
                setRadarConfigFromEditing(_radarInfo);
                updateRadarListViewValue(_radarInfo);
                updateRadarStatus();
            }
            else if (e.PropertyName == "RadarStatusBaseChanged")
            {
                updateRadarStatusBaseListView();
            }
            else if (e.PropertyName == "RadarChannels")
                initRadarChannels();
        }

        #region 雷达列表更新

        void radarsInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ReadAllConfig" || e.PropertyName == "AddRadar" || e.PropertyName == "RemoveRadar")
                loadListView();
            else if(e.PropertyName == "SelectedIndex")
            {
                if(radarsListView.SelectedIndex != _radarsInfo.SelectedIndex)//外部调整雷达列表选中项
                {
                    radarsListView.SelectedIndex = _radarsInfo.SelectedIndex;
                    updateSelectedRadar();
                    updateRadarStatus();
                    initRadarChannels();
                }
            }
        }

        private void loadListView()
        {
            initSelectedRadar();
            radarsListView.Items.Clear();
            _radarAddrList.Clear();
            for (int i = 0; i < _radarsInfo.Radars.Length; i++)
            {
                var radarAddr = _radarsInfo.Radars[i].RadarAddressEve;
                radarAddr.Uid = i;
                _radarAddrList.Add(radarAddr);
                radarsListView.Items.Add(_radarAddrList[i]);
            }
            if (radarsListView.SelectedIndex != _radarsInfo.SelectedIndex)
                radarsListView.SelectedIndex = _radarsInfo.SelectedIndex;
            updateSelectedRadar();
            updateRadarStatus();
        }

        #endregion

        #region 雷达列表选择项更新
        private void radarsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            resetRadarListView();
            initRadarChannels();
        }

        private void radarsListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            object item = ListViewBaseInfo.GetElementFromPoint((ItemsControl)sender, e.GetPosition((ItemsControl)sender));
            if (item != null)
                _radarsInfo.CenteredRadar();
        }
        private void initCamboBox()
        {
            int count = RadarColorTableDataInfo.Instance.Count;
            camboBoxColorTableSources.Clear();
            for (int i = 0; i < count; i++)
                camboBoxColorTableSources.Add(RadarColorTableDataInfo.Instance.GetTableDataConfig(i).Heading);
        }
        void resetRadarListView()
        {
            if (_radarsInfo.SelectedIndex != radarsListView.SelectedIndex)
            {
                initSelectedRadar();
                _radarsInfo.SelectedIndex = radarsListView.SelectedIndex;
                updateSelectedRadar();
            }
            updateRadarStatus();
        }

        private void initSelectedRadar()
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
                updateRadarListViewValue(_radarInfo);
            }
        }
        void updateRadarStatus()
        {
            updateRadarConfigListView();
            updateRadarStatusExtendedList();
        }

        private void updateRadarConfigListView()
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

        private void updateRadarStatusExtendedList()
        {
            lock (radarStatusListView)
            {
                int selectedIndex = radarStatusListView.SelectedIndex;
                radarStatusListView.Items.Clear();
                for (int i = 0; i < _radarStatus.Count; i++)
                {
                    radarStatusListView.Items.Add(_radarStatus[i]);
                }
                radarStatusListView.SelectedIndex = selectedIndex;
            }
        }

        private void updateRadarStatusBaseListView()
        {
            if(_radarInfo == null)
                return;
            var baseStatus = _radarInfo.RadarStatusBase;
            if (_radarConfig.Count < 8 || _radarStatus.Count < 10)
            {
                updateRadarStatus();
                return;
            }
            updateRadarConfigData(baseStatus);
        }

        private void updateRadarConfigData(RadarStatus baseStatus)
        {
            int index = 0;
            _radarConfig[index++].BaseValue = baseStatus.Name;
            _radarConfig[index++].BaseValue = baseStatus.Longitude.ToString();
            _radarConfig[index++].BaseValue = baseStatus.Latitude.ToString();
            _radarConfig[index++].BaseValue = baseStatus.Altitude.ToString();
            _radarConfig[index++].BaseValue = baseStatus.Range.ToString();
            _radarConfig[index++].BaseValue = baseStatus.StartRange.ToString();
            _radarConfig[index++].BaseValue = baseStatus.StartScanAngle.ToString();
            _radarConfig[index++].BaseValue = baseStatus.OffsetAngle.ToString();
        }

        private void updateRadarStatusData(RadarStatus baseStatus)
        {
            int index = 0;
            _radarStatus[index++].BaseValue = baseStatus.BearingPulseStatus.ToString();
            _radarStatus[index++].BaseValue = baseStatus.ShipHeadingMarkerStatus.ToString();
            _radarStatus[index++].BaseValue = baseStatus.TriggerStatus.ToString();
            _radarStatus[index++].BaseValue = baseStatus.InterruptStatus.ToString();
            _radarStatus[index++].BaseValue = baseStatus.RoundPerMinute.ToString();
            _radarStatus[index++].BaseValue = baseStatus.PulseRepetitionRate.ToString();
            _radarStatus[index++].BaseValue = baseStatus.BPPerSweep.ToString();
            _radarStatus[index++].BaseValue = baseStatus.ADBits.ToString();
            _radarStatus[index++].BaseValue = baseStatus.SampleCount.ToString();
            _radarStatus[index++].BaseValue = baseStatus.ScanLineCount.ToString();
        }

        #endregion

        #region 雷达参数设置
        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            var winAdd = new ConfigConnectionSettingClient("新增雷达设置", new RadarConnection());
            if (winAdd.ShowDialog().Value)
            {
                RadarConnection radarConnection = new RadarConnection() { Ip = winAdd.Config.Ip, Port = winAdd.Config.Port, RpcEndPoint = winAdd.Config.RpcEndPoint };
                _radarsInfo.Add(radarConnection);
                _radarsInfo.Save();
                radarsListView.SelectedIndex = _radarsInfo.Count - 1;
            }
        }

        private void btDelete_Click(object sender, RoutedEventArgs e)
        {
            if (radarsListView.SelectedIndex >= 0)
            {
                if (MessageBox.Show("确认删除选中雷达？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.Cancel) != MessageBoxResult.OK)
                    return;
                _radarsInfo.RemoveAt(radarsListView.SelectedIndex);
                _radarsInfo.Save();
                if (radarsListView.Items.Count > 0)
                    radarsListView.SelectedIndex = 0;
                else
                    radarsListView.SelectedIndex = -1;
                resetRadarListView();
            }
            else
            {
                MessageBox.Show("请选中需要删除的雷达！");
            }
        }

        private void editMenu_Click(object sender, RoutedEventArgs e)
        {
            radarInfoEditMenu();
        }

        private void cbRadarIsEnable_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            int uid = Convert.ToInt32(cb.Tag.ToString()); //获取该行id
            _radarsInfo.Radars[uid].IsEnable = cb.IsChecked.Value;
            _radarsInfo.Save();
            _radarsInfo.RadarEnableChanged();
        }
        
        private void ColorTableComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var data = sender as ComboBox;
            if (data != null)
            {
                int index = (int)data.Tag;
                if (index >= 0 && index < _radarAddrList.Count && _radarAddrList[index].ColorTableIndex != _radarsInfo.Radars[index].RadarAddress.ColorTableIndex)
                {
                    _radarsInfo.Radars[index].RadarAddress = _radarAddrList[index];
                    _radarsInfo.Save();
                }
            }
        }

        void radarInfoEditMenu()
        {
            if (_radarInfo != null)
            {
                var winAdd = new ConfigConnectionSettingClient("雷达信息编辑", new RadarConnection());
                winAdd.InitConfig(_radarInfo.RadarAddress);
                if (winAdd.ShowDialog().Value)
                {
                    if (_radarInfo.RadarAddress.IsSquels(winAdd.Config))
                        return;
                    _radarInfo.RadarAddress = new RadarConnection(winAdd.Config);
                    _radarsInfo.Save();
                }
            }
        }

        #endregion
        
        #region Radar Channels


        private void btFilter_Click(object sender, RoutedEventArgs e)
        {
            Button bt = sender as Button;
            if(bt != null && _radarInfo != null)
            {
                int port = (int)bt.Tag;
                var channels = _radarInfo.RadarChannelInfos;
                for(int i = 0; i < channels.Length; i++)
                {
                    if(channels[i].LegacyPort == port)
                    {
                        updateChannelFilter(i);
                        break;
                    }
                }
            }
        }
        private void channelRenamingMenu_Click(object sender, RoutedEventArgs e)
        {
            if (_radarInfo != null && radarChannelListView.SelectedIndex >= 0)
            {
                int index = radarChannelListView.SelectedIndex;
                ModifyValueClient client = new ModifyValueClient("通道名称修改", _radarInfo.RadarChannelInfos[index].Name, "通道名称：");
                if(client.ShowDialog().Value)
                    updateChannelName(client.Heading);
            }
        }

        private void channelRePortMenu_Click(object sender, RoutedEventArgs e)
        {
            if (_radarInfo != null && radarChannelListView.SelectedIndex >= 0)
            {
                int index = radarChannelListView.SelectedIndex;
                ModifyValueClient client = new ModifyValueClient("输出端口修改", _radarInfo.RadarChannelInfos[index].LegacyPort.ToString(), "输出端口：");
                if (client.ShowDialog().Value)
                {
                    int port;
                    if (int.TryParse(client.Heading, out port))
                        updateChannelPort(port);

                }
            }
        }

        private void btChannelSave_Click(object sender, RoutedEventArgs e)
        {
            if(_radarInfo != null)
                _radarInfo.SetRadarChannals();
        }


        private void btChannelReset_Click(object sender, RoutedEventArgs e)
        {
            if (_radarInfo != null)
                _radarInfo.ResetRadarChannals();
        }
        
        private void btChannelAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_radarInfo != null)
            {
                if (_radarInfo.IsEnable)
                {
                    RadarChannel channel = new RadarChannel() { Name = "新增滤波器", LegacyPort = 34000 };
                    _radarInfo.AddRadarChannals(channel);
                }
                else
                    MessageBox.Show("当前雷达未启用，无法进行通道修改！");
            }
            else
                MessageBox.Show("未选择任何任务，无法进行通道修改！");
        }

        private void btChannelDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_radarInfo != null && radarChannelListView.SelectedIndex >= 0)
            {
                if (MessageBox.Show("确认删除该雷达通道?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.Cancel) == MessageBoxResult.OK)
                {
                    _radarInfo.DeleteRadarChannals(radarChannelListView.SelectedIndex);
                }
            }
        }

        private void channelReFilterMenu_Click(object sender, RoutedEventArgs e)
        {
            updateChannelFilter(radarChannelListView.SelectedIndex);
        }

        private void channelSelectedMenu_Click(object sender, RoutedEventArgs e)
        {
            if (_radarInfo != null && radarChannelListView.SelectedIndex >= 0)
            {
                var addr = _radarInfo.RadarAddress;
                addr.Port = _radarInfo.GetRadarChannelInfo(radarChannelListView.SelectedIndex).LegacyPort;
                _radarInfo.RadarAddress = addr;
                _radarsInfo.Save();
                int selectedIndex = _radarsInfo.SelectedIndex;
                loadListView();
                radarsListView.SelectedIndex = selectedIndex;
            }
        }
        
        bool updateChannelName(string name)
        {
            int index = radarChannelListView.SelectedIndex;
            for (int i = 0; i < _radarInfo.RadarChannelInfos.Length; i++)
            {
                if (i != index)
                {
                    if (name == _radarInfo.RadarChannelInfos[i].Name)
                    {
                        MessageBox.Show("该名称已存在，通道名称修改失败！");
                        return false;
                    }
                }
            }
            _radarInfo.RadarChannelInfos[index].Name = name;
            return true;
        }

        bool updateChannelPort(int port)
        {
            try
            {
                int index = radarChannelListView.SelectedIndex;
                for (int i = 0; i < _radarInfo.RadarChannelInfos.Length; i++)
                {
                    if (i != index)
                    {
                        if (port == _radarInfo.RadarChannelInfos[i].LegacyPort)
                        {
                            MessageBox.Show("该端口已存在，输出端口修改失败！");
                            return false;
                        }
                    }
                }
                _radarInfo.RadarChannelInfos[index].LegacyPort = port;
                int selectedIndex = radarsListView.SelectedIndex;
                loadListView();
                radarsListView.SelectedIndex = selectedIndex;
                return true;
            }
            catch { }
            return false;
        }

        void updateChannelFilter(int index)
        {
            if (_radarInfo != null && index >= 0)
            {
                FiltersConfigDialog configDlg = new FiltersConfigDialog();
                configDlg.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                try
                {
                    configDlg.Filters = SpringXmlToObject.FromString<RadarFilters>(_radarInfo.RadarChannelInfos[index].Filter);
                }
                catch (Exception ex)
                {
                    LogService.Warn(ex.ToString());
                }

                if (configDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    XDocument xml = ObjectToSpringXml.Default.ToXml(configDlg.Filters);
                    _radarInfo.RadarChannelInfos[index].Filter = xml.ToString();
                }
            }
        }
        
        private void initRadarChannels()
        {
            radarChannelListView.Items.Clear();
            if (_radarInfo != null && _radarInfo.RadarChannelInfos != null)
            {
                var channels = _radarInfo.RadarChannelInfos;
                for (int i = 0; i < channels.Length; i++)
                {
                    radarChannelListView.Items.Add(channels[i]);
                }
                radarChannelListView.SelectedIndex = _radarInfo.RadarChannelSelectedIndex;
            }
        }

        #endregion

        #region 雷达参数部分读写

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
                catch(Exception ex)
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

        double doubleParse(string str)
        {
            if (string.IsNullOrEmpty(str))
                return 0;
            else
                return double.Parse(str);
        }

        private void setRadarConfigFromEditing(RadarSettingInfo radar)
        {
            _radarConfig.Clear();
            if (radar != null && radar.RadarStatusBase != null && radar.RadarStatus != null)
            {
                var baseStatus = radar.RadarStatusBase;
                var radarStatus = radar.RadarStatus;
                _radarConfig.Add(new DataEditUnitObj() { Heading = "雷达名称", BaseValue = baseStatus.Name, EditingValue = radarStatus.Name });
                _radarConfig.Add(new DataEditUnitObj() { Heading = "经度", BaseValue = baseStatus.Longitude.ToString(), EditingValue = radarStatus.Longitude.ToString() });
                _radarConfig.Add(new DataEditUnitObj() { Heading = "纬度", BaseValue = baseStatus.Latitude.ToString(), EditingValue = radarStatus.Latitude.ToString() });
                _radarConfig.Add(new DataEditUnitObj() { Heading = "高度", BaseValue = baseStatus.Altitude.ToString(), EditingValue = radarStatus.Altitude.ToString() });
                _radarConfig.Add(new DataEditUnitObj() { Heading = "量程", BaseValue = baseStatus.Range.ToString(), EditingValue = radarStatus.Range.ToString() });
                _radarConfig.Add(new DataEditUnitObj() { Heading = "量程开始", BaseValue = baseStatus.StartRange.ToString(), EditingValue = radarStatus.StartRange.ToString() });
                _radarConfig.Add(new DataEditUnitObj() { Heading = "起始角度", BaseValue = baseStatus.StartScanAngle.ToString(), EditingValue = radarStatus.StartScanAngle.ToString() });
                _radarConfig.Add(new DataEditUnitObj() { Heading = "偏移角度", BaseValue = baseStatus.OffsetAngle.ToString(), EditingValue = radarStatus.OffsetAngle.ToString() });
            }
        }

        private void updateRadarListViewValue(RadarSettingInfo radar)
        {
            _radarStatus.Clear();
            if (radar != null && radar.RadarStatusBase != null && radar.RadarStatus != null)
            {
                var baseStatus = radar.RadarStatusBase;
                var radarStatus = radar.RadarStatus;
                _radarStatus.Add(new DataEditUnitObj() { Heading = "BP信号", BaseValue = baseStatus.BearingPulseStatus.ToString() });
                _radarStatus.Add(new DataEditUnitObj() { Heading = "船首信号", BaseValue = baseStatus.ShipHeadingMarkerStatus.ToString() });
                _radarStatus.Add(new DataEditUnitObj() { Heading = "Trigger信号", BaseValue = baseStatus.TriggerStatus.ToString() });
                _radarStatus.Add(new DataEditUnitObj() { Heading = "Interrupt信号", BaseValue = baseStatus.InterruptStatus.ToString() });
                _radarStatus.Add(new DataEditUnitObj() { Heading = "天线转速", BaseValue = baseStatus.RoundPerMinute.ToString() });
                _radarStatus.Add(new DataEditUnitObj() { Heading = "Trigger频率", BaseValue = baseStatus.PulseRepetitionRate.ToString() });
                _radarStatus.Add(new DataEditUnitObj() { Heading = "一圈BP数信号", BaseValue = baseStatus.BPPerSweep.ToString() });
                _radarStatus.Add(new DataEditUnitObj() { Heading = "A/D精度", BaseValue = baseStatus.ADBits.ToString() });
                _radarStatus.Add(new DataEditUnitObj() { Heading = "扫描点数", BaseValue = baseStatus.SampleCount.ToString() });
                _radarStatus.Add(new DataEditUnitObj() { Heading = "扫描线数", BaseValue = baseStatus.ScanLineCount.ToString() });
            }
        }

        #endregion

        #region 按键控制

        private void btSaveRadarStatus_Click(object sender, RoutedEventArgs e)
        {
            if (_radarInfo != null)
            {
                _radarInfo.SaveRadarConfig();
                updateRadarInfoFromClient();
            }
        }

        private void btResetRadarStatus_Click(object sender, RoutedEventArgs e)
        {
            if (_radarInfo != null)
                _radarInfo.ResetRadarStatus();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _radarsInfo.SaveConformIfChanged();
            _radarsInfo.SetConfigIfChanged();
            VTSCore.Common.WindowFrontor.Instance.FrontWindow();
        }

        #endregion
    }
}
