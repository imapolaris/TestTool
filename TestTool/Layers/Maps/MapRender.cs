using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Services.SeaMap;
using VTSCore.Data.Common;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Threading;
using System.Diagnostics;
using System.Reactive.Subjects;
using System.Configuration;

namespace VTSCore.Layers.Maps
{
	public interface ILocator
	{
		MapPoint Center { get; }
		double Scale { get; }
		Size MapSize { get; }

		double RotateAngle { get; set; }
		double MapRotateAngle { get; set; }

		Point MapToScreen(double lon, double lat);
		MapPoint ScreenToMap(double x, double y);
        
		Task Locate(double scale, MapPoint centerPoint);

		event Action OnMapRefreshed;
	}

	public interface IMapNotification
	{
		event Action OnMapRefreshed;
		Task InitCompletion { get; }
	}

    public class LoadingStatusInfo : INotifyPropertyChanged
    {
        bool _isLoading;
        public bool IsLoading {
            get { return _isLoading; }
            set 
            {
                if (value == _isLoading)
                    return;
                _isLoading = value;
                FirePropertyChanged("IsLoading");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void FirePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

	public class MapRender : NotifyItem, IDisposable, ILocator, IMapNotification, ICoordConverter
	{
		MercatorMapRender _seaMapRender;

		public MapPoint Center { get; private set; }

		private double _scale;
		public double Scale
		{
			get { return _scale; }
			private set { updateProper(ref _scale, value); }
		}

		private Size _mapSize;
		public Size MapSize
		{
			get { return _mapSize; }
			private set { updateProper(ref _mapSize, value); }
		}

		FrameworkElement _seaMap;
		ImageBrush _brush;

		public event Action OnMapRefreshed;
        StatusBarBaseInfomation _statusBarInfo;
        TileLandMapLayer.TileLayerPainterEx2 _tilePainter;
        bool _displayRoadMap = false;
        bool _displaySeaChart = true;

		public MapRender(FrameworkElement seaMap, ImageBrush brush)
		{
            _displayRoadMap = isDisplayRoadMap();
            _displaySeaChart = isDisplaySeaChart();
            _statusBarInfo = StatusBarBaseInfomation.Instance;
			this._seaMap = seaMap;
			this._brush = brush;
			createMapRender();
            if (_displayRoadMap)
            {
                _tilePainter = new TileLandMapLayer.TileLayerPainterEx2(new TileLandMapLayer.TileSourceParam(TileLandMapLayer.TileSourceType.FCGI, ConfigurationManager.AppSettings["RoadMapAddress"]));
                _subject.Throttle(TimeSpan.FromMilliseconds(100)).Subscribe(drawTile);
                //_subject.Subscribe(drawTile);
                _tilePainter.TileLayerImgUpdated += _tilePainter_TileLayerImgUpdated;
            }
            dispatchSizeChanged();
		}
        int count;

        Subject<System.Drawing.Bitmap> _subject = new Subject<System.Drawing.Bitmap>();

        void drawTile(System.Drawing.Bitmap img)
        {            
            _seaMap.Dispatcher.BeginInvoke(new Action(() =>
            {
                using (MemoryStream memory = new MemoryStream())
                {                    
                    (img.Clone() as System.Drawing.Bitmap).Save(memory, ImageFormat.Png);
                    memory.Position = 0;
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    _brush.ImageSource = bitmapImage;
                }
            }), new object[0]);
        }

        private void _tilePainter_TileLayerImgUpdated(System.Drawing.Bitmap img)
        {
            _subject.OnNext(img);
        }

        void createMapRender()
		{
            var defaultData = DefaultConfigInfo.Instance.Default;
            var defaultCenter = new SeeCool.Geometry.Unit.PointD(defaultData.Lon, defaultData.Lat);
			_seaMapRender = new MercatorMapRender(() => new System.Drawing.Size((int)_seaMap.ActualWidth, (int)_seaMap.ActualHeight),
									_displaySetting.ToSeaMapDisplaySetting(),
                                    defaultCenter,
                                    defaultData.Scale);
		}
        public LoadingStatusInfo Loading = new LoadingStatusInfo();
        public void AddMapRender(string[] fileNames)
        {
            Loading.IsLoading = true;
            System.Threading.Thread newThread = new System.Threading.Thread(new System.Threading.ThreadStart(_threadProc));
            newThread.Start();
            _fileNames = fileNames;
            System.Threading.Thread newThread1 = new System.Threading.Thread(new System.Threading.ThreadStart(_threadProc1));
            newThread1.Start();
            
        }
        string[] _fileNames;
        private void _threadProc1()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new DeleFunc1(Func1));
        }

        private async void Func1()
        {
            await Task.Yield();
            MercatorMapRender.AddMapRender(_fileNames);
            RefreshMap().IgnorCompletion();
            Loading.IsLoading = false;
        }
        public delegate void DeleFunc();
        public delegate void DeleFunc1();
        MapLoadingClient _loadingClient;
        public async void Func()
        {
            await Task.Yield();
            if (_loadingClient != null)
                _loadingClient.Close();
            _loadingClient = new MapLoadingClient(Loading);
            _loadingClient.ShowDialog();
            _loadingClient = null;
        }
        
        private void _threadProc()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new DeleFunc(Func));
        }
        
