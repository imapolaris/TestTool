using CCTVClient;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using TargetInfomation;
using VTSCore.Data.Common;
using VTSCore.Layers.Maps;

namespace VTSCore.Layers.Tracks.CCTV
{
    public class CCTVNodeShow : Canvas, IDisposable
    {
        LocatorAndBorder _locator;
        CCTVInfo _cctvInfo;
        TrackVideoForm _trackVideoForm;
        CCTVsCanvas _cctvShow;
        CCTVConfigForm form;
        CCTVNodeTree _nodeTree = new CCTVNodeTree();
        TrackAdjustment _adjustment;
        ConfigCCTV _config;
        Timer _timer;
        IMovableTarget _tracking;
        ulong _lastTrackVideoID = 0;
        private Dictionary<ulong, VideoParser.Camera> _videoRealtime = new Dictionary<ulong, VideoParser.Camera>();
        string ConfigPath;
        MenuBarsBaseInfo _menuBarsInfo;
        ILog LogService { get { return LogManager.GetLogger(GetType()); } }
        public CCTVNodeShow(LocatorAndBorder locator)
        {
            _locator = locator;
            _adjustment = new TrackAdjustment();
            string path = System.IO.Directory.GetParent(System.Windows.Forms.Application.LocalUserAppDataPath).FullName;
            ConfigPath = System.IO.Path.Combine(path, "CCTVConfig.xml");
            _config = ConfigFile<ConfigCCTV>.FromFile(ConfigPath);
            if (_config != null)
                updateCCTVInfo();
            _locator.OnMapRefreshed += onMapRefreshed;
            _menuBarsInfo = MenuBarsBaseInfo.Instance;
            _menuBarsInfo.PropertyChanged += _menuBarsInfo_PropertyChanged;
        }

        private void _menuBarsInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CCTVConfigSetting")
                showSettingWindow();
            else if (e.PropertyName == "Tracking")
                cctvTracking();
            else if (e.PropertyName == "CCTVTreeView")
                ShowTreeView();
        }

        private void cctvTracking()
        {
            if (_menuBarsInfo.Tracking)
                targetTrackingStart(_menuBarsInfo.TrackingTarget);
            else
                targetTrackingStop();
        }

        bool targetTrackingStart(IMovableTarget movableTarget)
        {
            if (_config == null)
                return false;
            timerDispose();
            _tracking = movableTarget;
            updateTrack(true);
            _timer = new Timer(timerTrackingCallBack, null, 0, 1000);
            return true;
        }
        
        void targetTrackingStop()
        {
            timerDispose();
            stopTrack();
            _adjustment.Height = 0;
            _adjustment.Latency = 0;
        }

        void showSettingWindow()
        {
            var config = ReadSetting(_config);
            if (config != null && !config.Equal(_config))
            {
                _config = config;
                ConfigFile<ConfigCCTV>.SaveToFile(ConfigPath, _config);
                updateCCTVInfo();
            }
        }

        public void ShowCCTVWindow(VideoParser.Video selected)
        {
            if(_trackVideoForm != null)
            {
                if (selected != null)
                {
                    try
                    {
                        _trackVideoForm.SetVideoID(selected.Id, selected.Name);
                        _trackVideoForm.Show();
                        updateCCTVFormOwner();
                    }
                    catch(Exception ex)
                    {
                        LogService.Error("CCTV播放失败！！" + Environment.NewLine + ex.ToString());
                        MessageBox.Show("CCTV播放失败！！");
                    }
                }
            }
        }

        public VideoParser.Video Selected
        {
            get { return _cctvShow != null ? _cctvShow.Selected : null; }
            set
            {
                if (_cctvShow == null)
                    return;
                lock (_cctvShow)
                    _cctvShow.Selected = value;
                if (Selected != null)
                    UpdateStatus();
            }
        }
        
        public void CheckNeerestPoint(Point point, ref double distCCTVNode)
        {
            if (_cctvShow == null)
                return;
            double dist = 1000;
            var video = _cctvShow.SelectedOnPoint(point, out dist);
            if (dist < distCCTVNode && dist < 20)
            {
                distCCTVNode = dist;
                Selected = video;
            }
        }

