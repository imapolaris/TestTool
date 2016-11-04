using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Reflection;
using System.Xml;
using System.Data.SqlTypes;

using SeeCool.GISFramework.Net;
using SeeCool.GISFramework.Object;

namespace SeeCool.GISFramework.SvrFramework
{
    public class HittReceiver : VTSPlugin.TcpipReceiver
    {
        private Parser _parser = null;
        //private Dictionary<string, HittData> _dic = new Dictionary<string, HittData>();

        public HittReceiver()
        {
            //Framework.SubscribeRemoveRealtime("Hitt", onRemoveRealtime);
        }

        protected override void onConnected()
        {
            base.onConnected();
            _parser = new Parser();
        }

        public event Action<HittData> OnReceivedData;

        protected override void onRecv(byte[] pb, int len)
        {
            Target[] msgs = _parser.Parse(pb, 0, len);
            foreach (Target msg in msgs)
            {
                HittData data = new HittData();
                data.Id = msg.Id.ToString();
                data.Src = string.Empty;
                data.SourceId = msg.SourceId;
                data.COG = msg.COG;
                data.SOG = msg.SOG;
                data.Lost = msg.Lost;
                data.RateOfTurn = msg.RateOfTurn;
                data.Orientation = msg.Orientation;
                data.Length = msg.Length;
                data.Breadth = msg.Breadth;
                data.Altitude = msg.Altitude;
                if (msg.NavStatus > 9 && msg.NavStatus < 15)
                    data.NavStatus = HittData.NavStatuses.ReservedForFutureUse;
                else
                    data.NavStatus = (HittData.NavStatuses)msg.NavStatus;
                data.UpdSensorType = (HittData.UpdSensor)msg.UpdSensorType;
                data.ATONOffPos = msg.ATONOffPos;
                data.Shape = new GeoPointShape(msg.Long, msg.Lat);
                data.StaticId = msg.StaticId;
                data.SourceName = msg.SourceName;
                data.Source = (HittData.SourceTypes)msg.Source;
                data.Callsign = msg.Callsign;
                data.ShipName = msg.ShipName;
                data.ObjectType = (HittData.ObjectTypes)msg.ObjectType;
                data.ShipType = (HittData.ShipTypes)msg.ShipType;
                data.MMSI = msg.MMSI;
                data.IMO = msg.IMO;
                data.ATONType = (HittData.ATONTypes)msg.ATONType;
                data.ATONName = msg.ATONName;
                data.AntPosDistFromFront = msg.AntPosDistFromFront;
                data.AntPosDistFromLeft = msg.AntPosDistFromFront;
                data.NatLangShipName = msg.NatLangShipName;
                data.PortOfRegistry = msg.PortOfRegistry;
                data.CountryFlag = msg.CountryFlag;
                data.MaxAirDraught = msg.MaxAirDraught;
                data.MaxDraught = msg.MaxDraught;
                data.DeepWaterVesselind = msg.DeepWaterVesselind;
                data.VoyageId = msg.VoyageId;
                data.CargoType = (HittData.CargoTypes)msg.CargoType;
                data.Destination = msg.Destination;
                data.ETA = msg.ETA;
                data.ATA = msg.ATA;
                data.AirDraught = msg.AirDraught;
                data.Draught = msg.Draught;
                data.Time = DateTime.Now;
                if (data.MMSI != 0)
                    data.Id = "MMSI:" + data.MMSI.ToString();


                if (OnReceivedData != null)
                    OnReceivedData(data);

                //Framework.DistributeData(this, data);
                //DynamicObjMgr.Realtime.HandleData(data);
                //Framework.BroadcastByRegion(new NetCmd(FrameworkNetCmdType.SC_Realtime_String, data.Format()), data.Shape);
                //StorageService service = Framework.GetService("Storage") as StorageService;
                //if (service != null)
                //{
                //    service.SaveRealtime(data);
                //    service.SaveHistory(data);
                //}
            }
        }

