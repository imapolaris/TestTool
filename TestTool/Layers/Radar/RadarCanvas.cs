using Common.Logging;
using SeeCool.GISFramework.Util;
using System;
using System.Windows.Controls;
using VTSCore.Data.Common;
using VTSCore.Layers.Maps;

namespace VTSCore.Layers.Radar
{
    class RadarCanvas : Canvas, IDisposable
    {
        LocatorAndBorder _locator;
        RadarSettingInfo _radar;
        RadarImage _radarImage;
        RadarSettingControl _radarControl;
        PeriodTimer _timer;
        ILog LogService { get { return LogManager.GetLogger(GetType()); } }
        public bool LockAll
        { 
            set 
            { 
                if(_radarImage != null)
                    _radarImage.LockAll = value; 
            }
        }

        public RadarCanvas(LocatorAndBorder locator, RadarSettingInfo radar)
        {
            _locator = locator;
            _radar = radar;
            _radarControl = new RadarSettingControl();
            loadRadarInfo();
            _radar.PropertyChanged += _radar_PropertyChanged;
        }

        private void loadRadarInfo()
        {
            if (_radar.IsEnable)
            {
                loadRadarImage();
                startRpc();
                restartRadarImage();
            }
            else
            {
                stopTimer();
                _radarControl.Stop();
                if (_radarImage != null)
                {
                    this.Children.Remove(_radarImage);
                    _radarImage.Stop();
                }
                _radarImage = null;
            }
        }

        ~RadarCanvas()
        {
            Dispose();
        }
        
