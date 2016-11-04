using SeeCool.GISFramework.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTSCore.Layers.Tracks
{
    public class SDGPSDataReceiver : VTSPlugin.TcpipReceiver
    {
        private SDGPSParser _parser = null;
        public event Action<SDGPSData> OnReceivedData;
        protected override void onConnected()
        {
            _parser = new SDGPSParser();
        }
        protected override void onRecv(byte[] buf, int len)
        {
            SDGPSData[] msgs = _parser.Parse(buf, 0, len);
            foreach (SDGPSData msg in msgs)
            {
                if (OnReceivedData != null)
                    OnReceivedData(msg);
            }
        }


        public class SDGPSParser
        {
            private VarLenBuffer _buffer = null;


            public SDGPSParser()
            {

                _buffer = new VarLenBuffer();
            }

            public SDGPSData[] Parse(byte[] bytes, int offset, int len)
            {

                _buffer.Append(bytes, offset, len);
                List<SDGPSData> result = new List<SDGPSData>();
                int start = 0;
                int cmdOffeset = 0;
                int cmdLength = 0;
                while (tryGetNextNO29Cmd(_buffer.Buffer, start, _buffer.Length - start, out cmdOffeset, out cmdLength))
                {
                    start += cmdOffeset;

                    byte cmdType = _buffer.Buffer[start + 2];
                    SDGPSData tmp = null;
                    switch (cmdType)
                    {
                        case 0x80: tmp = parseNO2980Cmd(_buffer.Buffer, start, out cmdLength); break;
                        case 0x8E: tmp = parseNO298ECmd(_buffer.Buffer, start, out cmdLength); break;
                    }

                    if (tmp != null)
                        result.Add(tmp);
                    start += cmdLength;
                }
                start += cmdOffeset;
                _buffer.Remove(start);
                return result.ToArray();
            }


            private bool tryGetNextNO29Cmd(byte[] buffer, int offeset, int len, out int resultOffset, out int cmdLength)
            {
                resultOffset = 0;//当前位置
                while (resultOffset + 5 < len) //0x29 0x29信令信令长度在偏移第4字节处
                {
                    byte head1 = buffer[offeset + resultOffset];
                    byte head2 = buffer[offeset + resultOffset + 1];

                    if (head1 == 0x29 && head2 == 0x29)//包头为 0x29 0x29
                    {
                        //缓冲区中的数据数量是否大于当前信令长度
                        cmdLength = buffer[offeset + resultOffset + 4] + 5;// 包头及校验占额外5字节
                        if (cmdLength > 0)
                        {
                            if (resultOffset + cmdLength > len)
                                break;//数据不够了


                            //校验位是否为0xD
                            byte check = buffer[offeset + resultOffset + cmdLength - 1];
                            if (check == 0x0D)
                                return true;
                        }
                    }
                    ++resultOffset;
                }
                cmdLength = 0;
                return false;
            }

            private SDGPSData parseNO2980Cmd(byte[] buffer, int start, out int cmdLen)
            {
                SDGPSData result = new SDGPSData();
                cmdLen = buffer[start + 4] + 5;
                try
                {
                    string ip;
                    DateTime dt;
                    double Lat, Lon;

                    start += 5;
                    start += ParseField.ParseFieldIP(buffer, start, out ip);
                    start += ParseField.ParseFieldTime(buffer, start, out dt);
                    start += ParseField.ParseFieldLat(buffer, start, out Lat);
                    start += ParseField.ParseFieldLon(buffer, start, out Lon);
                    start += ParseField.ParseFieldSpeed(buffer, start, out result.SOG);
                    start += ParseField.ParseFieldAngle(buffer, start, out result.COG);
                    start += ParseField.ParseFieldPosition(buffer, start, out result.Position);
                    start += ParseField.ParseFieldDistance(buffer, start, out result.Distance);

                    result.Name = getNameByIP(ip);
                    result.Id = ip.Replace('.', '-');
                    result.Src = "";
                    result.Time = dt;
                    result.Shape = new GeoPointShape(Lon, Lat);
                    return result;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            private SDGPSData parseNO298ECmd(byte[] buffer, int start, out int cmdLen)
            {
                SDGPSData result = new SDGPSData();
                cmdLen = buffer[start + 4] + 5;
                try
                {
                    string ip;
                    DateTime dt;
                    double Lat, Lon;

                    start += 5;
                    start += ParseField.ParseFieldIP(buffer, start, out ip);
                    start += ParseField.ParseFieldTime(buffer, start, out dt);
                    start += ParseField.ParseFieldLat(buffer, start, out Lat);
                    start += ParseField.ParseFieldLon(buffer, start, out Lon);
                    start += ParseField.ParseFieldSpeed(buffer, start, out result.SOG);
                    start += ParseField.ParseFieldAngle(buffer, start, out result.COG);
                    start += ParseField.ParseFieldPosition(buffer, start, out result.Position);
                    result.Name = getNameByIP(ip);
                    result.Id = ip.Replace('.', '-');
                    result.Src = "";
                    result.Time = dt;
                    result.Shape = new GeoPointShape(Lon, Lat);
                    result.Distance = 0;
                    return result;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            private string getNameByIP(string ip)
            {
                IUniqueObj[] objArray = StaticObjMgr.Instance.GetSnapshot("山东执法车");
                foreach (IUniqueObj iuo in objArray)
                {
                    SubjectShip info = iuo as SubjectShip;
                    if (info["CarIp"].ToString() == ip)
                        return info["CarNumber"].ToString();
                }
                return ip;
            }

            private class ParseField
            {

                static public int ParseFieldIP(byte[] buffer, int start, out string ip)
                {
                    ip = buffer[start++].ToString()
                        + "." + buffer[start++].ToString()
                        + "." + buffer[start++].ToString()
                        + "." + buffer[start++].ToString();
                    return 4;
                }

                static public int ParseFieldTime(byte[] buffer, int start, out DateTime datatime)
                {
                    int Year = (buffer[start] >> 4) * 10 + (buffer[start] & 0x0F) + 2000;
                    int Month = (buffer[start + 1] >> 4) * 10 + (buffer[start + 1] & 0x0F);
                    int Day = (buffer[start + 2] >> 4) * 10 + (buffer[start + 2] & 0x0F);
                    int Hour = (buffer[start + 3] >> 4) * 10 + (buffer[start + 3] & 0x0F);
                    int Minute = (buffer[start + 4] >> 4) * 10 + (buffer[start + 4] & 0x0F);
                    int Second = (buffer[start + 5] >> 4) * 10 + (buffer[start + 5] & 0x0F);
                    datatime = new DateTime(Year, Month, Day, Hour, Minute, Second);
                    return 6;
                }

                static public int ParseFieldLat(byte[] buffer, int start, out double Lat)
                {
                    Lat = (buffer[start] & 0x0F) * 10
                          + (buffer[start + 1] >> 4)
                          + (
                                (buffer[start + 1] & 0x0F) * 10 + (buffer[start + 2] >> 4)
                              + (buffer[start + 2] & 0x0F) / 10.0 + (buffer[start + 3] >> 4) / 100.0
                              + (buffer[start + 3] & 0x0F) / 1000.0
                            ) / 60.0;

                    if (buffer[start] >> 4 > 0)
                    {
                        Lat = -Lat;
                    }
                    return 4;
                }

                static public int ParseFieldLon(byte[] buffer, int start, out double Lon)
                {
                    Lon = (buffer[start + 0] >> 4) * 100
                        + (buffer[start + 0] & 0x0F) * 10
                        + (buffer[start + 1] >> 4)
                        + (
                                (buffer[start + 1] & 0x0F) * 10
                                + (buffer[start + 2] >> 4)
                                + (buffer[start + 2] & 0x0F) / 10.0
                                + (buffer[start + 3] >> 4) / 100.0
                                + (buffer[start + 3] & 0x0F) / 1000.0
                           ) / 60.0;
                    return 4;
                }

                static public int ParseFieldSpeed(byte[] buffer, int start, out double speed)
                {
                    speed = (buffer[start] >> 4) * 1000
                            + (buffer[start] & 0x0F) * 100
                            + (buffer[start + 1] >> 4) * 10
                            + (buffer[start + 1] & 0x0F);
                    return 2;
                }

                static public int ParseFieldAngle(byte[] buffer, int start, out double angle)
                {
                    angle = (buffer[start] >> 4) * 1000
                        + (buffer[start] & 0x0F) * 100
                        + (buffer[start + 1] >> 4) * 10
                        + (buffer[start + 1] & 0x0F);
                    return 2;
                }

                static public int ParseFieldDistance(byte[] buffer, int start, out int distance)
                {
                    distance = (buffer[start] << 16)
                                + (buffer[start + 1] << 8)
                                + (buffer[start + 2]);
                    return 3;
                }

                static public int ParseFieldPosition(byte[] buffer, int start, out int position)
                {
                    position = buffer[start];
                    return 1;
                }

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