        public class Target
        {
            // Pos Report
            // The identification of this data
            public int Id;
            // The identification of the node who initially created this message
            public int SourceId;
            // Date and time in ISO 8601 UTC format this position was measured.
            public DateTime UpdateTime = DateTime.MinValue;
            // Speed over ground in meters per second
            public double SOG;
            // Course over ground  in degrees. (0-360)
            public double COG = 361;
            // 'yes' or 'no'
            public bool Lost;
            // Rate of turn in degrees per minute
            public int RateOfTurn = -128;
            // Orientation of the target in degrees
            public double Orientation = 361;
            // Length of the target in meter
            public double Length;
            // Breadth of the target in meter
            public double Breadth;
            // The altitude of the target above the WGS-84 ellipsoid in meters
            public double Altitude;
            /* Navigation status of the target
             * 0 = under way using engine
             * 
             * 1 = at anchor
             * 2 = not under command
             * 3 = restricted manoeuvrability
             * 4 = constrained by her draught
             * 5 = moored
             * 6 = aground
             * 7 = engaged in fishing
             * 8 = under way sailing
             * 9 - 14 = reserved for future use
             * 15 = undefined default</xs:documentation>
             * */
            public int NavStatus = 15;
            /* Type of detection or data type:
             * 1 = radar
             * 2 = ais
             * 3 = ais+radar
             * 4 = deadreckoning
             * */
            public int UpdSensorType;
            // "1" or "0". Indicates whether or not the ATON is on position or not
            public bool ATONOffPos;
            // Latitude (WGS84) in degrees. (+/- 90 degrees; North = positive; South = negative).
            public double Lat = -91;
            // Longitude (WGS84) in degrees. (+/- 180 degrees; East = positive; West = negative).
            public double Long = -181;

            // Static Data
            // The identification of this static data
            public string StaticId = "";
            // Identification of the originator of the data
            public string SourceName = "";
            // Source/originator type: 1 = transponder 2 = database 3 = manual
            public int Source;
            // Callsign of the target
            public string Callsign = "";
            // Name of the target
            public string ShipName = "";
            /* 1 = Aircraft
             * 2 = Vessel
             * 3 = Vehicle (not an aircraft or vessel)
             * 4 = BaseStation
             * 5 = Aids to Navigate
             * 6 = Virtual Aids to Navigate
             * 7 = Field Transponder
             * */
            public int ObjectType;
            /* 20 = WIG
             * 30 = fishing vessel
             * 31 = towing vessel
             * 32 = big towing vessel
             * 33 = dredging vessel
             * 34 = diving vessel
             * 35 = military vessel
             * 36 = sailing vessel
             * 37 = pleasure craft
             * 40 = HSC
             * 50 = pilot vissel
             * 51 = SAR
             * 52 = tug
             * 53 = port tender
             * 54 = anti pollution vessel
             * 55 = law enforcementvessel
             * 58 = medical vessel
             * 59 = mob83 vessel
             * 60 = passenger ship
             * 70 = cargo ship
             * 80 = tanker
             * 90 = other types of ship
             * */
            public int ShipType;
            // IMO number of the target
            public int IMO;
            // MMSI number of the target
            public int MMSI;
            /* 0 = Unknown
             * 1 = Unknown fixed
             * 2 = Unknown floating
             * 3 = Fixed off shore structure
             * 5 = Light, without sectors
             * 6 = Light, with sectors
             * 7 = Leading Light Front
             * 8 = Leading Light Rear
             * 9 = Beacon, Cardinal N
             * 10 = Beacon, Cardinal E
             * 11 = Beacon, Cardinal S
             * 12 = Beacon, Cardinal W
             * 13 = Beacon, Port hand
             * 14 = Beacon, Starboard hand
             * 15 = Beacon, Preferred Channel port hand
             * 16 = Beacon, Preferred Channel starboard hand
             * 17 = Beacon, Isolated danger
             * 18 = Beacon, Safe water
             * 19 = Beacon, Special mark
             * 20 = Cardinal Mark N
             * 21 = Cardinal Mark E
             * 22 = Cardinal Mark S
             * 23 = Cardinal Mark W
             * 24 = Port hand Mark
             * 25 = Starboard hand Mark
             * 26 = Preferred Channel Port hand
             * 27 = Preferred Channel Starboard hand
             * 28 = Isolated danger
             * 29 = Safe Water
             * 30 = Special Mark
             * 31 = Light Vessel / LANBY/Rigs
             * 32 = Reference point
             * 33 = RACON
             * */
            public int ATONType;
            // Name of Aids-to-navigation
            public string ATONName;
            // GPS Antenna position distance from front in meters
            public int AntPosDistFromFront;
            // GPS Antenna position distance from left in meters
            public int AntPosDistFromLeft;
            // The radarName of the vessel in native language
            public string NatLangShipName = "";
            // Port Of Registry
            public string PortOfRegistry = "";
            // The country flag
            public string CountryFlag = "";
            // Maximum air draught of the vessel in meters
            public double MaxAirDraught;
            // Maximum draught of the vessel in meters
            public double MaxDraught;
            // "yes" or "no"
            public bool DeepWaterVesselind;

