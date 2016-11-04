using System;
using System.Collections.Generic;
using System.Text;

namespace SeeCool.GISFramework.Object
{
    [Serializable]
    public class HittData : ShipObj
    {
        public enum UpdSensor
        {
            Radar = 1,
            Ais,
            AisAndRadar,
            Deadreckoning
        };
        public enum ObjectTypes
        {
            Aircraft = 1,
            Vessel,
            Vehicle,
            BaseStation,
            AidsToNavigate,
            VirtualAidsToNavigate,
            FieldTransponder,
        };
        public enum ATONTypes
        {
            Unknown = 0,
            UnknownFixed,
            UnknownFloating,
            FixedOffShoreStructure,
            LightWithoutAectors,
            LightWithSectors,
            LeadingLightFront,
            LeadingLightRear,
            BeaconCardinalN,
            BeaconCardinalE,
            BeaconCardinalS,
            BeaconCardinalW,
            BeaconPortHand,
            BeaconStarboardHand,
            BeaconPreferredChannelPortHand,
            BeaconPreferredChannelStarboardHand,
            BeaconIsolatedDanger,
            BeaconSafeWater,
            BeaconSpecialMark,
            CardinalMarkN,
            CardinalMarkE,
            CardinalMarkS,
            CardinalMarkW,
            PortHandMark,
            StarboardHandMark,
            PreferredChannelPortHand,
            PreferredChannelStarboardHand,
            IsolatedDanger,
            SafeWater,
            SpecialMark,
            LightVesselLANBYRigs,
            ReferencePoint,
            RACON
        };
        public enum CargoTypes
        {
            All = 0,
            A = 1,
            B = 2,
            C = 3,
            D = 4,
            Unknown = 9,
        };
        public enum NavStatuses
        {
            UnderWayUsngEngine = 0,
            AtAnchor = 1,
            NotUnderCommand = 2,
            RestrictedManoeuvrability = 3,
            ConstrainedByHerDraught = 4,
            Moored = 5,
            Aground = 6,
            EngagedInFishing = 7,
            UnderWaySailing = 8,
            ReservedForFutureUse = 9,
            UndefinedDefault = 15,
        };
        public enum ShipTypes
        {
            WIG = 20,
            FishingVessel = 30,
            TowingVessel = 31,
            BigTowingVessel = 32,
            DredgingVessel = 33,
            DivingVessel = 34,
            MilitaryVessel = 35,
            SailingVessel = 36,
            PleasureCraft = 37,
            HSC = 40,
            PilotVissel = 50,
            SAR = 51,
            Tug = 52,
            PortTender = 53,
            AntiPollutionVessel = 54,
            LawEnforcementvessel = 55,
            MedicalVessel = 58,
            Mob83Vessel = 59,
            PassengerShip = 60,
            CargoShip = 70,
            Tanker = 80,
            OtherTypesOfShip = 90,
        };
        public enum SourceTypes
        {
            Transponder = 1,
            Database = 2,
            Manual = 3,
        };
        // The identification of the node who initially created this message
        public int SourceId;//雷达信号创建源
        // 'yes' or 'no'
        public bool Lost;//信号是否丢失
        // Rate of turn in degrees per minute
        public int RateOfTurn = -128;//雷达转动率
        // Orientation of the target in degrees
        public double Orientation = 361;//目标相对于雷达的方向
        // Length of the target in meter
        public double Length;//长
        // Breadth of the target in meter
        public double Breadth;//宽
        // The altitude of the target above the WGS-84 ellipsoid in meters
        public double Altitude;//海拔        
        public NavStatuses NavStatus = NavStatuses.UndefinedDefault;//目标状态
        public UpdSensor UpdSensorType;
        // "1" or "0". Indicates whether or not the ATON is on position or not
        public bool ATONOffPos;//航标是否偏移
        // Static Data
        // The identification of this static data
        public string StaticId = "";
        // Identification of the originator of the data
        public string SourceName = "";//雷达名称
        // Source/originator type: 1 = transponder 2 = database 3 = manual
        public SourceTypes Source;//静态数据来源
        // Callsign of the target
        public string Callsign = "";
        // Name of the target
        public string ShipName = "";
        public ObjectTypes ObjectType;
        public ShipTypes ShipType;
        // IMO number of the target
        public int IMO;
        public ATONTypes ATONType;
        // Name of Aids-to-navigation
        public string ATONName;
        // GPS Antenna position distance from front in meters
        public int AntPosDistFromFront;
        // GPS Antenna position distance from left in meters
        public int AntPosDistFromLeft;
        // The radarName of the vessel in native language
        public string NatLangShipName = "";//本地船名
        // Port Of Registry
        public string PortOfRegistry = "";//登记港
        // The country flag
        public string CountryFlag = "";//国旗
        // Maximum air draught of the vessel in meters
        public double MaxAirDraught;//
        // Maximum draught of the vessel in meters
        public double MaxDraught;//最大吃水
        // "yes" or "no"
        public bool DeepWaterVesselind;//是否深水船只
        // Voyage
        // The identification of this voyage
        public string VoyageId = "";//航次
        public CargoTypes CargoType;
        // Destination of the target
        public string Destination = "";//目的港
        // Date and time in ISO 8601 UTC format of the Expected Time Of Arrival of the target.
        public DateTime ETA = DateTime.MinValue;
        // Date and time in ISO 8601 UTC format of the Actual Time Of Arrival of the target.
        public DateTime ATA = DateTime.MinValue;//实际到港时间
        // Actual air draught of the vessel in meters
        public double AirDraught;//船舶实际水上高度
        // Actual draught of the vessel in meters
        public double Draught;//船舶实际吃水

