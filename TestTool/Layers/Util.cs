using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VTSCore.Data.Common;
using VTSCore.Layers.Maps;

namespace VTSCore.Layers
{
	public static class LocatorExtension
	{
		public static Task SetAsCenter(this ILocator locator, MapPoint mapPoint)
		{
			return locator.Locate(locator.Scale, mapPoint);
		}

		public static Task SetAsCenter(this ILocator locator, Point point)
		{
			var mapPoint = locator.ScreenToMap(point.X, point.Y);
			return locator.SetAsCenter(mapPoint);
		}

		public static Task Locate(this ILocator locator, double scale, double lon, double lat)
		{
			return locator.Locate(scale, new MapPoint(lon, lat));
		}

		public static Task Offset(this ILocator locator, double dx, double dy)
		{
			var center = locator.MapToScreen(locator.Center.Lon, locator.Center.Lat);
			center.Offset(-dx, -dy);

			var newCenter = locator.ScreenToMap(center.X, center.Y);
			return locator.Locate(locator.Scale, newCenter);
		}
	}
}
