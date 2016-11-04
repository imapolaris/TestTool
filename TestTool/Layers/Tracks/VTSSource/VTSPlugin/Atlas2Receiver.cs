using System;
using System.Collections.Generic;
using System.Text;

using SeeCool.GISFramework.Object;
using SeeCool.GISFramework.Net;
using SeeCool.GISFramework.Util;

namespace SeeCool.GISFramework.SvrFramework
{
    public class Atlas2Receiver : VTSPlugin.TcpipReceiver
    {
        private VTSParser _parser = null;
        private Dictionary<int, Atlas2Data> _dic = new Dictionary<int, Atlas2Data>();

        public event Action<Atlas2Data> OnReceivedData;
        public Atlas2Receiver()
        {
            _parser = new VTSParser();
        }

        protected override void onRecv(byte[] pb, int len)
        {
            VTSInfo[] infos = _parser.Parse(pb, 0, len);
            foreach (VTSInfo info in infos)
            {
                Atlas2Data data = new Atlas2Data();
                data.Time = info.RecvTime;
                data.TrackId = info.SystemTrackId;
                data.Status = info.Status;
                data.Name = info.Name;
                data.ShortName = info.ShortName;
                data.CallSign = info.CallSign;
                data.ShipType = info.ShipType;
                data.Length = info.Length;
                data.Width = info.Width;
                data.Shape = new GeoPointShape(info.Longitude, info.Latitude);
                data.COG = info.Course;
                data.SOG = info.Speed;
                data.Heading = (int)info.Heading;
                data.TimeLastUpdate = info.TimeLastUpdate;
                data.ObjType = (Atlas2Data.ObjTypes)info.ObjType;
                data.TargetType = info.TargetType;
                data.PilotStatus = info.PilotStatus;
                data.Rating = info.Rating;
                data.LabelColorIndex = info.LabelColorIndex;
                data.Category = info.Category;
                int.TryParse(info.TransponderId, out data.MMSI);
                data.TransponderState = info.TransponderState;
                data.DBTrackId = info.DBTrackId;
                data.Draught = info.Draught;
                data.InfoTxt = info.InfoTxt;
                data.ETA = info.ETA;
                data.ETAEndPoint = info.ETAEndPoint;
                data.RateOfTurn = info.RateOfTurn;
                data.AISNavStatus = info.AISNavStatus;
                data.AISRateOfTurn = info.AISRateOfTurn;
                data.AISSOG = info.AISSOG;
                data.AISLatitude = info.AISLatitude;
                data.AISLongitude = info.AISLongitude;
                data.AISCOG = info.AISCOG;
                data.AISTrueHeading = info.AISTrueHeading;
                data.AISETA = info.AISETA;
                data.AISDraught = info.AISDraught;
                data.AISTimeOfLastUpdate = info.AISTimeOfLastUpdate;
                data.DataTime = info.DataTime;
                data.Id = data.GetID();

                if (!_dic.ContainsKey(data.TrackId))
                    _dic.Add(data.TrackId, data);
                 
                else  if (data.SOG > 1 || _dic[data.TrackId].Time.AddSeconds(Atlas2TimeoutUtil.TotalSeconds) <= data.Time)
                        _dic[data.TrackId] = data;
                else
                    return;

                if (OnReceivedData != null)
                    OnReceivedData(data);
            }
        }

        public class VTSParser
        {
            private StringBuilder _buffer = new StringBuilder();
            private static DateTime _baseTime = new DateTime(1970, 1, 1);
            public VTSInfo[] Parse(byte[] pb, int offset, int length)
            {
                List<VTSInfo> result = new List<VTSInfo>();
                _buffer.Append(Encoding.Default.GetString(pb, offset, length));
                string[] msgs = parseMsg();
                foreach (string msg in msgs)
                {
                    VTSInfo info = parseInfo(msg);
                    if (info != null)
                        result.Add(info);
                }
                return result.ToArray();
            }

            private string[] parseMsg()
            {
                List<string> result = new List<string>();
                int index = 0;
                while (index < _buffer.Length)
                {
                    int end = index;
                    while (end < _buffer.Length && _buffer[end] != '\n')
                        end++;
                    if (end < _buffer.Length)
                    {
                        result.Add(_buffer.ToString(index, end - index));
                        index = end + 1;
                    }
                    else
                        break;
                }
                _buffer.Remove(0, index);
                return result.ToArray();
            }