        public HittData()
        {
            Heading = 511;
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

        public override string Format()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("HITT,");
            sb.Append(this.Id);
            sb.Append(",");
            sb.Append(this.SourceId);
            sb.Append(",");
            sb.Append(this.Time.ToString());
            sb.Append(",");
            sb.Append(this.Lon.ToString("F6"));
            sb.Append(",");
            sb.Append(this.Lat.ToString("F6"));
            sb.Append(",");
            sb.Append(this.SOG.ToString("F1"));
            sb.Append(",");
            sb.Append(this.COG.ToString("F1"));
            sb.Append(",");
            sb.Append(this.Lost);
            sb.Append(",");
            sb.Append(this.RateOfTurn);
            sb.Append(",");
            sb.Append(this.Orientation);
            sb.Append(",");
            sb.Append(this.Length);
            sb.Append(",");
            sb.Append(this.Breadth);
            sb.Append(",");
            sb.Append(this.Altitude);
            sb.Append(",");
            sb.Append(this.NavStatus.ToString());
            sb.Append(",");
            sb.Append(this.UpdSensorType.ToString());
            sb.Append(",");
            sb.Append(this.ATONOffPos);
            sb.Append(",");
            sb.Append(this.StaticId);
            sb.Append(",");
            sb.Append(this.SourceName);
            sb.Append(",");
            sb.Append(this.Source);
            sb.Append(",");
            //sb.Append(this.Callsign);
            sb.Append(EncodeStr.Encode(this.Callsign));
            sb.Append(",");
            //sb.Append(this.ShipName);
            sb.Append(EncodeStr.Encode(this.ShipName));
            sb.Append(",");
            sb.Append(this.ObjectType.ToString());
            sb.Append(",");
            sb.Append(this.ShipType.ToString());
            sb.Append(",");
            sb.Append(this.IMO);
            sb.Append(",");
            sb.Append(this.MMSI);
            sb.Append(",");
            sb.Append(this.ATONType.ToString());
            sb.Append(",");
            sb.Append(this.ATONName);
            sb.Append(",");
            sb.Append(this.AntPosDistFromFront);
            sb.Append(",");
            sb.Append(this.AntPosDistFromLeft);
            sb.Append(",");
            sb.Append(this.NatLangShipName);
            sb.Append(",");
            sb.Append(this.PortOfRegistry);
            sb.Append(",");
            sb.Append(this.CountryFlag);
            sb.Append(",");
            sb.Append(this.MaxAirDraught);
            sb.Append(",");
            sb.Append(this.MaxDraught);
            sb.Append(",");
            sb.Append(this.DeepWaterVesselind);
            sb.Append(",");
            sb.Append(this.VoyageId);
            sb.Append(",");
            sb.Append(this.CargoType.ToString());
            sb.Append(",");
            //sb.Append(this.Destination);
            sb.Append(EncodeStr.Encode(this.Destination));
            sb.Append(",");
            sb.Append(this.ETA.ToString());
            sb.Append(",");
            sb.Append(this.ATA.ToString());
            sb.Append(",");
            sb.Append(this.AirDraught);
            sb.Append(",");
            sb.Append(this.Draught);
            sb.Append(",");
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
                return ts.TotalSeconds > 3 * 3;
            }
        }

