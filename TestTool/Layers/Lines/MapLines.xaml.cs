using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
using VTSCore.Layers.Maps;

namespace VTSCore.Layers.Lines
{
    /// <summary>
    /// MapLines.xaml 的交互逻辑
    /// </summary>
    public partial class MapLines : Canvas
    {
        public MapLines()
        {
            InitializeComponent();
            this.ClipToBounds = true;
        }
        [Import]
        Maps.IMapNotification _mapNotification = null;

        [Import]
        VTSCore.Utility.IMouseEventSource _mouseEventSource;

        LocatorAndBorder _locator;

        DistanceMeasurementLines _pointTracks = null;

        MenuBarsBaseInfo _menuBarsInfo;
        StatusBarBaseInfomation _statusBarInfo;

        protected override async void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (WindowUtil.IsDesingMode())
                return;

            await Task.Yield();
            await _mapNotification.InitCompletion;
            _locator = LocatorAndBorder.Instance;
            _locator.OnMapRefreshed += updateLocation;
            _menuBarsInfo = MenuBarsBaseInfo.Instance;
            _menuBarsInfo.PropertyChanged += _menuBarsInfo_PropertyChanged;
            _statusBarInfo = StatusBarBaseInfomation.Instance;
            _statusBarInfo.PropertyChanged += _statusBarInfo_PropertyChanged;
            _mouseEventSource.MouseDown.Subscribe(downMouse);
            _mouseEventSource.MouseDoubleClick.Subscribe(doubleMouse);
        }

        async void _statusBarInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(_pointTracks != null && e.PropertyName == "Position" && _menuBarsInfo.DistanceMeasurement == true)
            {//鼠标经纬度变化
                _mousePosition.Lon = _statusBarInfo.Position.X;
                _mousePosition.Lat = _statusBarInfo.Position.Y;
                await Task.Delay(0);
               _pointTracks.UpdateMousePosition(_mousePosition);
            }
        }

        void _menuBarsInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DistanceMeasurement")
            {
                if (_menuBarsInfo.DistanceMeasurement == true)
                {
                    AddPointsTrack();
                }
                else if (_pointTracks != null)
                {
                    removePointsTrack();
                }
            }
        }

        MapPoint _mousePosition = new MapPoint(-181, 91);
        public async void downMouse(Point point)
        {
            if (_pointTracks != null && _menuBarsInfo.DistanceMeasurement == true)
            {
                await Task.Delay(0);
                var location = _locator.Locator.ScreenToMap(point.X, point.Y);
                if (_pointTracks.IsStop)
                {
                    removePointsTrack();
                    AddPointsTrack();
                }
                _pointTracks.Push(location);
                _mousePosition.Lon = location.Lon;
                _mousePosition.Lat = location.Lat;
            }
        }

        public async void doubleMouse(Point point)
        {
            if (_pointTracks != null && _menuBarsInfo.DistanceMeasurement == true)
            {
                await Task.Delay(0);
                _pointTracks.IsStop = true;
            }
        }

        /// <summary>
        /// 在图像整体变动是发生（窗口改变，平移、缩放）
        /// </summary>
        private void updateLocation()
        {
            if (_pointTracks != null)
                _pointTracks.UpdateMouseAll(_mousePosition);
        }

        private void AddPointsTrack()
        {
            _pointTracks = new DistanceMeasurementLines(_locator.Locator);
            this.Children.Add(_pointTracks);
        }

        private void removePointsTrack()
        {
            if (_pointTracks != null)
                this.Children.Remove(_pointTracks);
            _pointTracks = null;
        }

    }
}