        public void AddMapRender()
        {
            OpenFileDialog fileDlg = new OpenFileDialog();
            fileDlg.RestoreDirectory = true;
            fileDlg.Multiselect = true;
            fileDlg.Filter = "S57 files (*.0??)|*.0??";
            fileDlg.Title = "添加海图文件";
            if (fileDlg.ShowDialog() == DialogResult.OK)
            {
                AddMapRender(fileDlg.FileNames);
            }
        }

		DisplaySetting _displaySetting = new DisplaySetting();
		public DisplaySetting DisplaySetting
		{
			get { return _displaySetting; }
			set
			{
				_displaySetting = value;
				createMapRender();
				RefreshMap().IgnorCompletion();
			}
		}

		public Task Locate(double scale, MapPoint centerPoint)
		{
			this.Scale = scale;
			this.Center = centerPoint;
			return RefreshMap();
		}

		public MapPoint ScreenToMap(double x, double y)
		{
			var p = _seaMapRender.ScreenToMap(x, y);
			return new MapPoint(p.X, p.Y);
		}

		public Point MapToScreen(double lon, double lat)
		{
			var p = _seaMapRender.MapToScreen(lon, lat);
			return new Point(p.X, p.Y);
		}

        System.Drawing.Bitmap _bmp;

        public async Task RefreshMap()
		{
            if (Center == null)
                return;
			var img = _brush;
            Scale = MapBorderInfo.CorrectionScale(Scale);
            Center = MapBorderInfo.CorrectionCenter(Center);
            
			_seaMapRender.Scale = this.Scale;
            _statusBarInfo.Scale = string.Format("比例尺 1 ：{0}", (int)this.Scale);
			_seaMapRender.Center = new SeeCool.Geometry.Unit.PointD(Center.Lon, Center.Lat);


            using (var memory = await Task.Run(() => _seaMapRender.LoadMap()))
            {
                if (_displaySeaChart)
                    updateBitmapImage(img, memory);
            }


            if (_displayRoadMap)
            {
                img.ImageSource = null;
                _bmp = new System.Drawing.Bitmap(_seaMapRender.ClientRectangle.Width, _seaMapRender.ClientRectangle.Height);

                var extent = _seaMapRender._extent;
                _tilePainter.Paint(extent.Left, extent.Top, extent.Right, extent.Bottom, _seaMapRender.Scale, _bmp);
            }


            updateMapSize();

			_initCompletion.TrySetResult(true);

			if (OnMapRefreshed != null)
				OnMapRefreshed();
		}

        void updateBitmapImage(ImageBrush img, MemoryStream memory)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();
            img.ImageSource = bitmapImage;
        }

        bool isDisplaySeaChart()
        {
            try
            {
                return bool.Parse(ConfigurationManager.AppSettings["DisplaySeaChart"]);
            }
            catch
            {
                return false;
            }
        }

        bool isDisplayRoadMap()
        {
            try
            {
                return bool.Parse(ConfigurationManager.AppSettings["DisplayRoadMap"]);
            }
            catch
            {
                return false;
            }
        }

        private void updateMapSize()
		{
			var wp1 = _seaMapRender.ScreenToMap(0, _seaMap.ActualHeight / 2);
			var wp2 = _seaMapRender.ScreenToMap(_seaMap.ActualWidth, _seaMap.ActualHeight / 2);

			var hp1 = _seaMapRender.ScreenToMap(_seaMap.ActualHeight / 2, 0);
			var hp2 = _seaMapRender.ScreenToMap(_seaMap.ActualHeight / 2, _seaMap.ActualHeight);

			//单位是海里
			var width = SeeCool.Geometry.Util.Calculate.CalcDis(wp1, wp2);
			var height = SeeCool.Geometry.Util.Calculate.CalcDis(hp1, hp2);

			this.MapSize = new Size(width, height);
		}

		#region 旋转
		//地图旋转的实际角度，由于有联动视角的功能，可能和设置角度不同
		public double RotateAngle
		{
			get { return _seaMapRender.RotateAngle; }
			set
			{
				if (_seaMapRender.RotateAngle == value)
					return;

				_seaMapRender.RotateAngle = value;
				RefreshMap().IgnorCompletion();
			}
		}

		//地图旋转的设置角度
		double _mapRotateAngle = 0;
		public double MapRotateAngle
		{
			get { return _mapRotateAngle; }
			set
			{
				_mapRotateAngle = value;

				//TODO
				//if (shipLayer.EnableTrackAngle)
				//	throw new InvalidOperationException("当前使能了视角联动，无法设置地图角度");

				RotateAngle = value;
			}
		}
		#endregion

		#region 窗口大小改变重绘海图
		void dispatchSizeChanged()
		{
			Observable.FromEventPattern<SizeChangedEventArgs>(_seaMap, "SizeChanged")
					  .Throttle(TimeSpan.FromSeconds(0.1))
					  .ObserveOn(_seaMap)
					  .Subscribe(_ => RefreshMap().IgnorCompletion());
		}
		#endregion
        
		TaskCompletionSource<bool> _initCompletion = new TaskCompletionSource<bool>();
		public Task InitCompletion { get { return _initCompletion.Task; } }


		public void Dispose()
		{
			_seaMapRender.Dispose();
		}

		~MapRender()
		{
			Dispose();
		}
	}
}
