using RadarImageProjection;
using RadarServiceNetCmds;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VTSCore.Layers.Maps;

namespace VTSCore.Layers.Radar
{
    public class RadarImage : Canvas, RadarImageProjection.ICoordConverter
    {
        byte _trailStage = 10;
        byte[] _radarFrame = new byte[0];
        byte[] _radarLock;
        byte[] _radarShow;
        bool _isLocked = false;

        CCTVConnection _connection;
        FastProjection _projection = new FastProjection();
        RotateFilter _rotateFilter = null;
        Image _image = new Image();
        double _anglePrev = 0;
        Image _icon;

        LocatorAndBorder _locator;
        RadarColorTableDataInfo _radarColorTableData;
        public RadarImage(LocatorAndBorder locator)
        {
            _locator = locator;
            _radarColorTableData = RadarColorTableDataInfo.Instance;
            _colorTableData = _radarColorTableData.GetColorTableData(0);
            _radarColorTableData.PropertyChanged += _radarColorTableData_PropertyChanged;
            this.Children.Add(_image);
            _image.Visibility = System.Windows.Visibility.Visible;
            _radarShow = _radarFrame;
            _icon = new Image() { Source = new BitmapImage(new Uri(@"../../../Resources/雷达基站.png", UriKind.Relative)) };
            this.Children.Add(_icon);
            loadRadarMap();
            ColorTableIndex = 0;
            _locator.OnMapRefreshed += iconRefreshed;
            iconRefreshed();
        }

        private void iconRefreshed()
        {
            if (Visibility == System.Windows.Visibility.Visible)
            {
                var pt = _locator.Locator.MapToScreen(RadarInfo.Longitude, RadarInfo.Latitiude);
                double length = 40 * ZoomFactor();
                _icon.Width = length;
                _icon.Height = length;
                pt.X -= length * 0.5;
                pt.Y -= length * 0.7;
                Canvas.SetLeft(_icon, pt.X);
                Canvas.SetTop(_icon, pt.Y);
            }
        }

        private float ZoomFactor()
        {
            double scale = _locator.Locator.Scale;
            if (scale > 900000)
                return 0.5f;
            else if (scale <= 20000)
                return 1f;
            return (float)(Math.Sin(Math.PI * (40000 / scale + 1) / 6));
        }

        void _radarColorTableData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ResetColorTableDataConfig" || e.PropertyName == "AddColorTable" || e.PropertyName == "RemoveColorTable" || e.PropertyName == "EditorColorTable")
            {
                ColorTableData = _radarColorTableData.GetColorTableData(ColorTableIndex);
            }
        }

        void loadRadarMap()
        {
            prepareColorTable();
            _image.Source = new BitmapImage();
        }

        public bool IsPlaying { get { return _connection != null; } }

        public void Start(string ip, int port)
        {
            Stop();
            _connection = new CCTVConnection(ip, port);
            _connection.MessageEvent += onMessage;
            _connection.Start();
        }

        public void Stop()
        {
            Clear();
            if (_connection != null)
            {
                _connection.MessageEvent -= onMessage;
                _connection.Stop();
            }
            _connection = null;
        }

        #region 雷达显示颜色设置

        private Color[] _colorTable = new Color[256];
        ColorTableData _colorTableData = new ColorTableData();


        int _colorTableIndex = -1;

        public int ColorTableIndex
        {
            get { return _colorTableIndex; }
            set
            {
                if (_colorTableIndex != value)
                {
                    _colorTableIndex = value;
                    ColorTableData = RadarColorTableDataInfo.Instance.GetColorTableData(_colorTableIndex);
                }
            }
        }

        ColorTableData ColorTableData
        {
            get { return _colorTableData; }
            set {
                if (!_colorTableData.IsSameWith(value))
                {
                    _colorTableData = value;
                    prepareColorTable();
                }
            }
        }

        private void prepareColorTable()
        {
            if ((int)_trailStage != ColorTableData.TrailState)
            {
                _trailStage = (byte)ColorTableData.TrailState;
                Array.Clear(_radarFrame, 0, _radarFrame.Length);
            }

            ColorLinear frontCl = new ColorLinear(ColorTableData.FrontStart, ColorTableData.FrontEnd);
            ColorLinear trailCl = new ColorLinear(ColorTableData.TrailStart, ColorTableData.TrailEnd);

            _colorTable[0] = Colors.Transparent;
            for (int i = 0; i < _trailStage; i++)
                _colorTable[i + 1] = trailCl.GetColor(i * 1.0 / (_trailStage - 1));
            for (int i = 0; i < 128; i++)
                _colorTable[i + 128] = frontCl.GetColor(i * 1.0 / 127);
        }

        #endregion

        public int ScreenWidth;
        public int ScreenHeight;
        
        private void onRadarDataRefreshed(byte[] data)
        {
            if (data == null)
                return;
            if (_radarFrame.Length != data.Length)
            {
                _radarFrame = new byte[data.Length];
                _radarShow = _radarFrame;
            }

            for (int i = 0; i < _radarFrame.Length; i++)
            {
                if (data[i] != 0)
                    _radarFrame[i] = (byte)(data[i] / 2 + 128);
                else if (_radarFrame[i] >= 128)
                    _radarFrame[i] = _trailStage;
                else if (_radarFrame[i] > 0)
                    _radarFrame[i]--;
            }
            if (!_isLocked)
                PaintRadarMap();
        }

