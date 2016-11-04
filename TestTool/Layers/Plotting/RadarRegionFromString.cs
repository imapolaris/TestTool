using Seecool.Radar;
using Seecool.Radar.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTSCore.Layers.Plotting
{
    public class RadarRegionFromString
    {
        public static RadarRegion GetRegion(string regionString)
        {
            char[] ch = new char[] {',', ' ', '\r', '\n' };
            string[] datas = regionString.Split(ch, StringSplitOptions.RemoveEmptyEntries);
            if (regionString != null)
            {
                int index = 0;
                RadarRegion region = new RadarRegion() { Name = datas[index++], IsMask = true };
                var points = new Seecool.Radar.Unit.PointD[(datas.Length - 1) / 2];
                for (int i = 0; i < points.Length; i++)
                {
                    points[i].X = double.Parse(datas[index++]);
                    points[i].Y = double.Parse(datas[index++]);
                }
                region.Polygon = points;
                return region;
            }
            return null;
        }

        public static PointD[] GetRegion(string[] datas)
        {
            if (datas.Length == 0 || datas[0] != "Polygon")
                return null;
            List<PointD> points = new List<PointD>();
            for(int i = 1; i < datas.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(datas[i]))
                    continue;
                points.Add(getPositionByDMSs(datas[i]));
            }
            return points.ToArray();
        }

        private static PointD getPositionByDMSs(string str)
        {
            string[] datas = str.Split(' ');
            return new PointD(getDataByDMS(datas[0]),getDataByDMS(datas[1]));
        }

        private static double getDataByDMS(string strDMS)
        {
            double value = 0;
            string[] datas = strDMS.Split('°', '\'', '\"');
            value = double.Parse(datas[0]);
            if (datas.Length > 1)
                value += double.Parse(datas[1]) / 60;
            if(datas.Length > 2)
                value+=double.Parse(datas[2]) / 3600;
            if(datas.Length > 3)
            {
                if (datas[3] == "W" || datas[3] == "S")
                    value *= -1;
            }
            return value;
        }
    }
}
