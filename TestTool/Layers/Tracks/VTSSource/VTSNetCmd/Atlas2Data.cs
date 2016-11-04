using System;
using System.Collections.Generic;
using System.Text;

namespace SeeCool.GISFramework.Object
{
    public class Atlas2TimeoutUtil
    {
        public const int TotalSeconds = 180; 
    }


    [Serializable]
    public class Atlas2Data : ShipObj
    {
        public Atlas2Data()
        {
            Heading = 511;
        }

        public enum ObjTypes
        {
            Vessel = 0,
            Buoy,
            Station,
        }
        public int TrackId;
        public int Status;
        public string ShortName;
        public string CallSign;
        public int ShipType;
        public int Length;
        public int Width;
        //public double TimeLastUpdate;
        public DateTime TimeLastUpdate;
        public ObjTypes ObjType;
        public int TargetType;
        public int PilotStatus;
        public int Rating;
        public int LabelColorIndex;
        public int Category;
        public int TransponderState;
        public int DBTrackId;
        public double Draught;
        public string InfoTxt;
        public double ETA;
        public double ETAEndPoint;
        public double RateOfTurn;//??????
        public int AISNavStatus;
        public int AISRateOfTurn;
        public int AISSOG;
        public int AISLatitude;
        public int AISLongitude;
        public int AISCOG;
        public int AISTrueHeading
        {
            get { return Heading; }
            set { Heading = value; }
        }
        public int AISETA;
        public double AISDraught;
        public double AISTimeOfLastUpdate;
        public DateTime DataTime;

        protected override string[] relatedUnqiueIds
        {
            get
            {
                if (this.MMSI == 0)
                    return new string[] { this.UniqueId };
                return new string[] { "AIS" + "." + "" + "." + this.MMSI };
            }
        }

        public override string Format()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("ATLAS2,");
            sb.Append(DataTime.ToString());
            sb.Append(",");
            sb.Append(TrackId);
            sb.Append(",");
            sb.Append(Status);
            sb.Append(",");
            //sb.Append(Name);
            sb.Append(EncodeStr.Encode(Name));
            sb.Append(",");
            sb.Append(ShortName);
            sb.Append(",");
            //5
            sb.Append(CallSign);
            sb.Append(",");
            sb.Append(ShipType);
            sb.Append(",");
            sb.Append(Length);
            sb.Append(",");
            sb.Append(Width);
            sb.Append(",");
            sb.Append(Lon.ToString("F6"));
            sb.Append(",");
            //10
            sb.Append(Lat.ToString("F6"));
            sb.Append(",");
            sb.Append(COG.ToString("F1"));
            sb.Append(",");
            sb.Append(SOG.ToString("F1"));
            sb.Append(",");
            sb.Append(Heading);
            sb.Append(",");
            sb.Append(TimeLastUpdate.ToString());
            sb.Append(",");
            //15
            sb.Append(ObjType);
            sb.Append(",");
            sb.Append(TargetType);
            sb.Append(",");
            sb.Append(PilotStatus);
            sb.Append(",");
            sb.Append(Rating);
            sb.Append(",");
            sb.Append(LabelColorIndex);
            sb.Append(",");
            //20
            sb.Append(Category);
            sb.Append(",");
            sb.Append(MMSI);
            sb.Append(",");
            sb.Append(TransponderState);
            sb.Append(",");
            sb.Append(DBTrackId);
            sb.Append(",");
            sb.Append(Draught);
            sb.Append(",");
            //25
            //sb.Append(InfoTxt);
            sb.Append(EncodeStr.Encode(InfoTxt));
            sb.Append(",");
            sb.Append(ETA);
            sb.Append(",");
            sb.Append(ETAEndPoint);
            sb.Append(",");
            sb.Append(RateOfTurn);
            sb.Append(",");
            sb.Append(AISNavStatus);
            sb.Append(",");
            //30
            sb.Append(AISRateOfTurn);
            sb.Append(",");
            sb.Append(AISSOG);
            sb.Append(",");
            sb.Append(AISLatitude);
            sb.Append(",");
            sb.Append(AISLongitude);
            sb.Append(",");
            sb.Append(AISCOG);
            sb.Append(",");
            //35
            sb.Append(AISTrueHeading);
            sb.Append(",");
            sb.Append(AISETA);
            sb.Append(",");
            sb.Append(AISDraught);
            sb.Append(",");
            sb.Append(AISTimeOfLastUpdate);
            sb.Append(",");
            sb.Append(Time.ToString());
            sb.Append(",");
            //40
            sb.Append(GID);
            sb.Append(",");
            sb.Append(FID);
            sb.Append(",");
            string result = sb.ToString();
            return result;
        }