            private VTSInfo parseInfo(string msg)
            {
                try
                {
                    string[] strs = msg.Split(new char[] { ',' });
                    if (strs.Length > 38 && strs[0] != "TimeStamp")
                    {
                        VTSInfo info = new VTSInfo();
                        double minSeconds = 0;
                        double.TryParse(strs[0], out minSeconds);
                        info.DataTime = _baseTime.AddMilliseconds(minSeconds).ToLocalTime();
                        int.TryParse(strs[1], out info.SystemTrackId);
                        int.TryParse(strs[2], out info.Status);

                        byte[] pbName = Convert.FromBase64String(strs[3]);
                        info.Name = Encoding.Default.GetString(pbName);

                        info.ShortName = strs[4];
                        info.CallSign = strs[5];
                        int.TryParse(strs[6], out info.ShipType);
                        int.TryParse(strs[7], out info.Length);
                        int.TryParse(strs[8], out info.Width);
                        double lat = 0;
                        double.TryParse(strs[9], out lat);
                        info.Latitude = lat * 180 / Math.PI;
                        double lon = 0;
                        double.TryParse(strs[10], out lon);
                        info.Longitude = lon * 180 / Math.PI;
                        double course = 0;
                        double.TryParse(strs[11], out course);
                        info.Course = course * 180 / Math.PI;
                        if (course < 0)
                            info.Course += 360;
                        double speed = 0;
                        double.TryParse(strs[12], out speed);
                        info.Speed = speed * 3600 / 1852;
                        double heading = 0;
                        double.TryParse(strs[13], out heading);
                        info.Heading = heading * 180 / Math.PI;
                        double timeLastUpdate = 0;
                        double.TryParse(strs[14], out timeLastUpdate);
                        info.TimeLastUpdate = _baseTime.AddSeconds(timeLastUpdate).ToLocalTime();
                        int.TryParse(strs[15], out info.ObjType);
                        int.TryParse(strs[16], out info.TargetType);
                        int.TryParse(strs[17], out info.PilotStatus);
                        int.TryParse(strs[18], out info.Rating);
                        int.TryParse(strs[19], out info.LabelColorIndex);
                        int.TryParse(strs[20], out info.Category);
                        info.TransponderId = strs[21];
                        int.TryParse(strs[22], out info.TransponderState);
                        int.TryParse(strs[23], out info.DBTrackId);
                        double.TryParse(strs[24], out info.Draught);

                        byte[] pbInfoTxt = Convert.FromBase64String(strs[25]);
                        info.InfoTxt = Encoding.Default.GetString(pbInfoTxt);

                        double.TryParse(strs[26], out info.ETA);
                        double.TryParse(strs[27], out info.ETAEndPoint);
                        int rot = 0;
                        int.TryParse(strs[28], out rot);
                        info.RateOfTurn = (rot / 4.733) * (rot / 4.733);
                        int.TryParse(strs[29], out info.AISNavStatus);
                        int.TryParse(strs[30], out info.AISRateOfTurn);
                        int.TryParse(strs[31], out info.AISSOG);
                        int.TryParse(strs[32], out info.AISLatitude);
                        int.TryParse(strs[33], out info.AISLongitude);
                        int.TryParse(strs[34], out info.AISCOG);
                        int.TryParse(strs[35], out info.AISTrueHeading);
                        int.TryParse(strs[36], out info.AISETA);
                        double.TryParse(strs[37], out info.AISDraught);
                        double.TryParse(strs[38], out info.AISTimeOfLastUpdate);
                        info.RecvTime = DateTime.Now;
                        return info;
                    }
                    return null;
                }
                catch (Exception)
                {
                    //TestTool.Common.LogService.Log.Error(err.ToString());//发现接收数据格式不正确，Convert.FromBase64String()时出错
                    return null;
                }
            }
        }

        public class VTSInfo
        {
            public DateTime DataTime;
            public int SystemTrackId;
            public int Status;
            public string Name;
            public string ShortName;
            public string CallSign;
            public int ShipType;
            public int Length;
            public int Width;
            public double Latitude;
            public double Longitude;
            public double Course;
            public double Speed;
            public double Heading;
            //public double TimeLastUpdate;
            public DateTime TimeLastUpdate;
            public int ObjType;//0 -Vessel, 1 - Buoy, 2 - Station
            public int TargetType;
            public int PilotStatus;
            public int Rating;
            public int LabelColorIndex;
            public int Category;
            public string TransponderId;//MMSI
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
            public int AISTrueHeading;
            public int AISETA;
            public double AISDraught;
            public double AISTimeOfLastUpdate;
            public DateTime RecvTime;
        }
    }
}