        public override void Parse(string[] data)
        {
            int index = 1;
            this.Id = data[index++];
            int.TryParse(data[index++], out this.SourceId);
            this.Time = DateTime.Parse(data[index++]);
            double x = 0;
            double.TryParse(data[index++], out x);
            double y = 0;
            double.TryParse(data[index++], out y);
            this.Shape = new GeoPointShape(x, y);
            double.TryParse(data[index++], out this.SOG);
            double.TryParse(data[index++], out this.COG);
            bool.TryParse(data[index++], out this.Lost);
            int.TryParse(data[index++], out this.RateOfTurn);
            double.TryParse(data[index++], out this.Orientation);
            double.TryParse(data[index++], out this.Length);
            double.TryParse(data[index++], out this.Breadth);
            double.TryParse(data[index++], out this.Altitude);
            this.NavStatus = (NavStatuses)Enum.Parse(typeof(NavStatuses), data[index++]);
            this.UpdSensorType = (UpdSensor)Enum.Parse(typeof(UpdSensor), data[index++]);
            bool.TryParse(data[index++], out this.ATONOffPos);
            this.StaticId = data[index++];
            this.SourceName = data[index++];
            this.Source = (SourceTypes)Enum.Parse(typeof(SourceTypes), data[index++]);
            //this.Callsign = data[index++];
            this.Callsign = EncodeStr.Decode(data[index++]);
            //this.ShipName = data[index++];
            this.ShipName = EncodeStr.Decode(data[index++]);
            this.ObjectType = (ObjectTypes)Enum.Parse(typeof(ObjectTypes), data[index++]);
            this.ShipType = (ShipTypes)Enum.Parse(typeof(ShipTypes), data[index++]);
            int.TryParse(data[index++], out this.IMO);
            int.TryParse(data[index++], out this.MMSI);
            this.ATONType = (ATONTypes)Enum.Parse(typeof(ATONTypes), data[index++]);
            this.ATONName = data[index++];
            int.TryParse(data[index++], out this.AntPosDistFromFront);
            int.TryParse(data[index++], out this.AntPosDistFromLeft);
            this.NatLangShipName = data[index++];
            this.PortOfRegistry = data[index++];
            this.CountryFlag = data[index++];
            double.TryParse(data[index++], out this.MaxAirDraught);
            double.TryParse(data[index++], out this.MaxDraught);
            bool.TryParse(data[index++], out this.DeepWaterVesselind);
            this.VoyageId = data[index++];
            this.CargoType = (CargoTypes)Enum.Parse(typeof(CargoTypes), data[index++]);
            //this.Destination = data[index++];
            this.Destination = EncodeStr.Decode(data[index++]);
            DateTime.TryParse(data[index++], out this.ETA);
            DateTime.TryParse(data[index++], out this.ATA);
            double.TryParse(data[index++], out this.AirDraught);
            double.TryParse(data[index++], out this.Draught);
            if (index < data.Length)
                this.GID = data[index++];
            if (index < data.Length)
                this.FID = data[index++];
        }

        public override string Type
        {
            get { return "HITT"; }
        }
    }
}
