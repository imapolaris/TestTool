using System;
using System.Collections.Generic;
using System.Text;

namespace SeeCool.GISFramework.Object
{
    [Serializable]
    public class WXAISTele18 : ShipObj
    {
        public int CommState;
        public bool PositionAccuracy;
        public bool RAIM_Flag;
        public int TimeStamp;

        public override string Type
        {
            get { return "WXAISTELE18"; }
        }

        public override bool IsTimeout
        {
            get
            {
                TimeSpan ts = DateTime.Now - this.Time;
                int std = 60 * 60 * 3;
                //if (this.SOG != WXAISTele123.INVALID_SOG)
                //{
                //    if (this.SOG > 2)
                //        std = 30;
                //    else if (this.SOG > 14)
                //        std = 15;
                //    else if (this.SOG > 23)
                //        std = 5;
                //}
                return ts.TotalSeconds > std * 3;
            }
        }

        public override string Format()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("WXAIS,18,");
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
            sb.Append(this.Heading.ToString());
            sb.Append(",");
            sb.Append(this.CommState.ToString());
            sb.Append(",");
            sb.Append(this.PositionAccuracy.ToString());
            sb.Append(",");
            sb.Append(this.RAIM_Flag.ToString());
            sb.Append(",");
            sb.Append(this.TimeStamp.ToString());
            sb.Append(",");
            string result = sb.ToString();
            return result;
        }

        public override void Parse(string[] data)
        {
            int index = 2;
            this.Id = data[index++];
            this.MMSI = Int32.Parse(this.Id);
            this.Time = DateTime.Parse(data[index++]);
            this.Name = data[index++];
            double lon = Double.Parse(data[index++]);
            double lat = Double.Parse(data[index++]);
            this.Shape = new GeoPointShape(lon, lat);
            this.SOG = Double.Parse(data[index++]);
            this.COG = Double.Parse(data[index++]);
            this.Heading = Int32.Parse(data[index++]);
            this.CommState = int.Parse(data[index++]);
            this.PositionAccuracy = bool.Parse(data[index++]);
            this.RAIM_Flag = bool.Parse(data[index++]);
            this.TimeStamp = int.Parse(data[index++]);
        }

        protected override string[] relatedUnqiueIds
        {
            get { return new string[] { "WXAIS" + "." + this.Src + "." + this.Id }; }
        }
    }
}
