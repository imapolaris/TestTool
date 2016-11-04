using System;
using System.Windows;
using System.Windows.Controls;
using TargetInfomation;
using VTSCore.Data.Common;
using VTSCore.Layers.Maps;

namespace VTSCore.Layers.Tracks
{
    public abstract class TracksCanvasDrawer : Canvas, IDisposable
    {
        protected TracksCanvas _tracksData;
        protected TracksCanvas _tracksShow;
        protected LocatorAndBorder _locator;
        MenuBarsBaseInfo _menuBarsInfo;
        System.Threading.Timer _timer;
        SelectedTargetDraw _selectedTargetDraw;
        public DynamicBaseCanvas.GeometryType EnumerateType { set { _tracksData.EnumerateType = value; } }
        public TracksCanvasDrawer(LocatorAndBorder locator)
        {
            _locator = locator;
            _tracksData = new TracksCanvas(_locator);
            _tracksShow = _tracksData;
            if (IsVisibility)
                addChildren(_tracksShow);
            _tracksData.SetColor(_fill, _stroke);
            _locator.OnMapRefreshed += OnMapRefreshed;
            _menuBarsInfo = MenuBarsBaseInfo.Instance;
            _menuBarsInfo.PropertyChanged += menuBarsInfo_PropertyChanged;
            _timer = new System.Threading.Timer(callbackUpdateTimeOutTracks, null, 1000, 1000);
            OnMapRefreshed();
            _selectedTargetDraw = SelectedTargetDraw.Instance;
        }

        private void menuBarsInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ClearCache")
                Clear();
            else if (e.PropertyName == "LockAll")
                LockData = _menuBarsInfo.LockAll;
            else if (e.PropertyName == "ShowAllHistoryTrackLine")
                _locator.UpdateLocator();
        }

        #region 超时设置

        private void callbackUpdateTimeOutTracks(object state)
        {
            var time = DateTime.Now;
            updateTimeout(time);
        }

        void updateTimeout(DateTime time)
        {
            _tracksData.UpdateTimeOut(time);
        }

        public TimeSpan TimeOutHide
        {
            get { return _tracksData.TimeOutHide; }
            set { _tracksData.TimeOutHide = value; }
        }

        #endregion

        public virtual void SetConfig(string ip, int port)
        {
            System.Windows.MessageBox.Show("配置数据格式有误，请重新配置！");
        }
        public virtual void SetConfig(string url)
        {
            System.Windows.MessageBox.Show(url + Environment.NewLine + "配置数据格式有误，请重新配置！");
        }

        public virtual void SetSetting(string setting)
        {
            string ip;
            int port;
            if (DataEligibleDetection.GetIpPort(setting, out ip, out port))
                SetConfig(ip, port);
            else if (DataEligibleDetection.GetIpEndPoint(setting))
                SetConfig(setting);
            else
                System.Windows.MessageBox.Show(setting + Environment.NewLine + "配置数据不合法，请重新配置！");
        }

        public void OnMapRefreshed()
        {
            if (IsVisibility)
                _tracksShow.OnRefreshed();
        }

        public void Clear()
        {
            lock (this)
            {
                var timeoutHide = _tracksData.TimeOutHide;
                var type = _tracksData.EnumerateType;
                removeChildren(_tracksShow);
                removeChildren(_tracksData);
                _tracksData = new TracksCanvas(_locator);
                _tracksData.TimeOutHide = timeoutHide;
                _tracksData.SetColor(_fill, _stroke);
                _tracksShow = _tracksData;
                if (IsVisibility)
                    addChildren(_tracksShow);
                EnumerateType = type;
            }
        }

        bool _dataLock = false;
        public bool LockData
        {
            get { return _dataLock; }
            set
            {
                lock (this)
                {
                    MenuBarsBaseInfo.Instance.Tracking = false;
                    _dataLock = value;
                    string id = null;
                    if (_tracksShow != null && _tracksShow.SelectedId != null && _selectedTargetDraw.SelectedTrack != null && _selectedTargetDraw.SelectedTrack.GetTarget().GetId() == _tracksShow.SelectedId)
                        id = _tracksShow.SelectedId;
                    if (this.Children.Contains(_tracksShow))
                        this.Children.Remove(_tracksShow);
                    updateShow();
                    if (id != null && IsVisibility)
                    {
                        TrackCanvas track = _tracksShow.GetTrackFromId(id);
                        if (track.Selected)
                            _selectedTargetDraw.SelectedTrack = track;
                    }
                }
            }
        }

        private void updateShow()
        {
            if (LockData)
                updateShowLocked();
            else
                updateShowUnlocked();
            if (IsVisibility)
                addChildren(_tracksShow);
        }

        System.Windows.Media.SolidColorBrush _fill = System.Windows.Media.Brushes.WhiteSmoke;
        System.Windows.Media.SolidColorBrush _stroke = System.Windows.Media.Brushes.Black;
        public void SetColor(System.Windows.Media.SolidColorBrush fill, System.Windows.Media.SolidColorBrush stroke)
        {
            _fill = fill;
            _stroke = stroke;
            _tracksShow.SetColor(_fill, _stroke);
        }

        public void UpdateSelected(Point point)
        {
            if (!IsVisibility)
                return;
            lock (_tracksShow)
            {
                _tracksShow.UpdateSelectedTarget(point, _selectedTargetDraw);
            }
        }

        public IMovableTarget[] GetTargetsAtPoint(Point pt)
        {
            if (!IsVisibility)
                return new MovableTarget[0];
            lock (_tracksShow)
            {
                return _tracksShow.GetTargetsAtPoint(pt);
            }
        }

        private void updateShowLocked()
        {
            _tracksShow = _tracksData.Clone();
            _tracksShow.StopTimer();
        }

        private void updateShowUnlocked()
        {
            if (_tracksShow != null)
                _tracksShow.Dispose();
            _tracksShow = _tracksData;
        }

        bool _visibility;
        public bool IsVisibility { 
            get { return _visibility; }
            set { 
                if(IsVisibility != value)
                {
                    _visibility = value;
                    if(IsVisibility)
                    {
                        addChildren(_tracksShow);
                        OnMapRefreshed();
                    }
                    else
                    {
                        _tracksShow.InvisibleSelected();
                        removeChildren(_tracksShow);
                    }
                }
            }
        }

        void addChildren(UIElement element)
        {
            if (!this.Children.Contains(element))
                this.Children.Add(element);
        }

        void removeChildren(UIElement element)
        {
            if (this.Children.Contains(element))
                this.Children.Remove(element);
        }

        public virtual void Dispose()
        {
            if (_timer != null)
                _timer.Dispose();
            _timer = null;
            Clear();
        }
    }
}
