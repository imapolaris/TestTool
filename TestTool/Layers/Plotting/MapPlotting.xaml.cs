using Seecool.Radar.Unit;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VTSCore.Data.Common;
using VTSCore.Layers.Maps;

namespace VTSCore.Layers.Plotting
{
    /// <summary>
    /// MapPlotting.xaml 的交互逻辑
    /// </summary>
    public partial class MapPlotting : Canvas
    {
        public MapPlotting()
        {
            InitializeComponent();
            this.ClipToBounds = true;
        }
        [Import]
        Maps.IMapNotification _mapNotification = null;

        [Import]
        VTSCore.Utility.IMouseEventSource _mouseEventSource;

        [Import]
        System.Windows.Controls.ContextMenu _contextMenu;

        LocatorAndBorder _locatorBorder;

        PlottingAreaSettingInfomation _plottingInfomation;
        PlottingArea _plottingArea = null;

        MenuBarsBaseInfo _menuBarsInfo;
        StatusBarBaseInfomation _statusBarInfo;
        ActivatingStatus activatingStatus;
        protected override async void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (WindowUtil.IsDesingMode())
                return;

            await Task.Yield();
            await _mapNotification.InitCompletion;

            _locatorBorder = LocatorAndBorder.Instance;
            _locatorBorder.OnMapRefreshed += OnRefreshed;

            activatingStatus = ActivatingStatus.Instance;
            _menuBarsInfo = MenuBarsBaseInfo.Instance;
            _menuBarsInfo.PropertyChanged += _menuBarsInfo_PropertyChanged;
            _statusBarInfo = StatusBarBaseInfomation.Instance;
            _statusBarInfo.PropertyChanged += _statusBarInfo_PropertyChanged;

            _mouseEventSource.MouseDown.Subscribe(downMouse);
            _mouseEventSource.MouseDoubleClick.Subscribe(doubleMouse);
            _mouseEventSource.MouseRightDown.Subscribe(rightMouse);
            _plottingInfomation = PlottingAreaSettingInfomation.Instance;
            _plottingInfomation.Locator = _locatorBorder.Locator;
            _plottingInfomation.PropertyChanged+=_listPlottingArea_PropertyChanged;
            this.Children.Add(_plottingInfomation.Data);
            InputManager.Current.PreProcessInput += Current_PreProcessInput;//获取键盘输入事件
        }

        void Current_PreProcessInput(object sender, PreProcessInputEventArgs e)
        {
            InputEventArgs inputEventArgs = e.StagingItem.Input;
            if (inputEventArgs is KeyboardEventArgs)
            {
                KeyEventArgs keyEventArgs = inputEventArgs as KeyEventArgs;
                if (keyEventArgs != null && keyEventArgs.IsDown)
                {
                    if (keyEventArgs.Key == Key.Delete)
                    {
                        if (_plottingArea != null && _plottingArea.Visibility == System.Windows.Visibility.Visible && _plottingArea.SelectedIndex >= 0)
                            _plottingArea.RemoveSelectedPoint();
                    }
                    return;
                }
            }
        }

