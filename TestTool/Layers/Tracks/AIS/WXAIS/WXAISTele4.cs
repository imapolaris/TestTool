using System;
using System.Collections.Generic;
using System.Text;

namespace SeeCool.GISFramework.Object
{
    [Serializable]
    public class WXAISTele4 : DynamicGeometryObj
    {
        public string UTC;

        public override bool IsTimeout
        {
            get
            {
                TimeSpan ts = DateTime.Now - this.Time;
                int std = 60 * 60 * 3;
                return ts.TotalSeconds > std * 3;
            }
        }

        public override string Format()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("WXAIS,4,");
            sb.Append(this.Id);
            sb.Append(",");
            sb.Append(this.Time.ToString());
            sb.Append(",");
            sb.Append(this.Name);
            sb.Append(",");
            GeoPointShape gps = this.Shape as GeoPointShape;
            sb.Append(gps.Point.X.ToString("F6"));
            sb.Append(",");
            sb.Append(gps.Point.Y.ToString("F6"));
            sb.Append(",");
            sb.Append(this.UTC);
            sb.Append(",");
            string result = sb.ToString();
            return result;
        }

        public override void Parse(string[] data)
        {
            int index = 2;
            Id = data[index++];
            Time = DateTime.Parse(data[index++]);
            Name = data[index++];

            double x = double.Parse(data[index++]);
            double y = double.Parse(data[index++]);
            Shape = new GeoPointShape(x, y);
            UTC = data[index++];
        }

        public override string Type
        {
            get { return "WXAISTELE4"; }
        }
    }
}
