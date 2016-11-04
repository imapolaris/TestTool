using System;
using System.Collections.Generic;
using System.Text;

namespace SeeCool.GISFramework.Object
{
    [Serializable]
    public class AtlasData : ShipObj
    {
        public AtlasData()
        {
            Heading = 511;
        }

        public AtlasData(int trackID)
        {
            this.TrackID = trackID;
            Heading = 511;
        }

        public int TrackID
        {
            get { return _trackID; }
            set
            {
                _trackID = value;
                if (_trackID >= 100000000)
                    MMSI = _trackID;
                updateName();
            }
        }
        private int _trackID;
        public string VesselName
        {
            get { return _vesselName; }
            set
            {
                _vesselName = value;
                updateName();
            }
        }
        private string _vesselName;
        public string CallSign;
        public int IMO_Number;
        public int TimeStamp;

        public string GetID()
        {
            if (MMSI != 0)
                return "MMSI:" + MMSI.ToString();
            else if (!string.IsNullOrEmpty(VesselName))
                return "Name:" + VesselName;
            else
                return "ID:" + TrackID.ToString();
        }

        private void updateName()
        {
            if (!string.IsNullOrEmpty(VesselName))
                Name = VesselName;
            else
                Name = TrackID.ToString();
        }

        public override string Type
        {
            get { return "ATLAS"; }
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

        public override bool IsTimeout
        {
            get
            {
                TimeSpan ts = DateTime.Now - this.Time;
                if (this.MMSI == 0)
                    return ts.TotalSeconds > 3 * 3;
                else
                {
                    int std = 180;
                    if (this.SOG != AISTele123.INVALID_SOG)
                    {
                        if (this.SOG > 3)
                            std = 10;
                        else if (this.SOG > 14)
                            std = 6;
                        else if (this.SOG > 23)
                            std = 2;
                    }
                    return ts.TotalSeconds > std * 3;
                }
            }
        }

        public override string Format()
        {
            return string.Format("ATL,{0},{1},{2},{3:F6},{4:F6},{5:F1},{6:F1},{7},{8},{9},{10},{11},{12}",
                TrackID, Time, VesselName, Lat, Lon, SOG, COG, MMSI, CallSign, IMO_Number, TimeStamp, GID, FID);
        }

        public override void Parse(string[] data)
        {
            int index = 1;
            int trackID = 0;
            int.TryParse(data[index++], out trackID);
            this.TrackID = trackID;
            this.Time = DateTime.Parse(data[index++]);
            this.VesselName = data[index++];
            double y = 0;
            double.TryParse(data[index++], out y);
            double x = 0;
            double.TryParse(data[index++], out x);
            this.Shape = new GeoPointShape(x, y);
            double.TryParse(data[index++], out this.SOG);
            double.TryParse(data[index++], out this.COG);
            int.TryParse(data[index++], out this.MMSI);
            this.CallSign = data[index++];
            int.TryParse(data[index++], out this.IMO_Number);
            int.TryParse(data[index++], out this.TimeStamp);
            if (index < data.Length)
                this.GID = data[index++];
            if (index < data.Length)
                this.FID = data[index++];

            this.Id = GetID();
        }
    }
}
