using System;
using System.Collections.Generic;
using System.Text;

namespace SeeCool.GISFramework.Object
{
    [Serializable]
    public class WXAISTele5 : DynamicLogicalObj
    {
        public string Name;
        public int ShipCargoType;
        public int AIS_Version;
        public string CallSign;
        public string Destination;
        public bool DTE;
        public int EPFD_Type;
        public DateTime ETA;
        public int IMO_Number;
        public double MaxSeaGauge;
        public int Measure_A;
        public int Measure_B;
        public int Measure_C;
        public int Measure_D;

        public override string Type
        {
            get { return "WXAISTELE5"; }
        }

        public override string Format()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("WXAIS,5,");
            sb.Append(this.Id);
            sb.Append(",");
            sb.Append(this.Time.ToString());
            sb.Append(",");
            sb.Append(this.Name);
            sb.Append(",");
            sb.Append(this.ShipCargoType.ToString());
            sb.Append(",");
            sb.Append(this.AIS_Version.ToString());
            sb.Append(",");
            sb.Append(this.CallSign);
            sb.Append(",");
            sb.Append(this.Destination);
            sb.Append(",");
            sb.Append(this.DTE.ToString());
            sb.Append(",");
            sb.Append(this.EPFD_Type.ToString());
            sb.Append(",");
            sb.Append(this.ETA.ToString());
            sb.Append(",");
            sb.Append(this.IMO_Number.ToString());
            sb.Append(",");
            sb.Append(this.MaxSeaGauge.ToString());
            sb.Append(",");
            sb.Append(this.Measure_A.ToString());
            sb.Append(",");
            sb.Append(this.Measure_B.ToString());
            sb.Append(",");
            sb.Append(this.Measure_C.ToString());
            sb.Append(",");
            sb.Append(this.Measure_D.ToString());
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
            ShipCargoType = Int32.Parse(data[index++]);
            AIS_Version = Int32.Parse(data[index++]);
            CallSign = data[index++];
            Destination = data[index++];
            DTE = Boolean.Parse(data[index++]);
            EPFD_Type = Int32.Parse(data[index++]);
            ETA = DateTime.Parse(data[index++]);
            IMO_Number = Int32.Parse(data[index++]);
            MaxSeaGauge = Double.Parse(data[index++]);
            Measure_A = Int32.Parse(data[index++]);
            Measure_B = Int32.Parse(data[index++]);
            Measure_C = Int32.Parse(data[index++]);
            Measure_D = Int32.Parse(data[index++]);
        }
    }
}
