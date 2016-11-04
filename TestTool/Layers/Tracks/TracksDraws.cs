using Common.Logging;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TargetInfomation;
using VTSCore.Data.Common;
using VTSCore.Layers.Maps;
using VTSCore.Layers.Tracks.CCTV;

namespace VTSCore.Layers.Tracks
{
    class TracksDraws : Canvas
    {
        LocatorAndBorder _locatorAndLimit;
        TracksCanvasDrawer[] _trackSources;
        SelectedTargetDraw _selectedTargetDraw;
        CCTVNodeShow cctv;
        ILog LogService { get { return LogManager.GetLogger(GetType()); } }
        public TracksDraws(ILocator locator)
        {
            LogService.Info("初始化信号源配置");
            string path = System.IO.Directory.GetParent(System.Windows.Forms.Application.LocalUserAppDataPath).FullName;
            _locatorAndLimit = LocatorAndBorder.Instance;
            ConfigPath = System.IO.Path.Combine(path, "信号源配置.xml");
            loadConfig();
            cctv = new CCTVNodeShow(_locatorAndLimit);
            this.Children.Add(cctv);
            _selectedTargetDraw = SelectedTargetDraw.Instance;
            _selectedTargetDraw.Init(_locatorAndLimit);
            this.Children.Add(_selectedTargetDraw);
            Canvas.SetZIndex(cctv, 0);
            Canvas.SetZIndex(_selectedTargetDraw, 1000);
            MenuBarsBaseInfo.Instance.PropertyChanged += _menuBarsInfo_PropertyChanged;
            LogService.Info("初始化信号源配置完毕");
        }
        
        #region 鼠标事件、海图重绘
        public void CheckSelectedPoint(Point point)
        {
            _selectedTargetDraw.SelectedDistance = 10;
            foreach(var drawer in _trackSources)
            {
                if (drawer != null)
                    drawer.UpdateSelected(point);
            }
        }

        public void MouseRightButton(Point point, System.Windows.Controls.ContextMenu contextMenu)
        {
            var source = _trackSources[(int)TrackType.视酷VTS] as RadarMuxerTrackDrawer;
            if (source != null)
                source.UpdateContextMenu(contextMenu, point);
        }

        public void DoubleMouse(Point point)
        {
            _selectedTargetDraw.SelectedTrack = null;
            CheckSelectedPoint(point);
            if (cctv.Selected != null)
                cctv.ShowCCTVWindow(cctv.Selected);
        }

        #endregion 鼠标事件、 海图重绘

        #region 数据源配置及设置

        void _menuBarsInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SignalSourceSetting")
                signalSourceSetting();
        }

        string ConfigPath;
        List<TrackSourceInfo> _sources = new List<TrackSourceInfo>();
        void loadConfig()
        {
            loadSource();
            TrackSource[] trackSources = ConfigFile<TrackSource[]>.FromFile(ConfigPath);
            if(trackSources != null)
            {
                foreach (var source in trackSources)
                    updateSource(source);
            }
        }

        bool saveConfig()
        {
            List<TrackSource> sources = new List<TrackSource>();
            foreach(var source in _sources)
            {
                if (!string.IsNullOrWhiteSpace(source.Setting) || !string.IsNullOrWhiteSpace(source.Remarks))
                    sources.Add(new TrackSource() {  Type=source.Type, Setting = source.Setting, Remarks= source.Remarks, IsStartDefault = source.IsEnable, IsVisible = source.IsVisible});
            }
            return ConfigFile<TrackSource[]>.SaveToFile(ConfigPath, sources.ToArray());
        }

        private void updateSource(TrackSource source)
        {
            foreach (var s in _sources)
            {
                if (source.Type != s.Type)
                    continue;
                s.Setting = source.Setting;
                s.Remarks = source.Remarks;
                s.IsEnable = source.IsStartDefault;
                s.IsVisible = source.IsVisible;
                updateRunningStatus(s);
                return;
            }
        }

        TracksSourceClient _signalSourceClient;
        void signalSourceSetting()
        {
            if (!VTSCore.Common.WindowStateDetect.ShowWindow(_signalSourceClient))
            {
                _signalSourceClient = new TracksSourceClient(_sources);
                var winformWindow = (System.Windows.Interop.HwndSource.FromDependencyObject(this) as System.Windows.Interop.HwndSource);
                if (winformWindow != null)
                    new System.Windows.Interop.WindowInteropHelper(_signalSourceClient) { Owner = winformWindow.Handle };
                _signalSourceClient.Show();
                _signalSourceClient.OnSourceChanged += updateShowFromSource;
            }
        }

        void loadSource()
        {
            var length = System.Enum.GetNames(typeof(TrackType)).Length;
            _trackSources = new TracksCanvasDrawer[length - 1];
            _sources.Clear();
            for (int i = 0; i < _trackSources.Length; i++)
                _sources.Add(new TrackSourceInfo() { Uid = i + 1, Type = ((TrackType)i).ToString() });
        }

        private void updateShowFromSource(TrackSourceInfo source)
        {
            if (updateSourceData(source))
                updateRunningStatus(source);
        }

        private bool updateSourceData(TrackSourceInfo source)
        {
            var s = _sources[source.Uid - 1];
            if (s.Type != source.Type)//类型不同
                return false;
            bool canUpdateShow = s.Setting != source.Setting || s.IsEnable != source.IsEnable || s.IsVisible != source.IsVisible;
            if (!s.AreEqual(source))
            {
                _sources[source.Uid - 1] = source.Clone();
                saveConfig();
            }
            return canUpdateShow;
        }

        private async void updateRunningStatus(TrackSourceInfo source)
        {
            try
            {
                TrackType type = (TrackType)Enum.Parse(typeof(TrackType), source.Type);
                int index = (int)type;
                if (index >= _trackSources.Length)
                    return;
                await System.Threading.Tasks.Task.Yield();
                if (!source.IsEnable)
                    removeTrackSource(index);
                else
                    UpdateTrackSource(source, type, index);
            }
            catch (Exception ex)
            {
                LogService.Error(ex.ToString());
            }
        }

        private void UpdateTrackSource(TrackSourceInfo source, TrackType type, int index)
        {
            try
            {
                if (_trackSources[index] == null)
                {
                    _trackSources[index] = TrackDrawerSource.NewTrackDrawer(type, _locatorAndLimit);
                    this.Children.Add(_trackSources[index]);
                    if (type == TrackType.视酷VTS)
                        Canvas.SetZIndex(_trackSources[index], 100);
                }
                _trackSources[index].SetSetting(source.Setting);
                _trackSources[index].IsVisibility = source.IsVisible;
            }
            catch(Exception ex)
            {
                LogService.Error(ex.ToString());
            }
        }

        private void removeTrackSource(int index)
        {
            if (_trackSources[index] != null)
                _trackSources[index].Dispose();
            this.Children.Remove(_trackSources[index]);
            _trackSources[index] = null;
        }

        #endregion 数据源配置及设置
    }
}