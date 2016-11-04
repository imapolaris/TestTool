using SeeCool.GISFramework.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTSCore.Layers.Tracks
{
    [Serializable]
    public class SDGPSData : DynamicMovableGeometryObj
    {
        public int Position;//定位方式
        public int Distance;//里程数

        public double Lon
        {
            get { return ((GeoPointShape)Shape).Point.X; }
        }

        public double Lat
        {
            get { return ((GeoPointShape)Shape).Point.Y; }
        }

        public override string Type
        {
            get { return "SDGPSData"; }
        }

        public override bool IsTimeout
        {
            get
            {
                TimeSpan ts = DateTime.Now - this.Time;
                int std = 10 * 60;
                return ts.TotalSeconds > std;
            }
        }

        public override string Format()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SDGPS,");
            sb.Append(this.Src);
            sb.Append(",");
            sb.Append(this.Id);
            sb.Append(",");
            sb.Append(this.Time.ToString());
            sb.Append(",");
            sb.Append(this.Name);
            sb.Append(",");
            sb.Append(this.Lon.ToString("F6"));
            sb.Append(",");
            sb.Append(this.Lat.ToString("F6"));
            sb.Append(",");
            sb.Append(this.SOG.ToString("F1"));
            sb.Append(",");
            sb.Append(this.COG.ToString("F1"));
            sb.Append(",");

            sb.Append(this.Position.ToString());
            sb.Append(",");
            sb.Append(this.Distance.ToString());



            string result = sb.ToString();
            return result;
        }

        public override void Parse(string[] data)
        {
            int index = 1;
            this.Src = data[index++];
            this.Id = data[index++];
            DateTime time = DateTime.Now;
            DateTime.TryParse(data[index++], out time);
            this.Time = time;
            this.Name = data[index++];
            double lon = 0;
            double.TryParse(data[index++], out lon);
            double lat = 0;
            double.TryParse(data[index++], out lat);
            this.Shape = new GeoPointShape(lon, lat);
            double.TryParse(data[index++], out this.SOG);
            double.TryParse(data[index++], out this.COG);
            int.TryParse(data[index++], out this.Position);
            int.TryParse(data[index++], out this.Distance);


        }

    }
}
