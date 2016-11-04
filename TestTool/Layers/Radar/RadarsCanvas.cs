using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using VTSCore.Data.Common;
using VTSCore.Layers.Maps;

namespace VTSCore.Layers.Radar
{
    class RadarsCanvas : Canvas, IDisposable
    {
        LocatorAndBorder _locator;
        bool _isLocked = false;
        RadarsSettingInfo _ri;
        List<RadarCanvas> _radars = new List<RadarCanvas>();
        RadarCanvas _radarPrev;
        ActivatingStatus _activatingStatus;
        public RadarsCanvas(LocatorAndBorder locator)
        {
            _winformWindow = (System.Windows.Interop.HwndSource.FromDependencyObject(this) as System.Windows.Interop.HwndSource);
            _locator = locator;
            _activatingStatus = ActivatingStatus.Instance;
            initRadarDataInfomation();
            if (VTSCore.Layers.Plotting.PlottingAreaSettingInfomation.Instance.Locator == null)
                VTSCore.Layers.Plotting.PlottingAreaSettingInfomation.Instance.Locator = _locator.Locator;
            VTSCore.Layers.Plotting.PlottingAreaSettingInfomation.Instance.InputLocalRadarAreaData();
        }

        private void initRadarDataInfomation()
        {
            _ri = RadarsSettingInfo.Instance;
            _ri.Reset();
            resetRadarsSettingInfo();
            _ri.PropertyChanged += _ri_PropertyChanged;
        }

        private void resetRadarsSettingInfo()
        {
            _ri.SelectedIndex = -1;
            this.Children.Clear();
            _radars.Clear();
            for (int i = 0; i < _ri.Radars.Length; i++)
                addNewRadar(i);
        }

        private void addNewRadar(int index)
        {
            if (index >= 0 && index <= _radars.Count)
            {
                RadarCanvas radar = new RadarCanvas(_locator, _ri.Radars[index]);
                _radars.Add(radar);
                this.Children.Add(radar);
            }
        }

        private void RemoveRadar(int index)
        {
            if (index >= 0 && index < _radars.Count)
            {
                var radar = _radars[index];
                this.Children.Remove(radar);
                _radars.RemoveAt(index);
                radar.Dispose();
            }
        }

        void _ri_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ReadAllConfig")
            {
                resetRadarsSettingInfo();
            }
            else if (e.PropertyName == "SelectedIndex")
            {
                if (_ri.SelectedIndex >= 0 && _ri.SelectedIndex < _radars.Count)
                    _radarPrev = _radars[_ri.SelectedIndex];
                else
                    _radarPrev = null;
            }
            else if (e.PropertyName == "AddRadar" || e.PropertyName == "RemoveRadar")
            {
                updateChangedMuster();
            }
        }

        private void updateChangedMuster()
        {
            while (_ri.ChangedMuster.Count > 0)
            {
                if (_ri.ChangedMuster[0].Status == RadarChangedStatus.新增)
                {
                    addNewRadar(_ri.ChangedMuster[0].Id);
                    _radars[_radars.Count - 1].OnScreenChanged(ScreenWidth, ScreenHeight);
                }
                else if (_ri.ChangedMuster[0].Status == RadarChangedStatus.删除)
                    RemoveRadar(_ri.ChangedMuster[0].Id);
                _ri.ChangedMuster.RemoveAt(0);
            }
        }

        #region 操作雷达
        public bool InRadarCoverageArea(MapPoint position)
        {
            if (_radarPrev != null && _radarPrev.InRadarCoverageArea(position))
                return true;
            else
                return false;
        }
        public void Offset(double x, double y)
        {
            if (_radarPrev != null)
            {
                var locator = _locator.Locator;
                var loc0 = locator.ScreenToMap(0, 0);
                var loc1 = locator.ScreenToMap(x, y);
                double lon = loc1.Lon - loc0.Lon;
                double lat = loc1.Lat - loc0.Lat;
                _radarPrev.Offset(lon, lat);
            }
        }

        public void Transform(double x, double y)
        {
            if(_radarPrev != null)
            {
                _radarPrev.Transform(x, y);
            }
        }


        public void ClearCache()
        {
        }

        public bool LockAll
        {
            set 
            {
                if (_isLocked != value)
                {
                    for (int i = 0; i < _radars.Count; i++)
                        _radars[i].LockAll = value;
                    _isLocked = value;
                }
            }
        }

        public void Drag(MapPoint mousePosition, MapPoint endPostion)
        {
            if (_radarPrev != null)
                _radarPrev.Drag(mousePosition, endPostion);
        }
        int ScreenWidth;
        int ScreenHeight;

        public void OnScreenChanged(int width, int height)
        {
            ScreenWidth = width;
            ScreenHeight = height;
            foreach(var radar in _radars)
            {
                radar.OnScreenChanged(width, height);
            }
        }

        #endregion

        #region 雷达操作窗口
        System.Windows.Interop.HwndSource _winformWindow;

        public void InitHwndSourse(System.Windows.Interop.HwndSource source)
        {
            _winformWindow = source;
        }

        RadarSettingClient _radarSetting;
        public void RadarEditorWindow()
        {
            if (!VTSCore.Common.WindowStateDetect.ShowWindow(_radarSetting))
            {
                _radarSetting = new RadarSettingClient();
                setWindowSetting(_radarSetting);
                _radarSetting.Show();
            }
            else
                _radarSetting.WindowState = WindowState.Normal;
        }

        private void setWindowSetting(Window win)
        {
            if (_winformWindow != null)
                new System.Windows.Interop.WindowInteropHelper(win) { Owner = _winformWindow.Handle };
        }
        #endregion
        public void SetConfigIfChanged()
        {
            _ri.SetConfigIfChanged();
        }
        public void Dispose()
        {
            for(int i = 0; i < _radars.Count; i++)
                _radars[i].Dispose();
        }
    }
}