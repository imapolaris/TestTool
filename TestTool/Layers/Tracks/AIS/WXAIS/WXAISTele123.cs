using System;
using System.Collections.Generic;
using System.Text;

namespace SeeCool.GISFramework.Object
{
    [Serializable]
    public class WXAISTele123 : ShipObj
    {
        public const double INVALID_SOG = 102.3;

        public int NavStatus;
        public int CommStateValue;
        public bool PositionAccuracy;
        public bool RAIM_Flag;
        public int ROT_AIS;
        public int TimeStamp;
        public DateTime UTCTime = DateTime.MinValue;

        public override string Type
        {
            get { return "WXAISTELE123"; }
        }

        public override bool IsTimeout
        {
            get
            {
                TimeSpan ts = DateTime.Now - this.Time;
                int std = 60 * 60 * 3;
                //if (this.SOG != WXAISTele123.INVALID_SOG)
                //{
                //    if (this.SOG > 3)
                //        std = 10;
                //    else if (this.SOG > 14)
                //        std = 6;
                //    else if (this.SOG > 23)
                //        std = 2;
                //}
                return ts.TotalSeconds > std * 3;
            }
        }

        public override string Format()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("WXAIS,1,");
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
            sb.Append(this.NavStatus.ToString());
            sb.Append(",");
            sb.Append(this.CommStateValue.ToString());
            sb.Append(",");
            sb.Append(this.PositionAccuracy.ToString());
            sb.Append(",");
            sb.Append(this.RAIM_Flag.ToString());
            sb.Append(",");
            sb.Append(this.ROT_AIS.ToString());
            sb.Append(",");
            sb.Append(this.TimeStamp.ToString());
            sb.Append(",");
            sb.Append(this.UTCTime.ToString());
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
            this.NavStatus = Int32.Parse(data[index++]);
            this.CommStateValue = Int32.Parse(data[index++]);
            this.PositionAccuracy = bool.Parse(data[index++]);
            this.RAIM_Flag = bool.Parse(data[index++]);
            this.ROT_AIS = Int32.Parse(data[index++]);
            this.TimeStamp = Int32.Parse(data[index++]);
            if (index < data.Length)
            {
                string utc = data[index++];
                if (!string.IsNullOrEmpty(utc))
                    DateTime.TryParse(utc, out this.UTCTime);
            }
        }

        protected override string[] relatedUnqiueIds
        {
            get { return new string[] { "WXAIS" + "." + this.Src + "." + this.Id }; }
        }

        public static string GetNavStatusDesc(int status)
        {
            switch (status)
            {
                case 0:
                    return "����������";
                case 1:
                    return "ê��";
                case 2:
                    return "ʧ��";
                case 3:
                    return "����������";
                case 4:
                    return "��ˮ����";
                case 5:
                    return "ϵ��";
                case 6:
                    return "��ǳ";
                case 7:
                    return "���²���";
                case 8:
                    return "ʻ�纽��";
                default:
                    return "δ֪";
            }
        }
    }
}
