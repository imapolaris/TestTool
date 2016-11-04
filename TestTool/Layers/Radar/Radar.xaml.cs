using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Reactive.Linq;
using VTSCore.Data.Common;
using VTSCore.Layers.Maps;

namespace VTSCore.Layers.Radar
{
	/// <summary>
	/// Config.xaml 的交互逻辑
	/// </summary>
	public partial class Radar : UserControl, IDisposable
	{
		public Radar()
		{
			InitializeComponent();
		}

        RadarsCanvas _radars;
        MenuBarsBaseInfo _menuBarInfo;
        ActivatingStatus activatingStatus;
        LocatorAndBorder _locator;
        System.Windows.Interop.HwndSource _winformWindow;
        [Import]
        VTSCore.Utility.IMouseEventSource _mouseEventSource;

		[Import]
		Maps.IMapNotification _mapNotification = null;

        protected override async void OnInitialized(EventArgs e)
		{
            base.OnInitialized(e);

			if (WindowUtil.IsDesingMode())
				return;
            await System.Threading.Tasks.Task.Yield();
			await _mapNotification.InitCompletion;
            _locator = LocatorAndBorder.Instance;
            _radars = new RadarsCanvas(_locator);
            this.AddChild(_radars);
            _locator.OnMapRefreshed += _locator_OnMapRefreshed;
            _mouseEventSource.MouseDown.Subscribe(onMouseDown);

            activatingStatus = ActivatingStatus.Instance;
            _menuBarInfo = MenuBarsBaseInfo.Instance;
            _menuBarInfo.PropertyChanged += _menuBarInfo_PropertyChanged;
            _winformWindow = (System.Windows.Interop.HwndSource.FromDependencyObject(this) as System.Windows.Interop.HwndSource);
            _radars.InitHwndSourse(_winformWindow);
            _locator_OnMapRefreshed();
		}

        void _locator_OnMapRefreshed()
        {
            if (_radars != null)
                _radars.OnScreenChanged((int)this.ActualWidth, (int)this.ActualHeight);
        }
        void _menuBarInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_radars == null)
                return;
            if (e.PropertyName == "RadarParameterSetting")
            {
                _radars.RadarEditorWindow();
            }
            else if (e.PropertyName == "ClearCache")
            {
                _radars.ClearCache();
            }
            else if (e.PropertyName == "LockAll")
            {
                _radars.LockAll = _menuBarInfo.LockAll;
            }
            else if(e.PropertyName == "RadarColorSchemes")
            {
                openColorSchemesClient();
            }
        }

        RadarColorClient _colorSchemes;

        private void openColorSchemesClient()
        {
            if (!VTSCore.Common.WindowStateDetect.ShowWindow(_colorSchemes))
            {
                _colorSchemes = new RadarColorClient();
                if (_winformWindow != null)
                    new System.Windows.Interop.WindowInteropHelper(_colorSchemes) { Owner = _winformWindow.Handle };
                _colorSchemes.Show();
            }
        }

        void onMouseDown(Point point)
        {
            if (Unavailable())
                return;
            var mousePosition = _locator.Locator.ScreenToMap(point.X, point.Y);
            bool inRadarCoverageArea = _radars.InRadarCoverageArea(mousePosition);
            if (inRadarCoverageArea == false)
                return;
            var dragDrop = _mouseEventSource.MouseDragDrop;
            Point transFrom = new Point();
            dragDrop = dragDrop.Skip(1).Zip(dragDrop, (i1, i2) => new Point(i1.X - i2.X, i1.Y - i2.Y));
            dragDrop.Subscribe(
                p =>
                {
                    if (activatingStatus.TragStatus == TraggingStatus.海图模式)
                    {
                        if (activatingStatus.ChartStatus == ChartsStatus.移动雷达)
                        {
                            transFrom.X += p.X;
                            transFrom.Y += p.Y;
                            _radars.Transform(transFrom.X, transFrom.Y);
                        }
                        else if (activatingStatus.ChartStatus == ChartsStatus.拖拽雷达)
                        {
                            point.X += p.X;
                            point.Y += p.Y;
                            var endPostion = _locator.Locator.ScreenToMap(point.X, point.Y);
                            _radars.Drag(mousePosition, endPostion);
                            mousePosition = endPostion;
                        }
                    }
                },

                () =>
                {
                    Mouse.Capture(null);
                    if (transFrom.X == 0 && transFrom.Y == 0)
                        return;
                    _radars.Transform(0, 0);
                    if (activatingStatus.ChartStatus == ChartsStatus.移动雷达)
                        _radars.Offset(transFrom.X, transFrom.Y);
                });
        }

        private bool Unavailable()
        {
            return activatingStatus.ChartStatus != ChartsStatus.移动雷达 && activatingStatus.ChartStatus != ChartsStatus.拖拽雷达;
        }


        public void SaveIfChanged()
        {
            if (_radars != null)
                _radars.SetConfigIfChanged();
        }
        public void Dispose()
        {
            if (_radars != null)
                _radars.Dispose();
            _radars = null;
        }
    }
}