        public void UpdateStatus()
        {
            if (_cctvShow != null)
                _cctvShow.UpdateStatus();
        }
        CCTVTreeCtrlClient _treeClient;
        public void ShowTreeView()
        {
            if(_videoNode == null)
            {
                LogService.Error("获取到CCTV视频列表失败，请检查配置是否正确！");
                MessageBox.Show("获取到CCTV视频列表失败，请检查配置是否正确！");
                return;
            }
            if (!VTSCore.Common.WindowStateDetect.ShowWindow(_treeClient))
            {
                _treeClient = new CCTVTreeCtrlClient();
                System.Windows.Interop.HwndSource winformWindow = (System.Windows.Interop.HwndSource.FromDependencyObject(this) as System.Windows.Interop.HwndSource);
                if (winformWindow != null)
                    new System.Windows.Interop.WindowInteropHelper(_treeClient) { Owner = winformWindow.Handle };
                _treeClient.VideoNode = _videoNode;
                _treeClient.Show();
                _treeClient.OnPlayVadio += ShowCCTVWindow;
            }
        }

        public void Dispose()
        {
            timerDispose();
            if (_cctvInfo != null)
                _cctvInfo.Stop();
            if (_trackVideoForm != null)
                _trackVideoForm.Dispose();
            _trackVideoForm = null;
        }

        void onMapRefreshed()
        {
            if (_cctvShow != null)
                _cctvShow.OnMapRefreshed();
        }

        private void timerTrackingCallBack(object state)
        {
            updateTrack(false);
        }

        private void updateTrack(bool first)
        {
            if (_tracking != null)
            {
                VideoParser.Video video = getNearestVideo(_tracking.Lat, _tracking.Lon);
                if (video != null)
                {
                    _trackVideoForm.SetVideoID(video.Id, video.Name);
                    if (first)
                    {
                        _trackVideoForm.Show();
                        updateCCTVFormOwner();
                    }
                    double elapsed = (DateTime.Now - _tracking.UpdateTime).TotalSeconds + _adjustment.Latency;
                    double delta = _tracking.SOG / 216000.0 * elapsed;
                    double dy = delta * Math.Sin((90 - _tracking.COG) * Math.PI / 180.0);
                    double dx = delta * Math.Cos((90 - _tracking.COG) * Math.PI / 180.0);
                    double latitude = _tracking.Lat + dy;
                    double longitude = _tracking.Lon + dx / Math.Cos(latitude * Math.PI / 180.0);
                    const int targetLength = 40;
                    track(video.Id, _tracking.GetType().ToString(), _tracking.GetId(),
                        longitude, latitude, _adjustment.Height, first ? targetLength : 0, targetLength);
                }
            }
        }

        private void updateCCTVFormOwner()
        {
            System.Windows.Interop.HwndSource winformWindow = (System.Windows.Interop.HwndSource.FromDependencyObject(this) as System.Windows.Interop.HwndSource);
            if (winformWindow != null)
                WindowOwnerBinding.SetOwner(_trackVideoForm.Handle, winformWindow.Handle);
        }

        private void track(ulong videoID, string targetType, string targetID, double lon, double lat, double alt, int targetWidth, int targetWidth2)
        {
            if (videoID != _lastTrackVideoID)
                stopTrack();
            _lastTrackVideoID = videoID;
            _cctvInfo.TrackTarget(videoID, targetType, targetID, lon, lat, alt, targetWidth, targetWidth2);
        }

        private void stopTrack()
        {
            if (_cctvInfo != null)
                _cctvInfo.StopTrack(_lastTrackVideoID);
            _lastTrackVideoID = 0;
        }

        private VideoParser.Video getNearestVideo(double lat, double lon)
        {
            var videos = _nodeTree.GetAllTrackVideos();
            if (videos == null)
                return null;
            VideoParser.Video nearestVideo = null;
            double minDist = 5.0 / 60;
            foreach (var video in videos)
            {
                double dir = 90.0 - 180.0 / Math.PI * Math.Atan2(lat - video.PanTiltUnit.Latitude, lon - video.PanTiltUnit.Longitude);
                if (dir < 0)
                    dir += 360.0;
                double left = video.PanTiltUnit.LeftLimit;
                double right = video.PanTiltUnit.RightLimit;
                if (right < left)
                {
                    if (dir < right)
                        dir += 360.0;
                    right += 360.0;
                }
                if (dir > left && dir < right)
                {
                    double dist = getDist(new Point(video.PanTiltUnit.Longitude, video.PanTiltUnit.Latitude), new Point(lon, lat));
                    if (dist < minDist)
                    {
                        minDist = dist;
                        nearestVideo = video;
                    }
                }
            }
            return nearestVideo;
        }

