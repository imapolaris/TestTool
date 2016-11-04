using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace SeeCool.GISFramework.Object
{
    [Serializable]
    public class SofrelogData : ShipObj
    {
        public enum TrackPlatforms { Surface = 0, Aircraft = 1, Helicopter = 2, IFF_only = 3, Buoy = 4, AIS_only = 5 };
        public enum TrackLocks { NotYet = 0, OnTrack = 1, Lost = 2 };
        public enum IFF_AlertCodes { NoAlert = 0, Alert7600 = 1, Alert7700 = 2, Alert7500 = 4, Alert4X = 6 };

        public int TrackID;
        public int SystemID
        {
            get { return _systemID; }
            set
            {
                _systemID = value;
                updateName();
            }
        }
        private int _systemID;
        public int MainRadar;
        public TrackPlatforms TrackPlatform;
        public int QualityFactor;
        public int LackOfDetection;
        public int PlotCells;
        public TrackLocks TrackLock;
        public int Classification;
        public string CallSign = string.Empty;
        public DateTime TimeStamp = DateTime.MinValue;
        public string VesselName
        {
            get { return _vesselName; }
            set
            {
                _vesselName = value;
                updateName();
            }
        }
        private string _vesselName = string.Empty;

        public SofrelogData()
        {
            Heading = 511;
        }

        public SofrelogData(int systemID)
        {
            this.SystemID = systemID;
        }

        public override string Type
        {
            get { return "SOFRELOG"; }
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
                return ts.TotalSeconds > 3 * 3;
            }
        }

        private void updateName()
        {
            if (!string.IsNullOrEmpty(VesselName))
                Name = VesselName;
            else
                Name = SystemID.ToString();
        }

        public string GetID()
        {
            if (MMSI != 0)
                return "MMSI:" + MMSI.ToString();
            else if (!string.IsNullOrEmpty(VesselName))
                return "Name:" + VesselName;
            else
                return "ID:" + SystemID.ToString();
        }

        public override string Format()
        {
            return string.Format("SOF,{0},{1},{2},{3:F6},{4:F6},{5:F1},{6:F1},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19}",
                SystemID, Time, VesselName.Replace(',', ';'), Lat, Lon, SOG, COG, MMSI, 
                Classification, CallSign.Replace(',', ';'), TrackID, MainRadar, (int)TrackPlatform, 
                QualityFactor, LackOfDetection, PlotCells, (int)TrackLock, TimeStamp, GID, FID);
        }

        public override void Parse(string[] data)
        {
            int index = 1;
            int systemID = 0;
            int.TryParse(data[index++], out systemID);
            this.SystemID = systemID;
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
            int.TryParse(data[index++], out this.Classification);
            this.CallSign = data[index++];
            int.TryParse(data[index++], out this.TrackID);
            int.TryParse(data[index++], out this.MainRadar);
            int trackPlatform = 0;
            int.TryParse(data[index++], out trackPlatform);
            this.TrackPlatform = (TrackPlatforms)trackPlatform;
            int.TryParse(data[index++], out this.QualityFactor);
            int.TryParse(data[index++], out this.LackOfDetection);
            int.TryParse(data[index++], out this.PlotCells);
            int trackLock = 0;
            int.TryParse(data[index++], out trackLock);
            this.TrackLock = (TrackLocks)trackLock;
            DateTime.TryParse(data[index++], out this.TimeStamp);
            if (index < data.Length)
                this.GID = data[index++];
            if (index < data.Length)
                this.FID = data[index++];

            this.Id = GetID();
        }
    }
}
