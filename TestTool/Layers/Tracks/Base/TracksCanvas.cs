using DynamicBaseCanvas;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TargetInfomation;
using VTSCore.Data.Common;
using VTSCore.Layers.Maps;

namespace VTSCore.Layers.Tracks
{
    public class TracksCanvas : Canvas, IDisposable
    {
        private ConcurrentDictionary<string, TrackCanvas> _dynamicObjectEvent = new ConcurrentDictionary<string, TrackCanvas>();
        LocatorAndBorder _locator;
        StatusBarBaseInfomation _statusBarInfomation;
        System.Windows.Media.SolidColorBrush _fill = null;
        System.Windows.Media.SolidColorBrush _stroke = System.Windows.Media.Brushes.Black;
        public GeometryType EnumerateType { get; set; }

        public TracksCanvas(LocatorAndBorder locator)
        {
            _locator = locator;
            _statusBarInfomation = StatusBarBaseInfomation.Instance;
            EnumerateType = GeometryType.Triangle;
            MenuBarsBaseInfo.Instance.PropertyChanged += menuBarsBaseInfo_PropertyChanged;
        }

        public void UpdateStaticEvent(MovableTarget target)
        {
            this.Dispatcher.BeginInvoke((Action)delegate()
            {
                lock (this)
                {
                    TrackCanvas track = getTrackAnyTime(target.GetId());
                    if (track != null)
                        track.UpdateStaticEvent(target);
                }
            });
        }

        public void UpdateDynamicEvent(MovableTarget target)
        {
            if (target.Lon <= -180 || target.Lon > 180 || target.Lat < -90 || target.Lat > 90)
                return;
            this.Dispatcher.BeginInvoke((Action)delegate()
            {
                lock (this)
                {
                    TrackCanvas track = getTrackAnyTime(target.GetId());
                    if (track != null)
                        track.UpdateDynamicEvent(target);
                }
            });
        }

        public void UpdateDynamicEvent(MovableTarget target, bool identified)
        {
            if (target.Lon <= -180 || target.Lon > 180 || target.Lat < -90 || target.Lat > 90)
                return;
            this.Dispatcher.BeginInvoke((Action)delegate()
            {
                lock (this)
                {
                    TrackCanvas track = getTrackAnyTime(target.GetId());
                    if (track == null)
                        return;
                    track.OpacityInfo = identified ? 1 : 0.4;
                    track.IsFill = !identified;
                    track.UpdateDynamicEvent(target);
                    if (!identified && MenuBarsBaseInfo.Instance.OnlyShowIdentifiedTrack)
                        track.Visibility = System.Windows.Visibility.Collapsed;
                    else
                        track.Visibility = System.Windows.Visibility.Visible;
                }
            });
        }

        public void UpdateIdentifiedTrack(bool onlyShowIdentifiedTrack)
        {
            this.Dispatcher.BeginInvoke((Action)delegate()
            {
                lock (this)
                {
                    _dynamicObjectEvent.Values.ToList().ForEach(track =>
                    {
                        if (!onlyShowIdentifiedTrack)
                            track.Visibility = System.Windows.Visibility.Visible;
                        else
                        {
                            if (track.OpacityInfo < 1)
                                track.Visibility = System.Windows.Visibility.Collapsed;
                            else
                                track.Visibility = System.Windows.Visibility.Visible;
                        }
                    });
                }
            });
        }
                        
        public void UpdateName(int trackID, string name)
        {
            this.Dispatcher.BeginInvoke((Action)delegate()
            {
                lock (_dynamicObjectEvent)
                {
                    TrackCanvas track = getTrackAnyTime(trackID.ToString());
                    if (track == null)
                        return;
                    var target = track.GetTarget() as MovableTarget;
                    if (target != null)
                        target.Name = name;
                }
            });
        }

        public void Remove(string id)
        {
            this.Dispatcher.BeginInvoke((Action)delegate()
            {
                remove(id);
            });
        }

        private void remove(string id)
        {
            lock (this)
            {
                if (id == SelectedId)
                    SelectedId = null;
                TrackCanvas track;
                if (_dynamicObjectEvent.TryRemove(id, out track))
                {
                    this.Children.Remove(track);
                    track.Dispose();
                }
            }
        }

        public void OnRefreshed()
        {
            this.Dispatcher.BeginInvoke((Action)delegate()
            {
                lock (this)
                {
                    foreach (var key in _dynamicObjectEvent.Keys)
                    {
                        TrackCanvas track;
                        if (_dynamicObjectEvent.TryGetValue(key, out track))
                            track.UpdateShow();
                    }
                }
            });
        }
        /// <summary>
        /// 数据备份
        /// </summary>
        /// <param video="clone">clone data</param>
        public TracksCanvas Clone()
        {
            lock (this)
            {
                TracksCanvas clone = new TracksCanvas(_locator);
                clone.SetColor(_fill, _stroke);
                foreach (var target in _dynamicObjectEvent)
                {
                    TrackCanvas track = target.Value.Clone();
                    track.OpacityInfo = target.Value.OpacityInfo;
                    if (clone._dynamicObjectEvent.TryAdd(target.Key, track))
                        clone.Children.Add(track);
                }
                return clone;
            }
        }

