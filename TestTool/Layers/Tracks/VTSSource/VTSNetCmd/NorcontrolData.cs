using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeeCool.GISFramework.Object
{
    [Serializable]
    public class NorcontrolData : ShipObj
    {
        public bool Movable;
        public int SensorType;
        public int AISTypeMask;
        public int OrgType;
        public string CallSign;
        public string Destination;
        public DateTime ETA;
        public int IMO_Number;
        public double MaxSeaGauge;
        public int NavStatus;
        public int ROT = -731;
        public int Persons;
        public int RefToLarboard;
        public int RefToProw;
        public int ShipCargoType;
        public int Length;
        public int Width;

        public override string Type
        {
            get { return "NORCONTROL"; }
        }

        public override bool IsTimeout
        {
            get
            {
                TimeSpan ts = DateTime.Now - this.Time;
                return ts.TotalSeconds > 3 * 3;
            }
        }

        public override string Format()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("NC,");
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
            sb.Append(this.MMSI.ToString());
            sb.Append(",");
            sb.Append(this.Movable.ToString());
            sb.Append(",");
            sb.Append(this.SensorType.ToString());
            sb.Append(",");
            sb.Append(this.AISTypeMask.ToString());
            sb.Append(",");
            sb.Append(this.OrgType.ToString());
            sb.Append(",");
            sb.Append(this.CallSign);
            sb.Append(",");
            sb.Append(this.Destination);
            sb.Append(",");
            sb.Append(this.ETA.ToString());
            sb.Append(",");
            sb.Append(this.IMO_Number.ToString());
            sb.Append(",");
            sb.Append(this.Length.ToString());
            sb.Append(",");
            sb.Append(this.MaxSeaGauge.ToString());
            sb.Append(",");
            sb.Append(this.NavStatus.ToString());
            sb.Append(",");
            sb.Append(this.Persons.ToString());
            sb.Append(",");
            sb.Append(this.RefToLarboard.ToString());
            sb.Append(",");
            sb.Append(this.RefToProw.ToString());
            sb.Append(",");
            sb.Append(this.ShipCargoType.ToString());
            sb.Append(",");
            sb.Append(this.Width.ToString());
            sb.Append(",");
            sb.Append(this.GID);
            sb.Append(",");
            sb.Append(this.FID);
            sb.Append(",");
            sb.Append(this.ROT);
            string result = sb.ToString();
            return result;
        }

        public override void Parse(string[] data)
        {
            int index = 1;
            this.Id = data[index++];
            this.Time = DateTime.Parse(data[index++]);
            this.Name = data[index++];
            double x = 0;
            double.TryParse(data[index++], out x);
            double y = 0;
            double.TryParse(data[index++], out y);
            this.Shape = new GeoPointShape(x, y);
            double.TryParse(data[index++], out this.SOG);
            double.TryParse(data[index++], out this.COG);
            int.TryParse(data[index++], out this.Heading);
            int.TryParse(data[index++], out this.MMSI);
            bool.TryParse(data[index++], out this.Movable);
            int.TryParse(data[index++], out this.SensorType);
            int.TryParse(data[index++], out this.AISTypeMask);
            int.TryParse(data[index++], out this.OrgType);
            this.CallSign = data[index++];
            this.Destination = data[index++];
            DateTime.TryParse(data[index++], out this.ETA);
            int.TryParse(data[index++], out this.IMO_Number);
            int length = 0;
            int.TryParse(data[index++], out length);
            this.Length = length;
            double.TryParse(data[index++], out this.MaxSeaGauge);
            int.TryParse(data[index++], out this.NavStatus);
            int.TryParse(data[index++], out this.Persons);
            int.TryParse(data[index++], out this.RefToLarboard);
            int.TryParse(data[index++], out this.RefToProw);
            int.TryParse(data[index++], out this.ShipCargoType);
            int width = 0;
            int.TryParse(data[index++], out width);
            this.Width = width;
            if (index < data.Length)
                this.GID = data[index++];
            if (index < data.Length)
                this.FID = data[index++];
            if (index < data.Length)
                int.TryParse(data[index++], out this.ROT);
        }

        protected override string[] relatedUnqiueIds
        {
            get
            {
                if (this.MMSI == 0)
                    return new string[] { this.UniqueId };
                return new string[] { "AIS" + "." + "" + "." + this.MMSI.ToString() };
            }
        }
    }
}