        public void PaintRadarMap()
        {
            if (this.Visibility != System.Windows.Visibility.Visible)
                return;
            byte[] radarFrame = _radarShow;
            //for (int i = 0; i < radarFrame.Length; i++)
            //{
            //    if (radarFrame[i] < 200)
            //        radarFrame[i] = 0;
            //}

            if (_locator != null && radarFrame.Length > 0)
            {
                ScreenParams sp = new ScreenParams();
                sp.Width = ScreenWidth;
                sp.Height = ScreenHeight;
                var locator = _locator.Locator;
                sp.CenterCoord = new SeeCool.Geometry.Unit.PointD(locator.Center.Lon, locator.Center.Lat);
                sp.Scale = locator.Scale;
                byte[] data = _projection.ProjectToScreen(RadarInfo, radarFrame, sp, this);
                byte[] bytes = new byte[sp.Width * 4 * sp.Height];
                for (int i = 0, j = 0; i < data.Length; i++)
                {
                    Color c = _colorTable[data[i]];
                    bytes[j++] = c.B;
                    bytes[j++] = c.G;
                    bytes[j++] = c.R;
                    bytes[j++] = c.A;
                }

                this.Dispatcher.BeginInvoke(new Action<int, int, byte[]>(onLayerImage), sp.Width, sp.Height, bytes);
            }
        }

        public RadarStatus RadarStatus { get; set; }
        public RadarInfo RadarInfo { get; set; }
        private void onMessage(byte[] message)
        {
            lock (_radarFrame)
            {
                try
                {
                    MessageReader reader = new MessageReader(message);
                    if (reader.MessageID == 0x10000)
                    {
                        //ScanLineCount = reader.Reader.ReadInt32();
                        //SampleCount = reader.Reader.ReadInt32();
                    }
                    else if (reader.MessageID == 1 || reader.MessageID == 2)
                    {
                        if (RadarStatus == null)
                            return;
                        int size = reader.Reader.ReadInt32();
                        byte[] compressed = reader.Reader.ReadBytes(size);
                        int timeStamp = 0;
                        if (reader.BytesLeft >= 4)
                            timeStamp = reader.Reader.ReadInt32();

                        int uncompressedSize = RadarStatus.ScanLineCount * RadarStatus.SampleCount;
                        byte[] data = null;
                        if (reader.MessageID == 1)
                            data = ZLibUtil.Uncompress(compressed, uncompressedSize);
                        else
                            data = Lz4Net.Lz4.DecompressBytes(compressed);
                            
                        if (_rotateFilter != null)
                            _rotateFilter.Filter(RadarInfo, data);

                        onRadarDataRefreshed(data);
                    }
                }
                catch { }
            }
        }

        #region CoordConverter
        private void onLayerImage(int width, int height, byte[] imageData)
        {
            if ((width == 0) || (height == 0))
                return;
            var bmp = _image.Source as WriteableBitmap;
            if ((bmp == null) || (bmp.PixelWidth != width) || (bmp.PixelHeight != height))
                bmp = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, BitmapPalettes.Halftone256Transparent);
            Int32Rect rect = new Int32Rect(0, 0, width, height);
            bmp.WritePixels(rect, imageData, width * 4, 0);
            _image.Source = bmp;
        }

        SeeCool.Geometry.Unit.PointD RadarImageProjection.ICoordConverter.MapToScreen(double lon, double lat)
        {
            var pt = _locator.Locator.MapToScreen(lon, lat);
            return new SeeCool.Geometry.Unit.PointD(pt.X, pt.Y);
        }

        SeeCool.Geometry.Unit.PointD RadarImageProjection.ICoordConverter.ScreenToMap(double x, double y)
        {
            var mp = _locator.Locator.ScreenToMap(x, y);
            return new SeeCool.Geometry.Unit.PointD(mp.Lon, mp.Lat);
        }
        #endregion

        public double RadarBaseAngle { get; set; }

        public void UpdateRadarData(RadarSettingInfo radar)
        {
            RadarStatus = radar.RadarStatus;
            RadarInfo = radar.RadarInfo;
            RadarBaseAngle = radar.RadarStatusBase.OffsetAngle;
            updateRotateFilter();
            UpdateRadarDraw();
            _icon.ToolTip = RadarStatus.Name;
        }

        public void UpdateRadarDraw()
        {
            if (IsPlaying)
            {
                updateShowRadarStatus();
                PaintRadarMap();
            }
            iconRefreshed();
        }

        private void updateShowRadarStatus()
        {
            if (RadarStatus != null && _locator.InScreenAtCircle(RadarStatus.Longitude, RadarStatus.Latitude, RadarStatus.Range / 60))
                Visibility = System.Windows.Visibility.Visible;
            else
                Visibility = System.Windows.Visibility.Collapsed;
        }

        void updateRotateFilter()
        {
            double offsetAngle = RadarBaseAngle - RadarStatus.OffsetAngle;
            if (_anglePrev != offsetAngle)
            {
                if (_radarFrame.Length > 0)
                    new RotateFilter(offsetAngle - _anglePrev).Filter(RadarInfo, _radarFrame);
                _anglePrev = offsetAngle;
            }
            if (offsetAngle != 0)
                _rotateFilter = new RotateFilter(offsetAngle);
            else
                _rotateFilter = null;
        }

        public bool LockAll
        {
            set
            {
                if (value != _isLocked)
                {
                    if (value)
                    {
                        lock (_radarFrame)
                        {
                            _radarLock = new byte[_radarFrame.Length];
                            Array.Copy(_radarFrame, _radarLock, _radarFrame.Length);
                            _radarShow = _radarLock;
                        }
                    }
                    else
                    {
                        lock (_radarLock)
                        {
                            _radarShow = _radarFrame;
                            _radarLock = null;
                        }
                    }
                    _isLocked = value;
                }
            }
        }

        public void Clear()
        {
            if (_radarFrame != null)
            {
                lock (_radarFrame)
                    _radarFrame = new byte[_radarFrame.Length];
            }
        }

        public void Transform(double x, double y)
        {
            this.VisualOffset = new Vector(x, y);
        }
    }
}
