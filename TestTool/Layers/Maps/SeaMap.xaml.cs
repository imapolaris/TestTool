using Services.SeaMap;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VTSCore.Data.Common;


namespace VTSCore.Layers.Maps
{
	[DesignTimeVisible(false)]
    public partial class SeaMap : UserControl, IDisposable
	{
		public SeaMap()
		{
			InitializeComponent();
		}

		[Export]
		Parts.SeaMapInfo _seaMapInfo;

		[Export(typeof(ILocator))]
		[Export(typeof(IMapNotification))]
		MapRender _render;

        StatusBarBaseInfomation _statusBar;

		public ILocator Locator { get { return _render; } }

		[ImportMany]
		Parts.IPart[] _mapParts = null;
        LocatorAndBorder _locatorAndBorder;

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			if (WindowUtil.IsDesingMode())
				return;
            _statusBar = StatusBarBaseInfomation.Instance;

			this.MouseDown += (_s, _e) => this.Focus();
			_render = new MapRender(this, this.Resources["seaMapBrush"] as ImageBrush);
			_seaMapInfo = new Parts.SeaMapInfo(this);
            MenuBarsBaseInfo.Instance.PropertyChanged+= menuBar_PropertyChanged;
		}

        private void menuBar_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "StartUpPosition")
            {
                if (DefaultConfigInfo.Instance.SaveDefault(Locator.Center.Lon, Locator.Center.Lat, Locator.Scale))
                    MessageBox.Show("成功配置当前位置为启动位置！");
                else
                    MessageBox.Show("配置当前位置为启动位置失败！", "错误");
            }
        }
        
        public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (WindowUtil.IsDesingMode())
				return;

			foreach (var part in _mapParts)
			{
				part.Init();
			}
            _mouseEventSource.MouseDown.Subscribe(downMouse);
            _mouseEventSource.MouseMove.Subscribe(moveMouse);
            _mouseEventSource.MouseUp.Subscribe(upMouse);

		}

        private async void upMouse(Point point)
        {
            await Task.Delay(20);
            _mouseDown = false;
        }
        bool _mouseDown;
        private void downMouse(Point point)
        {
            _mouseDown = true;
        }

		#region ISeaMap

		/// <summary>
		/// 鼠标位置
		/// </summary>
		public Point MouseLocation
		{
			get { return (Point)GetValue(MouseLocationProperty); }
			set { SetValue(MouseLocationProperty, value); }
		}

		public static readonly DependencyProperty MouseLocationProperty =
			DependencyProperty.Register("MouseLocation", typeof(Point), typeof(SeaMap));

		#endregion

		void SeaMap_Loaded(object sender, RoutedEventArgs e)
		{
			if (WindowUtil.IsDesingMode())
				return;

			if (this.RenderSize.Width == 0 || this.RenderSize.Height == 0)
				return;

			this.Focus();
			if (_render.InitCompletion.IsCompleted)
				return;
            var defaultData = DefaultConfigInfo.Instance.Default;
            Locator.Locate(defaultData.Scale, new MapPoint(defaultData.Lon, defaultData.Lat));

            _locatorAndBorder = LocatorAndBorder.Instance;
            _locatorAndBorder.InitLocator(Locator);
            Locator.OnMapRefreshed += Locator_OnMapRefreshed;
		}

        void Locator_OnMapRefreshed()
        {
            _locatorAndBorder.UpdateLocator();
        }

		public DisplaySetting DisplaySetting
		{
			get { return _render.DisplaySetting; }
			set { _render.DisplaySetting = value; }
		}

		#region 更新鼠标位置

		[Import]
        Utility.IMouseEventSource _mouseEventSource = null;

        public void moveMouse(Point point)
        {
            if (_mouseDown)
                return;
            var loc = Locator.ScreenToMap(point.X, point.Y);
            this.MouseLocation = new Point(loc.Lon, loc.Lat);
            _statusBar.Position = new Seecool.Radar.Unit.PointD(loc.Lon, loc.Lat); 
        }
        #endregion

		public void Dispose()
		{
			_render.Dispose();
		}

        private void AddSeaChart_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _render.AddMapRender();
        }
    }
}