            // Voyage
            // The identification of this voyage
            public string VoyageId = "";
            /* 0 = All ships of this type
             * 1 = Carrying DG, HS, or MP, IMO hazard or pollutant category A
             * 2 = Carrying DG, HS, or MP, IMO hazard or pollutant category B
             * 3 = Carrying DG, HS, or MP, IMO hazard or pollutant category C
             * 4 = Carrying DG, HS, or MP, IMO hazard or pollutant category D
             * 9 = No additional information
             * */
            public int CargoType;
            // Destination of the target
            public string Destination = "";
            // Date and time in ISO 8601 UTC format of the Expected Time Of Arrival of the target.
            public DateTime ETA = DateTime.MinValue;
            // Date and time in ISO 8601 UTC format of the Actual Time Of Arrival of the target.
            public DateTime ATA = DateTime.MinValue;
            // Actual air draught of the vessel in meters
            public double AirDraught;
            // Actual draught of the vessel in meters
            public double Draught;

            class BinaryTarget
            {
                Target _target = null;
                ulong _mask = 0;
                ulong _maskbit = 1;
                MemoryStream _stream;
                BinaryWriter _writer;
                BinaryReader _reader;
                static Target _nullTarget = new Target();

                public BinaryTarget(Target target)
                {
                    _target = target;
                    _stream = new MemoryStream();
                    _writer = new BinaryWriter(_stream);
                    _writer.Write((int)0);
                    _writer.Write(target.Id);
                    _writer.Write(_mask);

                    _mask = 0;
                    _maskbit = 1;
                    FieldInfo[] fields = typeof(Target).GetFields();
                    foreach (FieldInfo field in fields)
                        if (field.Name != "Id")
                            WriteItem(field);

                    _writer.Seek(0, SeekOrigin.Begin);
                    _writer.Write((int)_stream.Length);
                    _writer.Seek(sizeof(int), SeekOrigin.Current);
                    _writer.Write(_mask);
                }

                void WriteItem(FieldInfo field)
                {
                    object value = field.GetValue(_target);
                    if (!value.Equals(field.GetValue(_nullTarget)))
                    {
                        _mask |= _maskbit;
                        Type type = value.GetType();
                        if (type == typeof(int))
                            _writer.Write((int)value);
                        else if (type == typeof(uint))
                            _writer.Write((uint)value);
                        else if (type == typeof(double))
                            _writer.Write((double)value);
                        else if (type == typeof(bool))
                            _writer.Write((bool)value);
                        else if (type == typeof(string))
                            _writer.Write((string)value);
                        else if (type == typeof(DateTime))
                            _writer.Write(((DateTime)value).ToBinary());
                    }
                    _maskbit <<= 1;
                }

                public byte[] ToArray()
                {
                    return _stream.ToArray();
                }

                bool readStream()
                {
                    _reader = new BinaryReader(_stream);
                    _target = new Target();
                    try
                    {
                        int length = _reader.ReadInt32();
                        if (length == _stream.Length)
                        {
                            _target.Id = _reader.ReadInt32();
                            _mask = _reader.ReadUInt64();
                            _maskbit = 1;
                            FieldInfo[] fields = typeof(Target).GetFields();
                            foreach (FieldInfo field in fields)
                                if (field.Name != "Id")
                                    ReadItem(field);
                        }
                    }
                    catch
                    {
                        return false;
                    }

                    return true;
                }

                public BinaryTarget(byte[] bytes)
                {
                    _stream = new MemoryStream(bytes);
                    readStream();
                }

                public BinaryTarget(byte[] bytes, int start, int len)
                {
                    byte[] buf = new byte[len];
                    Array.Copy(bytes, start, buf, 0, len);
                    _stream = new MemoryStream(buf);
                    readStream();
                }

