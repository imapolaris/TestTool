using DynamicBaseCanvas;
using SeeCool.Geometry.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTSCore.Layers.Maps
{
    public class LocatorAndBorder
    {
        public static LocatorAndBorder Instance { get; private set; }

        public double ScaleTrans { get; private set; }

        public TargetDisplayStatus DisplayStatus { get; private set; }

        static LocatorAndBorder()
        {
            Instance = new LocatorAndBorder();
        }
        private LocatorAndBorder()
        {
            ScaleTrans = 0;
            DisplayStatus = TargetDisplayStatus.UnShow;
        }

        public event Action OnMapRefreshed;
        public ILocator Locator { get; private set; }

        public RectangleD Rect = new RectangleD();

        public void InitLocator(ILocator locator)
        {
            Locator = locator;
            UpdateLocator();
        }
        public void UpdateLocator()
        {
            if (Locator == null)
                return;
            double radiusLon = Locator.MapSize.Width / 60 / Math.Cos(Locator.Center.Lat * Math.PI / 180) / 2;
            double radiusLat = Locator.MapSize.Height / 60 / 2;
            Rect.Left = Locator.Center.Lon - radiusLon;
            Rect.Right = Locator.Center.Lon + radiusLon;
            Rect.Top = Locator.Center.Lat - radiusLat;
            Rect.Bottom = Locator.Center.Lat + radiusLat;
            updateShowStatus();
            if (OnMapRefreshed != null)
                OnMapRefreshed();
        }

        public bool InScreen(double lon, double lat)
        {
            if (Locator == null)
                return false;
            if (lon >= Rect.Left && lon < Rect.Right && lat >= Rect.Top && lat < Rect.Bottom)
                return true;
            return false;
        }

        public bool InScreenAtCircle(double lon, double lat, double radius)
        {
            if (Locator == null)
                return false;
            double radiusOnLon = radius / Math.Cos(Locator.Center.Lat * Math.PI / 180);
            if (lon + radiusOnLon < Rect.Left || lat + radius < Rect.Top || lon - radiusOnLon > Rect.Right || lat - radius > Rect.Bottom)
                return false;
            if ((lon >= Rect.Left && lon <= Rect.Right) || (lat >= Rect.Top && lat <= Rect.Bottom))
                return true;
            double lonDif = lon < Rect.Left ? Rect.Left : Rect.Right;
            double latDif = lat < Rect.Top ? Rect.Top : Rect.Bottom;
            VTSCore.Data.Common.MapPoint dif = new Data.Common.MapPoint(lonDif, latDif);
            double dist = VTSCore.Data.Common.PointsBaseInfomation.GetDelta(dif, new Data.Common.MapPoint(lon, lat));
            if (dist / 60 < radius)
                return true;
            else
                return false;
        }

        public bool InScreenAtRectangle(double lonInf, double lonSup, double latInf, double latSup)
        {
            if (Locator == null || Rect.Left >= lonSup || Rect.Right <= lonInf || Rect.Bottom <= latInf || Rect.Top >= latSup)
                return false;
            else
                return true;
        }

        private void updateShowStatus()
        {
            double scale = Locator.Scale;
            if (scale <= 150000)
            {
                DisplayStatus = TargetDisplayStatus.LargeIcon;
                ScaleTrans = 1.0 / scale * 1852 / 3600 * 3950 * 60 * 1;//1 minute
            }
            else
            {
                DisplayStatus = TargetDisplayStatus.SmallIcon;
                ScaleTrans = 0;
            }
        }
    }
}
