using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Seecool.Radar.Unit;

namespace VTSCore.Data.Common
{
	public class MapPoint
	{
		public double Lon { get; set; }
		public double Lat { get; set; }

		public MapPoint(double lon, double lat)
		{
			this.Lon = lon;
			this.Lat = lat;
		}

		public MapPoint(PointD point)
			: this(point.X, point.Y)
		{
		}

		public override string ToString()
		{
			return string.Format("{0},{1}", Lon, Lat);
		}
	}

	public interface ICoordConverter
	{
		Point MapToScreen(double lon, double lat);
		MapPoint ScreenToMap(double x, double y);
	}
}