                void ReadItem(FieldInfo field)
                {
                    if ((_mask & _maskbit) != 0)
                    {
                        Type type = field.FieldType;
                        if (type == typeof(int))
                            field.SetValue(_target, _reader.ReadInt32());
                        else if (type == typeof(uint))
                            field.SetValue(_target, _reader.ReadUInt32());
                        else if (type == typeof(double))
                            field.SetValue(_target, _reader.ReadDouble());
                        else if (type == typeof(bool))
                            field.SetValue(_target, _reader.ReadBoolean());
                        else if (type == typeof(string))
                            field.SetValue(_target, _reader.ReadString());
                        else if (type == typeof(DateTime))
                            field.SetValue(_target, DateTime.FromBinary(_reader.ReadInt64()));
                    }
                    _maskbit <<= 1;
                }

                public Target ToTarget()
                {
                    return _target;
                }
            }

            public byte[] ToBinary()
            {
                BinaryTarget bt = new BinaryTarget(this);
                return bt.ToArray();
            }

            public static Target FromBinary(byte[] bytes)
            {
                BinaryTarget bt = new BinaryTarget(bytes);
                return bt.ToTarget();
            }

            public static Target FromBinary(byte[] bytes, int start, int len)
            {
                BinaryTarget bt = new BinaryTarget(bytes, start, len);
                return bt.ToTarget();
            }
        }

        public class Parser
        {
            protected byte[] _buf = new byte[0];

            protected void AppendData(byte[] data, int start, int len)
            {
                int size = _buf.Length + len;
                byte[] temp = new byte[size];
                Array.Copy(_buf, temp, _buf.Length);
                Array.Copy(data, start, temp, _buf.Length, len);
                _buf = temp;
            }

            protected void DeleteHead(int len)
            {
                len = Math.Min(len, _buf.Length);
                int size = _buf.Length - len;
                byte[] temp = new byte[size];
                Array.Copy(_buf, len, temp, 0, size);
                _buf = temp;
            }

            public Target[] Parse(byte[] data, int offset, int size)
            {
                AppendData(data, offset, size);
                List<Target> targets = new List<Target>();
                while (_buf.Length >= sizeof(int))
                {
                    int length = BitConverter.ToInt32(_buf, 0);
                    if (_buf.Length >= length)
                    {
                        Target target = Target.FromBinary(_buf, 0, length);
                        DeleteHead(length);
                        if (target != null)
                            targets.Add(target);
                    }
                    else
                        break;
                }
                return targets.ToArray();
            }
        }

        public class XmlParser
        {
            protected string _buf = "";
            protected string _msg = "";

            public Target[] Parse(byte[] data, int offset, int size)
            {
                List<Target> targets = new List<Target>();
                string str = Encoding.UTF8.GetString(data, offset, size);
                _buf += str;
                string line;
                while (readLine(out line))
                {
                    string trim = line.Trim();
                    string left = trim.Substring(0, Math.Min(12, trim.Length));
                    int len = Math.Min(13, trim.Length);
                    string right = trim.Substring(trim.Length - len, len);
                    if (left == "<VesselData>")
                        _msg = line;
                    else if (_msg.Length > 0)
                        _msg += line;
                    if (right == "</VesselData>")
                    {
                        Target target = parseXml(_msg);
                        _msg = "";
                        if (target != null)
                            targets.Add(target);
                    }
                }
                return targets.ToArray();
            }

            bool readLine(out string line)
            {
                line = "";
                if (_buf.Length > 0)
                {
                    int index = _buf.IndexOf('\n');
                    if (index >= 0)
                    {
                        line = _buf.Substring(0, index + 1);
                        _buf = _buf.Substring(index + 1);
                        return true;
                    }
                }

                return false;
            }