        void _radar_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "RadarIpPort")
                restartRadarImage();
            else if (e.PropertyName == "EndPoint")
                startRpc();
            else if (e.PropertyName == nameof(_radar.ColorTableIndex))
            {
                if (_radarImage != null)
                    _radarImage.ColorTableIndex = _radar.RadarAddress.ColorTableIndex;
            }
            else if (e.PropertyName == nameof(_radar.SaveRadarConfig))
            {
                _radarControl.SetRadarConfig(_radar.RadarStatus);
                updateRadarImageFromConfig();
            }
            else if (e.PropertyName == nameof(_radar.SaveRadarRegions))
                _radarControl.SetRadarRegions(_radar.RadarRegions);
            else if (e.PropertyName == "SaveRadarChannals")
                _radarControl.SetRadarChannels(_radar.RadarChannels);
            else if (e.PropertyName == "ResetRadarRegions")
                _radarControl.ResetRadarRegions(_radar);
            else if (e.PropertyName == "ResetRadarChannals")
                _radarControl.ResetRadarChannels(_radar);
            else if (e.PropertyName == "GotoPositioning")
            {
                var position = _radar.Position;
                if ((position.Lon != 0 || position.Lat != 0) && position.Lon > -180 && position.Lon < 180 && position.Lat > -90 && position.Lat < 90)
                {
                    double scale = _locator.Locator.Scale;
                    scale *=  _radar.RadarStatus.Range / 60 * 2 / Math.Max(0.00001, Math.Min(_locator.Rect.Width, _locator.Rect.Height)) * 1.1;
                    _locator.Locator.Locate(scale, position);
                }
            }
            else if (e.PropertyName == "RadarConfigChanged" || e.PropertyName == "RadarConfigChangedFromClient")
            {
                if (_radarImage != null)
                    updateRadarImageFromConfig();
            }   
            else if (e.PropertyName == "IsEnable")
            {
                loadRadarInfo();
                onRefreshed();
            }
        }

        private void updateRadarImageFromConfig()
        {
            _radarImage.UpdateRadarData(_radar);
        }

        private void startRpc()
        {
            stopTimer();
            if (!_radar.IsEnable)
                return;
            _radarControl.Start(_radar.RadarAddress.RpcEndPoint);
            startTimer();
        }

        private void loadRadarImage()
        {
            if (_radarImage == null)
            {
                _radarImage = new RadarImage(_locator);
                _radarImage.Visibility = System.Windows.Visibility.Visible;
                _radarImage.ColorTableIndex = _radar.RadarAddress.ColorTableIndex;
                this.Children.Add(_radarImage);
            }
        }

        private void restartRadarImage()
        {
            if (_radarImage == null)
                return;
            if (_radarImage.IsPlaying)
            {
                disposeRadarImage();
                loadRadarImage();
            }
            if (!DataEligibleDetection.IsEffectPort(_radar.RadarAddress.Port) || !DataEligibleDetection.IsEffectIp(_radar.RadarAddress.Ip))
            {
                LogService.Error("雷达配置错误，请检查！" + Environment.NewLine + _radar.RadarAddress.Ip + ":" + _radar.RadarAddress.Port);
                System.Windows.MessageBox.Show("雷达配置错误，请检查！" + Environment.NewLine + _radar.RadarAddress.Ip + ":" + _radar.RadarAddress.Port);
                return;
            }
            _radarImage.Start(_radar.RadarAddress.Ip, _radar.RadarAddress.Port);
            if (_radar.RadarStatus == null)
                return;
            _radarImage.ScreenWidth = _width;
            _radarImage.ScreenHeight = _height;
            updateRadarImageFromConfig();
        }

        private void startTimer()
        {
            _refreshTimes = 0;
            _timer = new PeriodTimer(1000, onTimerCallBack);
        }

        private void stopTimer()
        {
            if (_timer != null)
                _timer.Dispose();
            _timer = null;
        }

        static int RefreshRate = 3600;
        int _refreshTimes = 0;
        private void onTimerCallBack(object state)
        {
            if (_refreshTimes % 3600 == 0 || (!_radarControl.IsLinking && _refreshTimes % 10 == 0))
            {
                this.Dispatcher.BeginInvoke(new Action(delegate() {
                    _radarControl.LoadData(_radar);
                }));
            }
            else
            {
                var rates = _radarControl.Rates;
                if (rates != null)
                {
                    _radar.InitRadarChannelsRate(rates);
                    _radarControl.ResetRadarStatus(_radar);
                }
            }
            _refreshTimes = (_refreshTimes + 1) % RefreshRate;
        }

        #region 雷达显示部分

        public bool InRadarCoverageArea(MapPoint position)
        {
            double dist = PointsBaseInfomation.GetDelta(_radar.Position, position);
            return Math.Abs(dist) <= _radar.RadarInfo.EndRange;
        }

        public void Offset(double lon, double lat)
        {
            if (_radar.RadarStatus == null)
                return;
            var radarConfig = _radar.RadarStatus;
            radarConfig.Longitude += lon;
            radarConfig.Latitude += lat;
            _radar.SetRadarConfig(radarConfig);
        }

        public void Drag(MapPoint mousePosition, MapPoint endPosition)
        {
            if (_radar.RadarStatus == null)
                return;
            var radarStatus = _radar.RadarStatus;
            double angleBegin, angleEnd;
            double distBegin = PointsBaseInfomation.GetDelta(_radar.Position, mousePosition, out angleBegin);
            double distEnd = PointsBaseInfomation.GetDelta(_radar.Position, endPosition, out angleEnd);
            double scale = (distEnd - radarStatus.StartRange) / (distBegin - radarStatus.StartRange);
            if (scale > 0)
                radarStatus.Range *= scale;
            double angle = angleBegin - angleEnd;//顺时针旋转
            radarStatus.OffsetAngle = PointsBaseInfomation.GetStandardAngle(radarStatus.OffsetAngle - angle);
            _radar.SetRadarConfig(radarStatus);
        }

        int _width = -1;
        int _height = -1;
        public void OnScreenChanged(int width, int height)
        {
            _width = width;
            _height = height;
            onRefreshed();
        }

        private void onRefreshed()
        {
            if (_radarImage != null && _width > 0 && _height > 0)
            {
                _radarImage.ScreenWidth = _width;
                _radarImage.ScreenHeight = _height;
                _radarImage.UpdateRadarDraw();
            }
        }
        
        public void Transform(double x, double y)
        {
            _radarImage.Transform(x, y);
        }

        #endregion

        public void Dispose()
        {
            stopTimer();
            if (_radar != null)
            {
                _radar.IsEnable = false;
                _radar.PropertyChanged -= _radar_PropertyChanged;
            }
            _radar = null;
            disposeRadarImage();
            _radarControl.Stop();
        }

        private void disposeRadarImage()
        {
            if (_radarImage != null)
            {
                _radarImage.Stop();
                this.Children.Remove(_radarImage);
            }
            _radarImage = null;
        }
    }
}