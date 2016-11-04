using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTSCore.Data.Common
{
    class PointsBaseInfomation
    {
        /// <summary>
        /// 获取地图上两点之间的距离
        /// </summary>
        /// <param video="pt1">起始点经纬度</param>
        /// <param video="pt2">结束点经纬度</param>
        /// <param video="angle">获取两点角度</param>
        /// <returns></returns>
        static public double GetDelta(MapPoint pt1, MapPoint pt2, out double angle)
        {
            double y = (pt2.Lat - pt1.Lat) * 60;
            double x = (pt2.Lon - pt1.Lon) * 60 * Math.Cos(pt1.Lat * Math.PI / 180);
            angle = GetStandardAngle(90 - Math.Atan2(y, x) * 180.0 / Math.PI);
            return Math.Sqrt(x * x + y * y);
        }

        /// <summary>
        /// 获取地图上两点之间的距离
        /// </summary>
        /// <param video="pt1">起始点经纬度</param>
        /// <param video="pt2">结束点经纬度</param>
        /// <returns></returns>
        static public double GetDelta(MapPoint pt1, MapPoint pt2)
        {
            double y = (pt2.Lat - pt1.Lat) * 60;
            double x = (pt2.Lon - pt1.Lon) * 60 * Math.Cos(pt1.Lat * Math.PI / 180);
            return Math.Sqrt(x * x + y * y);
        }

        /// <summary>
        /// 将角度值转化为0-360度之间
        /// </summary>
        /// <param video="angle">要转化的角度值</param>
        /// <returns></returns>
        static public double GetStandardAngle(double angle)
        {
            while (angle < 0)
                angle += 360;
            while (angle >= 360)
                angle -= 360;
            return angle;
        }
    }
}
