using System;
using System.Collections.Generic;
using System.Text;

using USNT.DataParser.AISNMEA;
using SeeCool.GISFramework.Object;
using VTSPlugin;
using SeeCool.GISFramework.SvrFramework;
using DynamicObjContainer;

namespace VTSCore.Layers.Tracks
{
    public class WXAISReceiver : VTSPlugin.TcpipReceiver
    {
        private WXAISParser _parser = null;

        public Action<WXAISTarget> OnDynamicEvent;

        private Dictionary<string, WXAISTele5> _dicCacheAISTele5 = new Dictionary<string, WXAISTele5>();
        private Dictionary<string, WXAISTele19> _dicCacheAISTele19 = new Dictionary<string, WXAISTele19>();

        protected override void onConnected()
        {
            _parser = new WXAISParser();
        }

        protected override void onRecv(byte[] buf, int len)
        {
            Msg[] msgs = _parser.Parse(buf, 0, len);
            HandleMsg(msgs);
        }

        private static string getSrcName() { return String.Empty; }

        private static string trim(string src)
        {
            if (src == null)
                return String.Empty;
            return src.Trim(new char[] { '\0', ' ' }).Replace(",", " ");
        }
        public void HandleMsg(Msg[] msgs)
        {
            foreach (Msg msg in msgs)
            {
                //此处暂时将所有文本中的逗号改成了空格，是为了可以用逗号作为分隔符的ASCII方式表示数据
                if (msg is Tele123)
                {
                    Tele123 tel = msg as Tele123;
                    WXAISTele123 at = new WXAISTele123();
                    //at.Id = tel.MMSI.ToString();
                    //at.MMSI = tel.MMSI;
                    //at.Src = getSrcName();
                    //at.Time = tel.DataTime;
                    //at.Shape = new GeoPointShape(tel.Longitude, tel.Latitude);
                    //at.SOG = tel.SOG;
                    //at.COG = tel.COG;
                    //at.Heading = tel.TrueHeading;
                    //at.NavStatus = tel.NavStatus;
                    //at.CommStateValue = 0;
                    //at.PositionAccuracy = tel.PositionAccuracy;
                    //at.RAIM_Flag = tel.RAIM_Flag;
                    //at.ROT_AIS = tel.ROT_AIS;
                    //at.TimeStamp = tel.TimeStamp;
                    //at.UTCTime = tel.CommState.UTCTime;
                    WXAISTarget target = new WXAISTarget(tel.MMSI);

                    target.Src = getSrcName();
                    target.Lon = tel.Longitude;
                    target.Lat = tel.Latitude;
                    target.SOG = tel.SOG;
                    target.COG = tel.COG;
                    target.Heading = tel.TrueHeading;
                    target.NavStatus = tel.NavStatus;
                    target.PositionAccuracy = tel.PositionAccuracy;
                    target.RAIM_Flag = tel.RAIM_Flag;
                    target.UpdateTime = tel.DataTime;
                    target.TimeStamp = tel.TimeStamp;
                    target.Name = GetNameFromFicCacheAISTele5(target.GetId());

                    fireOnDynamic(target);
                }
                else if (msg is Tele5)
                {
                    Tele5 tel = msg as Tele5;
                    WXAISTele5 at = new WXAISTele5();
                    at.Id = tel.MMSI.ToString();
                    at.Src = getSrcName();
                    at.Time = tel.DataTime;
                    at.Name = trim(tel.Name);
                    at.ShipCargoType = tel.ShipCargoType;
                    at.AIS_Version = tel.AIS_Version;
                    at.CallSign = trim(tel.CallSign);
                    at.Destination = trim(tel.Destination);
                    at.DTE = tel.DTE;
                    at.EPFD_Type = tel.EPFD_Type;
                    at.ETA = tel.ETA;
                    at.IMO_Number = tel.IMO_Number;
                    at.MaxSeaGauge = tel.Draught;
                    at.Measure_A = tel.Measure_A;
                    at.Measure_B = tel.Measure_B;
                    at.Measure_C = tel.Measure_C;
                    at.Measure_D = tel.Measure_D;

                    updateDicCacheAISTele5(at);
                }
                else if (msg is Tele18)
                {
                    Tele18 tel = msg as Tele18;
                    //WXAISTele18 at = new WXAISTele18();
                    //at.Id = tel.MMSI.ToString();
                    //at.MMSI = tel.MMSI;
                    //at.Src = getSrcName();
                    //at.Time = tel.DataTime;
                    //at.Shape = new GeoPointShape(tel.Longitude, tel.Latitude);
                    //at.SOG = tel.SOG;
                    //at.COG = tel.COG;
                    //at.Heading = tel.TrueHeading;
                    //at.CommState = 0;
                    //at.PositionAccuracy = tel.PositionAccuracy;
                    //at.RAIM_Flag = tel.RAIM_Flag;
                    //at.TimeStamp = tel.TimeStamp;

                    
                    //WXAISTele19 sat = DynamicObjContainerHelper.GetObject("WXAISTELE19", at.Id) as WXAISTele19;
                    //if (sat != null)
                    //    at.Name = sat.Name;

                    WXAISTarget target = new WXAISTarget(tel.MMSI);

                    target.Src = getSrcName();
                    target.Lon = tel.Longitude;
                    target.Lat = tel.Latitude;
                    target.SOG = tel.SOG;
                    target.COG = tel.COG;
                    target.Heading = tel.TrueHeading;
                    target.NavStatus = 0;
                    target.PositionAccuracy = tel.PositionAccuracy;
                    target.RAIM_Flag = tel.RAIM_Flag;
                    target.UpdateTime = tel.DataTime;
                    target.TimeStamp = tel.TimeStamp;
                    target.Name = GetNameFromFicCacheAISTele19(target.GetId());

                    fireOnDynamic(target);

                }
                else if (msg is Tele19)
                {
                    Tele19 tel = msg as Tele19;
                    WXAISTele19 at = new WXAISTele19();
                    at.Id = tel.MMSI.ToString();
                    at.Src = getSrcName();
                    at.Time = tel.DataTime;
                    at.Name = trim(tel.Name);
                    at.ShipCargoType = tel.ShipCargoType;
                    at.COG = tel.COG;
                    at.DTE = tel.DTE;
                    at.EPFD_Type = tel.EPFD_Type;
                    at.Latitude = tel.Latitude;
                    at.Longitude = tel.Longitude;
                    at.Measure_A = tel.Measure_A;
                    at.Measure_B = tel.Measure_B;
                    at.Measure_C = tel.Measure_C;
                    at.Measure_D = tel.Measure_D;
                    at.PositionAccuracy = tel.PositionAccuracy;
                    at.RAIM_Flag = tel.RAIM_Flag;
                    at.SOG = tel.SOG;
                    at.TimeStamp = tel.TimeStamp;
                    at.TrueHeading = tel.TrueHeading;

                    updateDicCacheAISTele19(at);
                }
                else if (msg is Tele21)
                {
                    Tele21 tel = msg as Tele21;
                    WXAISTele21 at = new WXAISTele21();
                    at.Id = tel.MMSI.ToString();
                    at.Src = getSrcName();
                    at.Time = tel.DataTime;
                    at.Shape = new GeoPointShape(tel.Longitude, tel.Latitude);
                    at.Name = trim(tel.Name);
                    at.AtoNType = tel.Type;
                    at.EPFD_Type = tel.EPFD_Type;
                    at.Measure_A = tel.Measure_A;
                    at.Measure_B = tel.Measure_B;
                    at.Measure_C = tel.Measure_C;
                    at.Measure_D = tel.Measure_D;
                    at.OffPosition = tel.OffPosition;
                    at.PositionAccuracy = tel.PositionAccuracy;
                    at.RAIM_Flag = tel.RAIM_Flag;
                    at.TimeStamp = tel.TimeStamp;

                    WXAISTarget target = new WXAISTarget(tel.MMSI);

                    target.Name = trim(tel.Name);
                    target.Src = getSrcName();
                    target.Lon = tel.Longitude;
                    target.Lat = tel.Latitude;
                    target.AtoNType = tel.Type;
                    target.NavStatus = 0;
                    target.PositionAccuracy = tel.PositionAccuracy;
                    target.RAIM_Flag = tel.RAIM_Flag;
                    target.UpdateTime = tel.DataTime;
                    target.TimeStamp = tel.TimeStamp;

                    fireOnDynamic(target);


                }
                else if (msg is Tele4)
                {
                    Tele4 tel = msg as Tele4;
                    WXAISTele4 at = new WXAISTele4();
                    at.Id = tel.MMSI.ToString();
                    at.Src = getSrcName();
                    at.Time = tel.DataTime;
                    at.Shape = new GeoPointShape(tel.Longitude, tel.Latitude);
                    at.Name = tel.MMSI.ToString();
                    at.UTC = tel.UTC;

                    WXAISTarget target = new WXAISTarget(tel.MMSI);

                    target.Name = tel.MMSI.ToString();
                    target.Src = getSrcName();
                    target.Lon = tel.Longitude;
                    target.Lat = tel.Latitude;
                    target.UpdateTime = tel.DataTime;
                    fireOnDynamic(target);
                }
            }
        }


