using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VTSCore.Utility;

namespace VTSCore.Layers.Maps.Parts
{
	class Zoom : IPart
	{
		[Import]
		SeaMapInfo _seaMapInfo = null;

		[Import]
		ILocator _locator = null;

		[Import]
		IMouseEventSource _mouseEventSource = null;

		public void Init()
		{
			_seaMapInfo.MapFrame.AddCommandBinding(Commands.Zoom.ZoomOut, zoomOut_Excuted);
			_seaMapInfo.MapFrame.AddCommandBinding(Commands.Zoom.ZoomIn, zoomIn_Excuted);

			DispatchZoomMap();
			_mouseEventSource.MouseWheel.Subscribe(Delta =>
                {
                    resetZoom();
                    _zoomCount += Delta / 120;
                    processZoom(true);
                });
        }

        int _zoomCount = 0;
		Subject<Tuple<bool>> _zoomRequests = new Subject<Tuple<bool>>();
        void processZoom(bool keepMouseFixed)
        {
            _zoomRequests.OnNext(Tuple.Create(keepMouseFixed));
        }
        double _scaleData = 0;
        double _lon = 0;
        double _lat = 0;
		async void DispatchZoomMap()
		{
			//处理缩放请求
			while (true)
			{
				var para = await _zoomRequests.Any();//.FirstAsync();
                if (_scaleData == 0)
                    _scaleData = _locator.Scale;

                var zoomRate = getZoomRate();
				var point = Mouse.GetPosition(_seaMapInfo.MapFrame);
				var mousePoint = _locator.ScreenToMap(point.X, point.Y);
				var oldScale = _locator.Scale;

                _scaleData = MapBorderInfo.CorrectionScale(_scaleData / zoomRate);
				var lon = _locator.Center.Lon;
				var lat = _locator.Center.Lat;
				
				if (true)
				{
                    var rot = _scaleData / oldScale;
					var dy = mousePoint.Lat - _locator.Center.Lat;
					var dx = _locator.Center.Lon - mousePoint.Lon;

					lat = lat - dy * (rot - 1);
					lon = lon + dx * (rot - 1);
				}

                await _locator.Locate(_scaleData, lon, lat);
			}
		}

        private void resetZoom()
        {
            if (_locator.Center.Lon != _lon || _locator.Center.Lat != _locator.Center.Lat)
            {
                _zoomCount = 0;
                _scaleData = 0;
                _lon = _locator.Center.Lon;
                _lat = _locator.Center.Lat;
            }
        }

        private double getZoomRate()
        {
            var zoomRate = 1.0;
            while (_zoomCount != 0)
            {
                if (_zoomCount > 0)
                {
                    zoomRate *= 1.1;
                    _zoomCount--;
                }
                else
                {
                    zoomRate /= 1.1;
                    _zoomCount++;
                }
            }
            return zoomRate;
        }

		void zoomOut_Excuted(object sender, ExecutedRoutedEventArgs e)
		{
			processZoom(false);
		}

		void zoomIn_Excuted(object sender, ExecutedRoutedEventArgs e)
		{
			processZoom(false);
		}
	}
}