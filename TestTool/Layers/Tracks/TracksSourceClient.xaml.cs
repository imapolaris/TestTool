using Common.Logging;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VTSCore.Data.Common;
using VTSCore.Layers.Base;

namespace VTSCore.Layers.Tracks
{
    /// <summary>
    /// TracksSourceClient.xaml 的交互逻辑
    /// </summary>
    public partial class TracksSourceClient : Window
    {
        List<TrackSourceInfo> _sources = new List<TrackSourceInfo>();

        public Action<TrackSourceInfo> OnSourceChanged;
        ILog LogService { get { return LogManager.GetLogger(GetType()); } }
        public TracksSourceClient(List<TrackSourceInfo> sources)
        {
            InitializeComponent();
            updateSource(sources);
            updateSourceGridView();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        private void updateSource(List<TrackSourceInfo> sources)
        {
            _sources.Clear();
            if (sources == null)
                return;
            for(int i = 0; i < sources.Count; i++)
                _sources.Add(sources[i].Clone());
        }
        
        void updateSourceGridView()
        {
            sourceListView.Items.Clear();
            for (int i = 0; i < _sources.Count; i++ )
                sourceListView.Items.Add(_sources[i]);
        }

        #region 选中事件
        private void cbEnable_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var cb = sender as CheckBox;
                var source = _sources[(int)cb.Tag - 1];

                if (string.IsNullOrWhiteSpace(source.Setting))
                {
                    source.IsEnable = false;
                    cb.IsChecked = false;
                }
                updateSource(source);
            }
            catch(Exception ex)
            {
                (sender as CheckBox).IsChecked = false;
                LogService.Error(ex.ToString());
                MessageBox.Show(ex.Message);
            }
        }

        private void cbVisible_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var cb = sender as CheckBox;
                var source = _sources[(int)cb.Tag - 1];
                if(source.IsEnable)
                    updateSource(source);
            }
            catch (Exception ex)
            {
                LogService.Error(ex.ToString());
                MessageBox.Show(ex.Message);
            }
        }
        #endregion 选中事件

        private void sourceListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition((ItemsControl)sender);
            object item = ListViewBaseInfo.GetElementFromPoint((ItemsControl)sender, pt);
            if (item != null)
            {
                int columnIndex = getColumnIndex(pt.X);
                if(columnIndex == 2)
                    settingBox(sourceListView.SelectedIndex);
                else if(columnIndex == 5)
                    updateRemarks(sourceListView.SelectedIndex);
            }
        }

        int getColumnIndex(double x)
        {
            GridView gv = sourceListView.View as GridView;
            double count = 0;
            for (int i = 0; i < gv.Columns.Count; i++)
            {
                if (!Double.IsNaN(gv.Columns[i].Width))
                {
                    count += gv.Columns[i].Width;
                    if (count >= x)
                        return i;
                }
            }
            return -1;
        }
        private void resetsettingMenu_Click(object sender, RoutedEventArgs e)
        {
            settingBox(sourceListView.SelectedIndex);
        }

        private void resetRemarksMenu_Click(object sender, RoutedEventArgs e)
        {
            updateRemarks(sourceListView.SelectedIndex);
        }

        private void settingBox(int index)
        {
            if (index < 0)
                return;
            var source = _sources[index];
            switch(source.Type)
            {
                case "AIS":
                    updateIpPortFromSource(source, new AISConfig());
                    break;
                case "AtlasVTS":
                    updateIpPortFromSource(source, new AtlasVTSConfig());
                    break;
                case "Atlas2VTS":
                    updateIpPortFromSource(source, new Atlas2VTSConfig());
                    break;
                case "HittVTS":
                    updateIpPortFromSource(source, new HittVTSConfig());
                    break;
                case "NorcontrolVTS":
                    updateIpPortFromSource(source, new NorcontrolVTSConfig());
                    break;
                case "SofrelogVTS":
                    updateIpPortFromSource(source, new SofrelogVTSConfig());
                    break;
                case "视酷雷达识别信号":
                    updateIpPortFromSource(source, new SeecoolRadarConfig());
                    break;
                case "视酷视频分析信号":
                    updateIpPortFromSource(source, new VideoRecognizeConfig());
                    break;
                case "山东GPS":
                    updateIpPortFromSource(source, new ShanDongGPSConfig());
                    break;
                case "卫星AIS":
                    updateIpPortFromSource(source, new WeiXingAISConfig());
                    break;
                case "视酷VTS":
                    updateSubRpcFromSource(source, "tcp://127.0.0.1:16701,tcp://127.0.0.1:16702");
                    break;
                case "湖州GPS":
                    updateUrlFromSource(source, "http://172.21.25.33:30611/HZShipDataWebService.asmx");
                    break;
            }
        }

        private void updateUrlFromSource(TrackSourceInfo source, string defaultConfig = "")
        {
            string config = source.Setting;
            if (string.IsNullOrWhiteSpace(config))
                config = defaultConfig;
            StringSettingClient menu = new StringSettingClient(config, "Url配置:");
            if (menu.ShowDialog().Value)
            {
                source.Setting = menu.Data;
                if (source.IsEnable)
                    updateSource(source);
            }
        }

        private void updateSubRpcFromSource(TrackSourceInfo source, string defaultConfig = "")
        {
            string config = string.IsNullOrWhiteSpace(source.Setting) ? defaultConfig : source.Setting;
            string[] datas = config.Split(',');
            RadarMuxerClient client = new RadarMuxerClient(datas[0], datas[1]);
            if (client.ShowDialog().Value)
            {
                source.Setting = client.SubEndpoint + "," + client.RpcEndpoint;
                if(source.IsEnable)
                    updateSource(source);
            }
        }

        private void updateIpPortFromSource(TrackSourceInfo source, ConfigBase defaultConfig)
        {
            string config = string.IsNullOrWhiteSpace(source.Setting) ? defaultConfig.ToString() : source.Setting;
            string[] datas = config.Split(',');
            System.Windows.Interop.HwndSource winformWindow = (System.Windows.Interop.HwndSource.FromDependencyObject(this) as System.Windows.Interop.HwndSource);
            var client = new IpPortSettingClient(datas[0], datas[1], source.Type + "配置");
            if (client.ShowDialog().Value && source.Setting != client.Ip + "," + client.Port)
            {
                source.Setting = client.Ip + "," + client.Port;
                if (string.IsNullOrWhiteSpace(client.Ip) || string.IsNullOrWhiteSpace(client.Port))
                    source.IsEnable = false;
                if (string.IsNullOrWhiteSpace(client.Ip) && string.IsNullOrWhiteSpace(client.Port))
                    source.Setting = null;
                updateSource(source);
            }
        }

        private void updateRemarks(int index)
        {
            if (index < 0)
                return;
            var source = _sources[index];
            StringSettingClient client = new StringSettingClient(source.Remarks, "备注：", "修改备注");
            if(client.ShowDialog().Value)
            {
                source.Remarks = client.Data;
                updateSource(source);
            }
        }

        void updateSource(TrackSourceInfo source)
        {
            if (OnSourceChanged != null)
                OnSourceChanged(source);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            VTSCore.Common.WindowFrontor.Instance.FrontWindow();
        }
    }
}
