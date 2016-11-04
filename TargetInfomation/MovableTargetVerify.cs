using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TargetInfomation
{
    public class MovableTargetVerify
    {
        DateTime _lastestTime;
        double _lon;
        double _lat;
        public static double AngleSup = 0.0009;//100m
        bool IsRunning { get; set; }

        /// <summary>
        /// 检查目标经纬度是否处于异常工作状态
        /// </summary>
        /// <param name="lon">经度，单位（度）</param>
        /// <param name="lat">纬度，单位（度）</param>
        /// <param name="dateTime">更新时间</param>
        /// <returns>true表示在正常区间工作，false表示经纬度不在正常区间</returns>
        public bool Update(double lon, double lat, DateTime dateTime)
        {
            bool backValue = true;
            if (!IsRunning)
                IsRunning = true;
            else
                backValue = (lonLatChanged(lon, lat) < AngleSup);

            _lastestTime = dateTime;
            _lon = lon;
            _lat = lat;
            return backValue;
        }

        private double lonLatChanged(double lon, double lat)
        {
            double lonDif = Math.Abs(lon - _lon) * Math.Cos(lat * Math.PI / 180);
            double latDif = Math.Abs(lat - _lat);
            double angleDif = Math.Sqrt(lonDif * lonDif + latDif * latDif);
            return angleDif;
        }
    }
}