using SeeCool.GISFramework.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTSCore.Layers.Tracks
{
    [Serializable]
    public class HZGPSData : ShipObj
    {
        public HZGPSData()
        {
            Heading = 511;
        }

        private string _sshq = string.Empty;//所属航区
        public string SSHQ
        {
            set
            {
                _sshq = value;
            }
            get
            {
                return _sshq;
            }
        }

        private bool _isOnline = false;//是否在线
        public bool IsOnline
        {
            set
            {
                _isOnline = value;
            }
            get
            {
                return _isOnline;
            }
        }

        protected override string[] relatedUnqiueIds
        {
            get { return new string[] { this.UniqueId }; }
        }

        public override string Format()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("HZGPS,");
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
            sb.Append(this.SSHQ);
            sb.Append(",");
            sb.Append(this.IsOnline.ToString());
            sb.Append(",");
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
            this.SSHQ = data[index++];
            bool.TryParse(data[index++], out _isOnline);
        }

        public override bool IsTimeout
        {
            get
            {
                //return false;
                TimeSpan ts = DateTime.Now - this.Time;
                return ts.TotalSeconds > 15 * 60;
            }
        }

        public override string Type
        {
            get { return "HZGPSData"; }
        }
    }
}