        private double getDist(Point pt1, Point pt2)
        {
            double y = (pt2.Y - pt1.Y);
            double x = (pt2.X - pt1.X) * Math.Cos(pt1.Y * Math.PI / 180);
            return Math.Sqrt(x * x + y * y);
        }

        private void timerDispose()
        {
            if (_timer != null)
                _timer.Dispose();
            _timer = null;
        }

        private ConfigCCTV ReadSetting(ConfigCCTV config)
        {
            if (config == null)
                config = new ConfigCCTV();
            form = new VTSCore.Layers.Tracks.CCTV.CCTVConfigForm(config.Clone());
            System.Windows.Interop.HwndSource winformWindow = (System.Windows.Interop.HwndSource.FromDependencyObject(this) as System.Windows.Interop.HwndSource);
            if (winformWindow != null)
                new System.Windows.Interop.WindowInteropHelper(form) { Owner = winformWindow.Handle };
            if (form.ShowDialog().Value)
                return form.Config;
            else
                return null;
        }

        private void updateCCTVInfo()
        {
            if (_cctvInfo != null)
                _cctvInfo.Stop();
            _cctvInfo = null;
            if (_trackVideoForm != null)
                _trackVideoForm.Dispose();
            _trackVideoForm = null;
            if (_config == null)
                return;
            if (DataEligibleDetection.IsEffectIp(_config.Ip))
            {
                _cctvInfo = new CCTVInfo(_config.Ip);
                _cctvInfo.UserName = _config.User;
                _cctvInfo.Password = _config.Pass;
                _cctvInfo.NodeTreeEvent += _cctvInfo_NodeTreeEvent;
                _cctvInfo.RealtimeInfoEvent += _cctvInfo_RealtimeInfoEvent;
                _cctvInfo.Start();
                _trackVideoForm = new TrackVideoForm(null, _cctvInfo, _config.Bandwidth, _adjustment);
            }
            else
                MessageBox.Show("CCTV配置单Ip配置错误！");
        }

        private void _cctvInfo_RealtimeInfoEvent(VideoParser.Camera camera)
        {
            lock (_videoRealtime)
            {
                if (_videoRealtime.ContainsKey(camera.Id))
                    _videoRealtime[camera.Id] = camera;
                else
                    _videoRealtime.Add(camera.Id, camera);
                updateCameraAngle(camera);
            }
        }

        VideoParser.Node _videoNode;
        private void _cctvInfo_NodeTreeEvent(VideoParser.Node tree, string xml)
        {
            lock (_nodeTree)
            {
                bool isSameNode = _nodeTree.IsSameNode(tree);
                _nodeTree.UpdateTree(tree);
                _videoNode = tree;
                if (isSameNode)
                    updateNodesData();
                else
                    addCCTVsShow();
            }
        }

        private void updateCameraAngle(VideoParser.Camera camera)
        {
            if (_cctvShow != null)
                _cctvShow.UpdateCameraAngle(camera);
        }
        
        private void addCCTVsShow()
        {
            this.Dispatcher.BeginInvoke((Action)delegate()
            {
                VideoParser.Video selected = Selected;
                if (_cctvShow != null)
                    this.Children.Remove(_cctvShow);
                _cctvShow = new CCTVsCanvas(_locator, _nodeTree.GetAllTrackVideos(), _videoRealtime);
                this.Children.Add(_cctvShow);
                onMapRefreshed();
                Selected = selected;
            });
        }

        private void updateNodesData()
        {
            this.Dispatcher.BeginInvoke((Action)delegate()
            {
                _cctvShow.UpdateVideoNodes(_nodeTree.GetAllTrackVideos());
            });
        }
    }
}