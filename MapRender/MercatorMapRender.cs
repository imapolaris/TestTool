using SeeCool.ECDIS;
using SeeCool.ECDIS.S52;
using SeeCool.ECDIS.S57;
using SeeCool.Geometry.Unit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.SeaMap
{
	public class MercatorMapRender : MercatorMap,IDisposable
	{
		public static readonly double DefaultScale = 50000;
		//public static readonly PointD DefaultCenter = new PointD(117.878367, 38.954685);//天津
        public static readonly PointD DefaultCenter = new PointD(121.4887, 31.2114);//上海

		static S57FileMgr _fileMgr;
        static string _mapDir = @".\SeaMap";
		static MercatorMapRender()
		{
            string path = Directory.GetParent(System.Windows.Forms.Application.LocalUserAppDataPath).FullName;
            _mapDir = Path.Combine(path, "Charts");
            if (!Directory.Exists(_mapDir))
                Directory.CreateDirectory(_mapDir);
            //if (!System.IO.Directory.Exists(_mapDir))
            //    throw new InvalidDataException("找不到海图路径");

            S57FileParser.Init();

			_fileMgr = new S57FileMgr();
			_fileMgr.IsStartClearWork = true;
			_fileMgr.OpenDirectory(_mapDir, false);

           // MapImageService.InitPainterPool(_mapDir, 1, 4);
		}

        public static ReadOnlyCollection<S57File> Files()
        {
            return _fileMgr.Files;
        }

        PiecePainter _painter = null;

        public static void AddMapRender(string[] fileNames)
        {
            foreach(var file in fileNames)
            {
                string fileName = Path.GetFileName(file);
                File.Copy(file, Path.Combine(_mapDir, fileName), true);
            }
            _fileMgr.OpenDirectory(_mapDir, false);
        }

		public double RotateAngle { get; set; }
		public double Scale { get; set; }
		public PointD Center{get; set;}

		S52DisplaySetting _setting;

		private MercatorMapRender(Func<Size> controlSizeGetter)
			: base(controlSizeGetter)
		{
            _setting = new S52DisplaySetting() { Palette = SeeCool.ECDIS.S52.ColorSchema.Day, ShowScaleBar = true, ShowCompass = true, ShowGrid = false };
			this.Center = DefaultCenter;
			this.Scale = DefaultScale;

           

            _painter = new PiecePainter(_fileMgr, 1, _setting);
		}

        public MercatorMapRender(Func<Size> controlSizeGetter, DisplaySetting displaySetting, PointD center, double scale)
            : base(controlSizeGetter)
        {
            _setting = displaySetting.ToS52DisplaySetting();
            this.Center = center;
            this.Scale = scale;

            _painter = new PiecePainter(_fileMgr, 1, _setting);
        }

		public MercatorMapRender(Func<Size> controlSizeGetter, PointD center, double scale)
			: this(controlSizeGetter)
		{
			this.Center = center;
			this.Scale = scale;
		}

		public MemoryStream LoadMap()
		{
			lock (this)
			{
				this.setMapCenter(Center, Scale);

				using (var bmp = _painter.GetPicture(Center, (int)Scale, RotateAngle, ClientRectangle.Size.Width, ClientRectangle.Size.Height, 0))
				//using (var bmp = MapImageService.CreateMapImage(this._extent, this.ClientRectangle.Size, _setting))
				{
					var ms = new System.IO.MemoryStream();
					bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
					ms.Position = 0;
					return ms;
				}
			}
			
		}

        public void UpdateCleats(DisplaySetting displaySetting)
        {
            _setting = displaySetting.ToS52DisplaySetting();
            _painter = new PiecePainter(_fileMgr, 1, _setting);
        }

		public override Point MapToScreen(double lon, double lat)
		{
            try
            {
                return _painter.MapToScreen(lon, lat);
            }
            catch (NullReferenceException)
            {
                return Point.Empty;
            }
            catch (IndexOutOfRangeException)
            {
                return Point.Empty;
            }
		}

		public override PointD ScreenToMap(double x, double y)
		{
            try
            {
                return _painter.ScreenToMap((int)x, (int)y);
            }
            catch (IndexOutOfRangeException)
            {
                return PointD.Empty;
            }
		}

		public void Offset(double x, double y)
		{
			var size = this.ClientRectangle.Size;
			var centerPos = new Point(size.Width / 2, size.Height / 2);

			var p = ScreenToMap(centerPos.X, centerPos.Y);

			centerPos.X -= (int)x;
			centerPos.Y -= (int)y;

			Center = ScreenToMap(centerPos.X, centerPos.Y);
		}

		public void Dispose()
		{
			_painter.Dispose();
		}
	}
}