        public void SetColor(System.Windows.Media.SolidColorBrush fill, System.Windows.Media.SolidColorBrush stroke)
        {
            _fill = fill;
            _stroke = stroke;
        }

        void menuBarsBaseInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ShowAllHistoryTrackLine")
                updateAllLine(MenuBarsBaseInfo.Instance.ShowAllHistoryTrackLine);
        }

        private void updateAllLine(bool showAllHistoryTrackLine)
        {
            lock(this)
            {
                foreach(var track in _dynamicObjectEvent)
                {
                    track.Value.UpdateHistoryInfo(showAllHistoryTrackLine);
                }
            }
        }

        #region 超时设置

        TimeSpan _timeoutHide = new TimeSpan(0, 0, 10);
        public TimeSpan TimeOutHide
        {
            get { return _timeoutHide; }
            set { _timeoutHide = value; }
        }

        public void UpdateTimeOut(DateTime time)
        {
            if (TimeOutHide.Ticks == 0)
                return;
            
            this.Dispatcher.BeginInvoke((Action)delegate()
            {
                updateTimeOutOnlyHide(time);
            });
        }

        private void updateTimeOutOnlyHide(DateTime time)
        {
            lock(this)
            {
                DateTime timeBegin = time.Subtract(TimeOutHide);
                List<string> timeOutKey = new List<string>();
                foreach (var key in _dynamicObjectEvent.Keys)
                {
                    TrackCanvas target;
                    if (_dynamicObjectEvent.TryGetValue(key, out target))
                    {
                        if (target.GetTarget().UpdateTime < timeBegin)
                            timeOutKey.Add(key);
                    }
                }
                for (int i = 0; i < timeOutKey.Count; i++)
                    remove(timeOutKey[i]);
            }
        }
        #endregion

        private TrackCanvas getTrackAnyTime(string id)
        {
            TrackCanvas track = null;
            if (!_dynamicObjectEvent.TryGetValue(id, out track))
                addShow(id, ref track);
            return track;
        }

        private void addShow(string id, ref TrackCanvas track)
        {
            TrackCanvas target = new TrackCanvas(_locator, EnumerateType, _stroke, _fill, true);
            if (_dynamicObjectEvent.TryAdd(id, target))
            {
                track = target;
                this.Children.Add(target);
            }
        }
                
        #region 鼠标选中事件
        
        public void UpdateSelectedTarget(Point point, SelectedTargetDraw selectedTargetDraw)
        {
            string id = null;
            double dist = selectedTargetDraw.SelectedDistance;
            double lengthPrev = selectedTargetDraw.SelectedDistance;
            foreach (var key in _dynamicObjectEvent.Keys.ToArray())
            {
                if (isNeerPoint(key, point, ref lengthPrev))
                {
                    if (dist > lengthPrev)
                    {
                        id = key;
                        dist = lengthPrev;
                    }
                }
            }
            if (dist < selectedTargetDraw.SelectedDistance)
            {
                selectedTargetDraw.SelectedDistance = dist;
                selectedTargetDraw.SelectedTrack = _dynamicObjectEvent[id];
                SelectedId = id;
            }
            else
                SelectedId = null;
        }

        public TrackCanvas GetTrackFromId(string id)
        {
            TrackCanvas track;
            if (_dynamicObjectEvent.TryGetValue(id, out track))
            {
                SelectedId = id;
                return track;
            }
            return null;
        }

        public IMovableTarget[] GetTargetsAtPoint(Point pt)
        {
            List<IMovableTarget> targets = new List<IMovableTarget>();
            foreach (var key in _dynamicObjectEvent.Keys.ToArray())
            {
                double length = 10;
                if (isNeerPoint(key, pt, ref length))
                    targets.Add(_dynamicObjectEvent[key].GetTarget());
            }
            return targets.ToArray();
        }

        public string SelectedId { get; private set; }

        public void InvisibleSelected()
        {
            if(SelectedId != null)
            {
                TrackCanvas track;
                if(_dynamicObjectEvent.TryGetValue(SelectedId, out track))
                    track.Invisible();
                SelectedId = null;
            }
        }

        private bool isNeerPoint(string id, Point point, ref double lengthPrev)
        {
            TrackCanvas target;
            if(_dynamicObjectEvent.TryGetValue(id, out target))
                if (target.DisplayStatusPrev != TargetDisplayStatus.UnShow)
                {
                    double x = target.PointInScreen.X - point.X;
                    double y = target.PointInScreen.Y - point.Y;
                    double length = Math.Sqrt(x * x + y * y);
                    if (length < lengthPrev)
                    {
                        lengthPrev = length;
                        return true;
                    }
                }
            return false;
        }
        #endregion

        public void StopTimer()
        {
            TimeOutHide = new TimeSpan();
        }

        public void Dispose()
        {
            lock(this)
            {
                this.Children.Clear();
                foreach (var eve in _dynamicObjectEvent)
                    eve.Value.Dispose();
                SelectedId = null;
                _dynamicObjectEvent.Clear();
            }
        }
    }
}