        public override bool IsTimeout
        {
            get
            {
                TimeSpan ts = DateTime.Now - this.Time;
                if (SOG > 1)
                {
                    return ts.TotalSeconds > 3 * 3;
                }
                else
                {
                    return ts.TotalSeconds > Atlas2TimeoutUtil.TotalSeconds;
                }
            }
        }

        public override void Parse(string[] data)
        {
            int index = 1;
            DataTime = DateTime.Parse(data[index++]);
            int.TryParse(data[index++], out TrackId);
            int.TryParse(data[index++], out Status);
            Name = EncodeStr.Decode(data[index++]);
            ShortName = data[index++];
            //5
            CallSign = data[index++];
            int.TryParse(data[index++], out ShipType);
            int.TryParse(data[index++], out Length);
            int.TryParse(data[index++], out Width);
            double x = 0;
            double.TryParse(data[index++], out x);
            //10
            double y = 0;
            double.TryParse(data[index++], out y);
            Shape = new GeoPointShape(x, y);
            double.TryParse(data[index++], out COG);
            double.TryParse(data[index++], out SOG);
            int.TryParse(data[index++], out Heading);
            TimeLastUpdate = DateTime.Parse(data[index++]);
            //15
            ObjType = (ObjTypes)Enum.Parse(typeof(ObjTypes), data[index++]);
            int.TryParse(data[index++], out TargetType);
            int.TryParse(data[index++], out PilotStatus);
            int.TryParse(data[index++], out Rating);
            int.TryParse(data[index++], out LabelColorIndex);
            //20
            int.TryParse(data[index++], out Category);
            int.TryParse(data[index++], out MMSI);
            int.TryParse(data[index++], out TransponderState);
            int.TryParse(data[index++], out DBTrackId);
            double.TryParse(data[index++], out Draught);
            //25
            InfoTxt = EncodeStr.Decode(data[index++]);
            double.TryParse(data[index++], out ETA);
            double.TryParse(data[index++], out ETAEndPoint);
            double.TryParse(data[index++], out RateOfTurn);
            int.TryParse(data[index++], out AISNavStatus);
            //30
            int.TryParse(data[index++], out AISRateOfTurn);
            int.TryParse(data[index++], out AISSOG);
            int.TryParse(data[index++], out AISLatitude);
            int.TryParse(data[index++], out AISLongitude);
            int.TryParse(data[index++], out AISCOG);
            //35
            int heading = 511;
            if (int.TryParse(data[index++], out heading))
                AISTrueHeading = heading;
            int.TryParse(data[index++], out AISETA);
            double.TryParse(data[index++], out AISDraught);
            double.TryParse(data[index++], out AISTimeOfLastUpdate);
            Time = DateTime.Parse(data[index++]);
            //40
            if (index < data.Length)
                this.GID = data[index++];
            if (index < data.Length)
                this.FID = data[index++];

            Id = GetID();
        }

        public string GetID()
        {
            if (MMSI != 0)
                return "MMSI:" + MMSI.ToString();
            else if (!string.IsNullOrEmpty(Name))
                return "Name:" + Name;
            else
                return "ID:" + TrackId.ToString();
        }

        public override string Type
        {
            get { return "ATLAS2"; }
        }
    }
}
