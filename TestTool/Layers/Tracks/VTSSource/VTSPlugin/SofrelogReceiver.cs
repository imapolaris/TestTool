using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using SeeCool.GISFramework.Object;
using SeeCool.GISFramework.Net;
using SeeCool.GISFramework.Util;

namespace SeeCool.GISFramework.SvrFramework
{
    public class SofrelogReceiver : VTSPlugin.TcpipReceiver
    {
        private class ReceiveBuffer
        {
            public byte[] Buffer { get { return _buffer; } }
            public int Length { get { return _length; } }
            public int BufferSize { get { return _bufferSize; } }

            private int _length;
            private byte[] _buffer;
            private int _bufferSize;

            public ReceiveBuffer()
            {
                init(0);
            }

            public ReceiveBuffer(int initBufferSize)
            {
                init(initBufferSize);
            }

            private void init(int initBufferSize)
            {
                _bufferSize = initBufferSize;
                _buffer = new byte[_bufferSize];
                _length = 0;
            }

            public void Append(byte[] data, int start, int len)
            {
                if (_length + len > _bufferSize)
                {
                    _bufferSize = (_length + len + 0x3FF) & (~0x3FF);
                    byte[] newBuffer = new byte[_bufferSize];
                    Array.Copy(Buffer, newBuffer, _length);
                    _buffer = newBuffer;
                }
                Array.Copy(data, start, _buffer, _length, len);
                _length += len;
            }

            public void Delete(int len)
            {
                len = Math.Min(len, _length);
                _length -= len;
                Array.Copy(_buffer, len, _buffer, 0, _length);
            }

            public void Clear()
            {
                _length = 0;
            }
        }

        public SofrelogReceiver()
        {
        }

        ReceiveBuffer _buffer = new ReceiveBuffer();
        //private Dictionary<int, SofrelogData> _dic = new Dictionary<int, SofrelogData>();
        //private Dictionary<int, int> _idDict = new Dictionary<int, int>();
        object _lockObj = new object();

        public event Action<SofrelogData> OnReceivedData;

        public event Action<int> OnDropData;
        public event Action<int, int> OnResyncID;
        public event Action<int, string> OnResyncName;

        public event Action<int, int> OnResyncClassification;
        public event Action<int, string> OnResyncCallSign;

        protected override void onConnected()
        {
            base.onConnected();
            _buffer.Clear();
        }

        private double getFloat(byte[] data, int start)
        {
            byte[] buf = new byte[4];
            buf[0] = data[start + 3];
            buf[1] = data[start + 2];
            buf[2] = data[start + 1];
            buf[3] = data[start];
            return (double)BitConverter.ToSingle(buf, 0);
        }

        private UInt32 getUInt32(byte[] data, int start)
        {
            UInt32 byte1 = ((UInt32)data[start]) & 0xFF;
            UInt32 byte2 = ((UInt32)data[start + 1]) & 0xFF;
            UInt32 byte3 = ((UInt32)data[start + 2]) & 0xFF;
            UInt32 byte4 = ((UInt32)data[start + 3]) & 0xFF;
            return (byte1 << 24) | (byte2 << 16) | (byte3 << 8) | byte4;
        }

        private bool isTaidMsgType(UInt32 msgType)
        {
            return msgType >= 400 && msgType <= 410;
        }

        protected override void onRecv(byte[] data, int len)
        {
            _buffer.Append(data, 0, len);
            const int maxBufferSize = 0x10000000;
            if (_buffer.BufferSize > maxBufferSize)
                _buffer.Delete(_buffer.Length);

            int index = 0;
            while (index < _buffer.Length)
            {
                while (index + 8 <= _buffer.Length && (!isTaidMsgType(getUInt32(_buffer.Buffer, index)) || getUInt32(_buffer.Buffer, index + 4) >= 500))
                    index++;

                if (index + 8 <= _buffer.Length)
                {
                    int packetSize = 8 + (int)getUInt32(_buffer.Buffer, index + 4);
                    if (index + packetSize <= _buffer.Length)
                    {
                        parseCmd(_buffer.Buffer, index, packetSize);
                        index += packetSize;
                        continue;
                    }
                }
                break;
            }

            _buffer.Delete(index);
        }

        private enum MessageTypes
        {
            TrackNew = 400,
            TrackUpdate = 401,
            TrackDrop = 402,
            Status = 403,
            Error = 404,
            Resync = 405,
            TrackIFFNew = 406,
            TrackIFFUpdate = 407,
            TrackCompl = 408
        };