        private void updateDicCacheAISTele5(WXAISTele5 obj)
        {
            if (_dicCacheAISTele5.ContainsKey(obj.Id))
                _dicCacheAISTele5[obj.Id] = obj;
            else
                _dicCacheAISTele5.Add(obj.Id, obj);
        }

        private void updateDicCacheAISTele19(WXAISTele19 obj)
        {
            if (_dicCacheAISTele19.ContainsKey(obj.Id))
                _dicCacheAISTele19[obj.Id] = obj;
            else
                _dicCacheAISTele19.Add(obj.Id, obj);
        }

        private string GetNameFromFicCacheAISTele5(string id)
        {
            if (_dicCacheAISTele5.ContainsKey(id))
                return _dicCacheAISTele5[id].Name;
            else
                return null;
        }

        private string GetNameFromFicCacheAISTele19(string id)
        {
            if (_dicCacheAISTele19.ContainsKey(id))
                return _dicCacheAISTele19[id].Name;
            else
                return null;
        }


        void fireOnDynamic(WXAISTarget target)
        {
            if(OnDynamicEvent != null)
                OnDynamicEvent(target);
        }

        public class WXAISParser
        {
            private Parser _parser = null;
            private VarLenBuffer _buffer = null;
            private DateTime _baseTime = new DateTime(1970, 1, 1);
            public WXAISParser()
            {
                _buffer = new VarLenBuffer();
                _parser = new Parser();
            }

