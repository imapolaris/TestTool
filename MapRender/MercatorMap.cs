using SeeCool.Geometry.Unit;
using SeeCool.Geometry.Util.Projection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.SeaMap
{
	public abstract class MercatorMap
	{
		Func<Size> _controlSizeGetter;
		public MercatorMap(Func<Size> controlSizeGetter)
		{
			this._controlSizeGetter = controlSizeGetter;
		}

		public Rectangle ClientRectangle
		{
			get
			{
				var size = _controlSizeGetter();
				return new Rectangle(0, 0, size.Width, size.Height); 
			}
		}

		public RectangleD _extent = new RectangleD();

		#region 成员

		MercatorProjection _projection = new MercatorProjection(0, 0);


		/// <summary>
		/// 电子海图显示的缩放因子
		/// </summary>
		private float _zoomFactor = 1.0F;

		private PointD _center = PointD.Empty;
		private double _scale = 0.0;


		/// <summary>
		/// 电子海图最大显示比例尺
		/// </summary>
		private const int MinMapScale = 1000;

		/// <summary>
		/// 电子海图最小显示比例尺
		/// </summary>
		private const int MaxMapScale = 100000000;

		/// <summary>
		/// 电子海图开窗缩放的最小像素边长
		/// </summary>
		private const int MinMouseMove = 10;

		/// <summary>
		/// 在当前比例尺下米和像素的换算比例
		/// </summary>
		private double _scaleRate = 0;

		/// <summary>
		/// 中心的大地平面坐标
		/// </summary>
		private PointD _centerFlat;



		#endregion

		#region 功能函数
		/// <summary>
		/// 设置电子海图显示中心经纬度和显示比例尺
		/// TODO：尚未实现旋转问题
		/// </summary>
		/// <param name="center"></param>
		/// <param name="scale"></param>
		public void setMapCenter(PointD center, double scale)
		{

			_center = center;

			double oldscale = _scale;
			//检查显示比例尺的合理性
			_scale = scale;
			if (_scale < MinMapScale)
				_scale = MinMapScale;
			if (_scale > MaxMapScale)
				_scale = MaxMapScale;

			//TODO:这个算法没看出什么严格的数学依据
			//计算缩放因子，20000以下原样显示，900000以上二分之一大小显示
			//20000到900000之间按照(Math.Sin(Math.PI * (40000 / scale + 1) / 6))取值
			if (scale > 900000)
				_zoomFactor = 0.5f;
			else if (scale < 20000)
				_zoomFactor = 1f;
			else
				_zoomFactor = (float)(Math.Sin(Math.PI * (40000 / scale + 1) / 6));

			//计算当前比例尺下像素和米的换算关系
			_scaleRate = _scale * SeeCool.Geometry.Util.CommenData.PPM;

			//计算中心点的大地平面坐标，单位：米
			_centerFlat = _projection.MapToFlat(_center.X, _center.Y);
			//计算四分之一屏幕的大地平面坐标跨度，需要注意：因为是将屏幕中心位置左上角像素作为中心点，所以Width和Height均要减一
			double cx = ((double)(this.ClientRectangle.Width - 1) / 2) * _scaleRate;
			double cy = ((double)(this.ClientRectangle.Height - 1) / 2) * _scaleRate;

			//计算屏幕的经纬度范围
			PointD lt = _projection.FlatToMap(_centerFlat.X - cx, _centerFlat.Y - cy);
			PointD rb = _projection.FlatToMap(_centerFlat.X + cx, _centerFlat.Y + cy);
			_extent = RectangleD.FromLTRB(lt.X, lt.Y, rb.X, rb.Y);
		}


		/// <summary>
		/// 屏幕坐标转空间坐标
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public virtual PointD ScreenToMap(double x, double y)
		{
			double cx = (x - (double)(this.ClientRectangle.Width - 1) / 2) * _scaleRate + _centerFlat.X;
			double cy = ((double)(this.ClientRectangle.Height - 1) / 2 - y) * _scaleRate + _centerFlat.Y;
			return _projection.FlatToMap(cx, cy);
		}

		/// <summary>
		/// 空间坐标转屏幕坐标
		/// </summary>
		/// <param name="lon"></param>
		/// <param name="lat"></param>
		/// <returns></returns>
		public virtual System.Drawing.Point MapToScreen(double lon, double lat)
		{
			var pt = System.Drawing.Point.Empty;

			PointD pd = _projection.MapToFlat(lon, lat);
			pt.X = (int)((pd.X - _centerFlat.X) / _scaleRate + (double)(this.ClientRectangle.Width - 1) / 2);
			pt.Y = (int)((double)(this.ClientRectangle.Height - 1) / 2 - (pd.Y - _centerFlat.Y) / _scaleRate);

			return pt;
		}
		#endregion
	}
}