        private void parseCmd(byte[] buf, int start, int packetSize)
        {
            MessageTypes type = (MessageTypes)getUInt32(buf, start);
            switch (type)
            {
                case MessageTypes.Status:
                    break;
                case MessageTypes.TrackNew:
                    {
                        int trackID = (int)getUInt32(buf, start + 8);
                        parseTrackData(true, trackID, buf, start + 12);
                    }
                    break;
                case MessageTypes.TrackUpdate:
                    {
                        int trackID = (int)getUInt32(buf, start + 8);
                        parseTrackData(false, trackID, buf, start + 12);
                    }
                    break;
                case MessageTypes.TrackDrop:
                    {
                        int trackID = (int)getUInt32(buf, start + 8);
                        dropTrack(trackID);
                    }
                    break;
                case MessageTypes.Error:
                    break;
                case MessageTypes.Resync:
                    {
                        int oldTrackID = (int)getUInt32(buf, start + 8);
                        int newTrackID = (int)getUInt32(buf, start + 12);
                        resyncTrack(oldTrackID, newTrackID);
                    }
                    break;
                case MessageTypes.TrackIFFNew:
                    {
                        int trackID = (int)getUInt32(buf, start + 8);
                        parseIFFTrackData(true, trackID, buf, start + 12);
                    }
                    break;
                case MessageTypes.TrackIFFUpdate:
                    {
                        int trackID = (int)getUInt32(buf, start + 8);
                        parseIFFTrackData(false, trackID, buf, start + 12);
                    }
                    break;
                case MessageTypes.TrackCompl:
                    {
                        int trackID = (int)getUInt32(buf, start + 8);
                        parseCompl(trackID, buf, start + 12, packetSize - 12);
                    }
                    break;
                default:
                    break;
            }
        }

        private void resyncTrack(int oldTrackID, int newTrackID)
        {
            lock (_lockObj)
            {
                if (OnResyncID != null)
                    OnResyncID(oldTrackID, newTrackID);
            }
        }

        private void parseCompl(int trackID, byte[] buf, int start, int len)
        {
            int type = (int)getUInt32(buf, start);
            int field = (int)getUInt32(buf, start + 4);
            switch (field)
            {
                case 0:
                    if (type == 5)
                    {
                        string name = parseString(buf, start + 28);
                        if (OnResyncName != null)
                            OnResyncName(trackID, name);
                    }
                    break;
                case 1:
                    if (type == 0)
                    {
                        int classification = (int)getUInt32(buf, start + 8);
                        if (OnResyncClassification != null)
                            OnResyncClassification(trackID, classification);
                    }
                    break;
                case 2:
                    if (type == 5)
                    {
                        string callsign = parseString(buf, start + 28);
                        if (OnResyncCallSign != null)
                            OnResyncCallSign(trackID, callsign);
                    }
                    break;
            }
        }

        private string parseString(byte[] buf, int start)
        {
            int len = (int)getUInt32(buf, start);
            return Encoding.UTF8.GetString(buf, start + 4, len);
        }

        private void parseIFFTrackData(bool isNewTrack, int trackID, byte[] buf, int start)
        {
            parseTrackData(isNewTrack, trackID, buf, start);
        }

        private void parseTrackData(bool isNewTrack, int trackID, byte[] buf, int start)
        {
            if (isNewTrack)
                dropTrack(trackID);

            SofrelogData sd = new SofrelogData(trackID);
            sd.Src = "";
            sd.Time = DateTime.Now;
            sd.TrackID = trackID;
            uint msgCount = getUInt32(buf, start);
            sd.MMSI = (int)getUInt32(buf, start + 4);
            sd.MainRadar = (int)getUInt32(buf, start + 8);
            sd.TrackPlatform = (SofrelogData.TrackPlatforms)getUInt32(buf, start + 12);
            int classification = (int)getUInt32(buf, start + 16);
            if (classification > 0)
                sd.Classification = classification;
            sd.QualityFactor = (int)getUInt32(buf, start + 20);
            sd.LackOfDetection = (int)getUInt32(buf, start + 24);
            sd.PlotCells = (int)getUInt32(buf, start + 28);
            double lon = (double)getUInt32(buf, start + 32);
            double lonMinute = getFloat(buf, start + 36);
            lon += lonMinute / 60;
            double lat = (double)getUInt32(buf, start + 40);
            double latMinute = getFloat(buf, start + 44);
            lat += latMinute / 60;
            sd.Shape = new GeoPointShape(lon, lat);
            sd.COG = getFloat(buf, start + 48);
            sd.SOG = getFloat(buf, start + 52);
            long secs = (long)getUInt32(buf, start + 56);
            int milsecs = (int)getUInt32(buf, start + 60);
            sd.TimeStamp = new DateTime(1970, 1, 1) + new TimeSpan((secs * 1000 + milsecs) * 10000);
            sd.TrackLock = (SofrelogData.TrackLocks)getUInt32(buf, start + 64);

            updateData(sd);
        }

        private void updateData(SofrelogData sd)
        {
            if (sd.Shape != null)
            {
				if (OnReceivedData != null)
					OnReceivedData(sd);
            }
        }

        private void dropTrack(int trackID)
        {
            if (OnDropData != null)
                OnDropData(trackID);
        }
    }
}
