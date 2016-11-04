using System;
using System.Collections.Generic;
using System.Text;

namespace SeeCool.GISFramework.Object
{
    [Serializable]
    public class WXAISTele19 : DynamicLogicalObj
    {
        public string Name;
        public int ShipCargoType;
        public double COG;
        public bool DTE;
        public int EPFD_Type;
        public double Latitude;
        public double Longitude;
        public int Measure_A;
        public int Measure_B;
        public int Measure_C;
        public int Measure_D;
        public bool PositionAccuracy;
        public bool RAIM_Flag;
        public double SOG;
        public int TimeStamp;
        public int TrueHeading;

        public override string Type
        {
            get { return "WXAISTELE19"; }
        }

        public override string Format()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("WXAIS,19,");
            sb.Append(this.Id);
            sb.Append(",");
            sb.Append(this.Time.ToString());
            sb.Append(",");
            sb.Append(this.Name);
            sb.Append(",");
            sb.Append(this.ShipCargoType.ToString());
            sb.Append(",");
            sb.Append(this.COG.ToString("F1"));
            sb.Append(",");
            sb.Append(this.DTE.ToString());
            sb.Append(",");
            sb.Append(this.EPFD_Type.ToString());
            sb.Append(",");
            sb.Append(this.Latitude.ToString("F6"));
            sb.Append(",");
            sb.Append(this.Longitude.ToString("F6"));
            sb.Append(",");
            sb.Append(this.Measure_A.ToString());
            sb.Append(",");
            sb.Append(this.Measure_B.ToString());
            sb.Append(",");
            sb.Append(this.Measure_C.ToString());
            sb.Append(",");
            sb.Append(this.Measure_D.ToString());
            sb.Append(",");
            sb.Append(this.PositionAccuracy.ToString());
            sb.Append(",");
            sb.Append(this.RAIM_Flag.ToString());
            sb.Append(",");
            sb.Append(this.SOG.ToString());
            sb.Append(",");
            sb.Append(this.TimeStamp.ToString());
            sb.Append(",");
            sb.Append(this.TrueHeading.ToString());
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
            COG = Double.Parse(data[index++]);
            DTE = Boolean.Parse(data[index++]);
            EPFD_Type = Int32.Parse(data[index++]);
            Latitude = Double.Parse(data[index++]);
            Longitude = Double.Parse(data[index++]);
            Measure_A = Int32.Parse(data[index++]);
            Measure_B = Int32.Parse(data[index++]);
            Measure_C = Int32.Parse(data[index++]);
            Measure_D = Int32.Parse(data[index++]);
            PositionAccuracy = Boolean.Parse(data[index++]);
            RAIM_Flag = Boolean.Parse(data[index++]);
            SOG = Double.Parse(data[index++]);
            TimeStamp = Int32.Parse(data[index++]);
            TrueHeading = Int32.Parse(data[index++]);
        }
    }
}