            public Msg[] Parse(byte[] pb, int offset, int len)
            {
                _buffer.Append(pb, offset, len);
                List<byte[]> list = parseMsg();
                List<Msg> result = new List<Msg>();
                foreach (byte[] buf in list)
                {
                    Msg[] msgs = _parser.Parse(buf, 0, buf.Length);
                    if (msgs.Length > 0)
                    {
                        DateTime time = parseTime(buf);
                        msgs[0].DataTime = parseTime(buf);
                        result.Add(msgs[0]);
                    }
                }
                return result.ToArray();
            }

            private List<byte[]> parseMsg()
            {
                List<byte[]> list = new List<byte[]>();
                int index = 0;
                while (index < _buffer.Length - 1)
                {
                    if (_buffer.Buffer[index] == 92 && _buffer.Buffer[index + 1] == 115)// \s
                    {
                        int end = index;
                        while (end < _buffer.Length - 1 && _buffer.Buffer[end] != 13 && _buffer.Buffer[end + 1] != 10)// /r/n
                            end++;
                        if (end < _buffer.Length - 1)
                        {
                            byte[] temp = new byte[end + 2 - index];
                            Array.Copy(_buffer.Buffer, index, temp, 0, temp.Length);
                            list.Add(temp);
                            index = end + 2;
                        }
                        else
                            break;
                    }
                    else
                        index++;
                }
                _buffer.Remove(index);
                return list;
            }

            private DateTime parseTime(byte[] pb)
            {
                string str = Encoding.ASCII.GetString(pb);
                int start = str.IndexOf("c:");
                int end = str.IndexOf("*");
                string secondsStr = str.Substring(start + 2, end - start - 2);
                double seconds = 0;
                double.TryParse(secondsStr, out seconds);
                return _baseTime.AddSeconds(seconds).ToLocalTime();
            }

            private class VarLenBuffer
            {
                public const int BlockLen = 10000;
                public const int MaxBlockLen = 1024 / 8 * 1024 * 1;//最大缓冲区1M
                private byte[] _buffer = new byte[BlockLen];
                public byte[] Buffer
                {
                    get { return _buffer; }
                }
                private int _length = 0;
                public int Length
                {
                    get { return _length; }
                }
                public void Append(byte[] pb, int offset, int len)
                {
                    if (_length + len > _buffer.Length)
                    {
                        byte[] temp = new byte[((_length + len) / BlockLen + 1) * 2 * BlockLen];
                        Array.Copy(_buffer, 0, temp, 0, _length);
                        _buffer = temp;
                    }
                    Array.Copy(pb, offset, _buffer, _length, len);
                    _length += len;
                    if (_length > MaxBlockLen)
                        Remove(_length);
                }

                public void Remove(int len)
                {
                    if (len <= 0)
                        return;
                    if (_length <= len)
                        _length = 0;
                    else
                    {
                        Array.Copy(_buffer, len, _buffer, 0, _length - len);
                        _length -= len;
                    }
                    if (_buffer.Length / BlockLen >= ((_length / BlockLen) + 1) * 2)
                    {
                        byte[] temp = new byte[(_length / BlockLen + 1) * BlockLen];
                        Array.Copy(_buffer, 0, temp, 0, _length);
                        _buffer = temp;
                    }
                }
            }
        }
    }
}