        private void _listPlottingArea_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "重置标绘区选择项")
                _plottingArea = _plottingInfomation.PlotPres;
        }

        private void revisePlottingPoint(Point point, PointD pointD)
        {
            _plottingArea.SelectedIndex = _plottingArea.IndexOnPoints(point);
            if (_plottingArea.SelectedIndex >= 0)
                PlotPointDragDrop();
        }
        
        private void PlotPointDragDrop()
        {
            TraggingStatus status = activatingStatus.TragStatus;
            activatingStatus.TragStatus = TraggingStatus.标绘模式;
            Mouse.Capture(this);
            var dragDrop = _mouseEventSource.MouseDragDrop;
            dragDrop.Subscribe(
                p =>
                {
                    if (IsOperableRadarArea())
                    {
                        var position = _locatorBorder.Locator.ScreenToMap(p.X, p.Y);
                        _plottingArea.RevisePointData(new PointD() { X = position.Lon, Y = position.Lat }, _plottingArea.SelectedIndex);
                    }
                },
                    () =>
                    {
                        Mouse.Capture(null);
                        activatingStatus.TragStatus = status;
                    }
                );
        }

        private async void _statusBarInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Position" && IsOperableRadarArea() && _plottingArea.PlotStatus == PlottingStatus.新建模式)
            {//鼠标经纬度变化
                await Task.Delay(0);
                _plottingArea.UpdateMousePosition(_statusBarInfo.Position);
            }
        }

        private void _menuBarsInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "PlottingAreaParameterSetting")
                showRadarAreaClient();
            else if(e.PropertyName =="FeatureSelectShape")
                updateFeatureSelectedUI();
        }

        RadarAreaClient _radarAreaClient;
        private void showRadarAreaClient()
        {
            if (!VTSCore.Common.WindowStateDetect.ShowWindow(_radarAreaClient))
            {
                _radarAreaClient = new RadarAreaClient();
                System.Windows.Interop.HwndSource winformWindow = (System.Windows.Interop.HwndSource.FromDependencyObject(this) as System.Windows.Interop.HwndSource);
                if (winformWindow != null)
                    new System.Windows.Interop.WindowInteropHelper(_radarAreaClient) { Owner = winformWindow.Handle };
                _radarAreaClient.Show();
            }
            else
                _radarAreaClient.WindowState = WindowState.Normal;
        }

        #region 海图分析

        FeatureSelectUI _featureSelectUI;
        private void updateFeatureSelectedUI()
        {
            if (!_menuBarsInfo.FeatureSelectShape)
                _featureSelectUI = null;
            else
            {
                newFeatureSelectUIIfNull();
                if (_featureSelectUI != null)
                {
                    Point pt = _locatorBorder.Locator.MapToScreen(_statusBarInfo.Position.X, _statusBarInfo.Position.Y);
                    _featureSelectUI.ShapeCommitted(pt);
                }
            }
        }

        private void newFeatureSelectUIIfNull()
        {
            if (_featureSelectUI == null)
            {
                _featureSelectUI = new FeatureSelectUI();
                _featureSelectUI.WinformWindow = (System.Windows.Interop.HwndSource.FromDependencyObject(this) as System.Windows.Interop.HwndSource);
                _featureSelectUI.OnRestFeatureSelect += onRestFeatureSelect;
            }
        }

        PlottingArea _seaCheatAnalyzeArea;
        private void onRestFeatureSelect(string shape)
        {
            if (_seaCheatAnalyzeArea != null)
                this.Children.Remove(_seaCheatAnalyzeArea);
            _seaCheatAnalyzeArea = null;

            if (!string.IsNullOrWhiteSpace(shape))//无效字符，表示无区域
            {
                var region = RadarRegionFromString.GetRegion(shape);
                _seaCheatAnalyzeArea = new PlottingArea(_locatorBorder.Locator, region);
                _seaCheatAnalyzeArea.PolygonOpacity = 0.5;
                _seaCheatAnalyzeArea.PlotStatus = PlottingStatus.锁定模式;
                _seaCheatAnalyzeArea.MoveToCentered();
                this.Children.Add(_seaCheatAnalyzeArea);
            }
        }

        #endregion 海图分析

        #region 鼠标事件处理

        private async void downMouse(Point point)
        {
            this.Focus();
            if (_plottingArea != null)
            {
                await Task.Delay(0);
                var position = _locatorBorder.Locator.ScreenToMap(point.X, point.Y);
                PointD pointD = new PointD(position.Lon, position.Lat);
                switch (_plottingArea.PlotStatus)
                {
                    case PlottingStatus.新建模式:
                        _plottingArea.Push(pointD);
                        break;
                    case PlottingStatus.编辑模式:
                        if (IsOperableRadarArea())
                            revisePlottingPoint(point, pointD);
                        break;
                    default:
                        break;
                }
            }
        }

        public void doubleMouse(Point point)
        {
            this.Focus();
            if (_plottingArea != null)
            {
                switch (_plottingArea.PlotStatus)
                {
                    case PlottingStatus.新建模式:
                        _plottingInfomation.PlotStatus = PlottingStatus.编辑模式;
                        break;
                    case PlottingStatus.编辑模式:
                        if (IsOperableRadarArea())
                        {
                            var position = _locatorBorder.Locator.ScreenToMap(point.X, point.Y);
                            PointD pointD = new PointD(position.Lon, position.Lat);
                            int index = _plottingArea.IndexOnLines(point);
                            if (index >= 0)
                                _plottingArea.Push(pointD, index);
                        }
                        break;
                }
            }
        }

        private void rightMouse(Point pt)
        {
            var names = _plottingInfomation.GetRadarAreasName(pt);
            foreach(var name in names)
            {
                MenuItem item = new MenuItem { Header = name };
                item.Click += editingAreaFromName_Click;
                _contextMenu.Items.Add(item);
            }
            if (names.Count > 0)
                _contextMenu.Items.Add(new Separator());
            MenuItem item2 = new MenuItem { Header = "海图分析" };
            item2.Click +=seaCheatAnalyze_Click;
            _contextMenu.Items.Add(item2);
        }

        void editingAreaFromName_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;
            int index = -1;
            if(item != null)
            {
                index = _plottingInfomation.GetIndexFromName(item.Header.ToString());
            }
            showRadarAreaClient();
            if(_radarAreaClient != null && index >= 0)
            {
                _radarAreaClient.SelectedIndex(index);
            }
        }

        private void seaCheatAnalyze_Click(object sender, RoutedEventArgs e)
        {
            _menuBarsInfo.FeatureSelectShape = true;
        }

        #endregion 鼠标事件处理

        private void OnRefreshed()
        {
            _plottingInfomation.Data.OnRefreshed(_statusBarInfo.Position);
            if (_seaCheatAnalyzeArea != null)
                _seaCheatAnalyzeArea.OnRefreshed();
        }

        bool IsOperableRadarArea()
        {
            return _plottingArea != null && _plottingArea.Visibility == System.Windows.Visibility.Visible;
        }

        public void SaveIfChanged()
        {
            if (_plottingInfomation != null)
                _plottingInfomation.SetRegionsIfChanged();
        }
    }
}