            Target parseXml(string xml)
            {
                XmlDocument dom = new XmlDocument();
                try
                {
                    dom.LoadXml(xml);
                }
                catch
                {
                    //Console.WriteLine(ex.ToString());
                    return null;
                }

                Target target = new Target();
                XmlNode posReport = dom.SelectSingleNode("./VesselData/PosReport");
                XmlNode pos = dom.SelectSingleNode("./VesselData/PosReport/Pos");
                XmlNode staticData = dom.SelectSingleNode("./VesselData/StaticData");
                XmlNode voyage = dom.SelectSingleNode("./VesselData/Voyage");

                int.TryParse(getAttr(posReport, "Id"), out target.Id);
                int.TryParse(getAttr(posReport, "SourceId"), out target.SourceId);
                DateTime.TryParse(getAttr(posReport, "UpdateTime"), out target.UpdateTime);
                double.TryParse(getAttr(posReport, "SOG"), out target.SOG);
                double.TryParse(getAttr(posReport, "COG"), out target.COG);
                yesNoParse(getAttr(posReport, "Lost"), out target.Lost);
                int.TryParse(getAttr(posReport, "RateOfTurn"), out target.RateOfTurn);
                double.TryParse(getAttr(posReport, "Orientation"), out target.Orientation);
                if (!double.TryParse(getAttr(posReport, "Length"), out target.Length))
                    double.TryParse(getAttr(staticData, "Length"), out target.Length);
                if (!double.TryParse(getAttr(posReport, "Breadth"), out target.Breadth))
                    double.TryParse(getAttr(staticData, "Breadth"), out target.Breadth);
                double.TryParse(getAttr(posReport, "Altitude"), out target.Altitude);
                int.TryParse(getAttr(posReport, "NavStatus"), out target.NavStatus);
                int.TryParse(getAttr(posReport, "UpdSensorType"), out target.UpdSensorType);
                bool.TryParse(getAttr(posReport, "ATONOffPos"), out target.ATONOffPos);

                double.TryParse(getAttr(pos, "Lat"), out target.Lat);
                double.TryParse(getAttr(pos, "Long"), out target.Long);

                target.StaticId = getAttr(staticData, "Id");
                target.SourceName = getAttr(staticData, "SourceName");
                if (target.SourceName == "")
                    target.SourceName = getAttr(voyage, "SourceName");
                if (!int.TryParse(getAttr(staticData, "Source"), out target.Source))
                    int.TryParse(getAttr(voyage, "Source"), out target.Source);
                target.Callsign = getAttr(staticData, "Callsign");
                target.ShipName = getAttr(staticData, "ShipName");
                int.TryParse(getAttr(staticData, "ObjectType"), out target.ObjectType);
                int.TryParse(getAttr(staticData, "ShipType"), out target.ShipType);
                int.TryParse(getAttr(staticData, "IMO"), out target.IMO);
                int.TryParse(getAttr(staticData, "MMSI"), out target.MMSI);
                int.TryParse(getAttr(staticData, "ATONType"), out target.ATONType);
                target.ATONName = getAttr(staticData, "ATONName");
                int.TryParse(getAttr(staticData, "AntPosDistFromFront"), out target.AntPosDistFromFront);
                int.TryParse(getAttr(staticData, "AntPosDistFromLeft"), out target.AntPosDistFromLeft);
                target.NatLangShipName = getAttr(staticData, "NatLangShipName");
                target.PortOfRegistry = getAttr(staticData, "PortOfRegistry");
                target.CountryFlag = getAttr(staticData, "CountryFlag");
                double.TryParse(getAttr(staticData, "MaxAirDraught"), out target.MaxAirDraught);
                double.TryParse(getAttr(staticData, "MaxDraught"), out target.MaxDraught);
                yesNoParse(getAttr(staticData, "DeepWaterVesselind"), out target.DeepWaterVesselind);

                target.VoyageId = getAttr(voyage, "Id");
                int.TryParse(getAttr(voyage, "CargoType"), out target.CargoType);
                target.Destination = getAttr(voyage, "Destination");
                DateTime.TryParse(getAttr(voyage, "ETA"), out target.ETA);
                DateTime.TryParse(getAttr(voyage, "ATA"), out target.ATA);
                double.TryParse(getAttr(voyage, "AirDraught"), out target.AirDraught);
                double.TryParse(getAttr(voyage, "Draught"), out target.Draught);

                return target;
            }

            string getAttr(XmlNode node, string attrName)
            {
                if (node != null)
                {
                    XmlNode attr = node.Attributes.GetNamedItem(attrName);
                    if (attr != null)
                        return attr.Value;
                }

                return "";
            }

            bool yesNoParse(string str, out bool b)
            {
                str = str.ToLower();
                if (str == "yes")
                {
                    b = true;
                    return true;
                }
                else if (str == "no")
                {
                    b = false;
                    return true;
                }

                b = false;
                return false;
            }
        }

    }

}
