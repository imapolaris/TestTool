using SeeCool.GISFramework.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USNT.DataParser.WIS;

namespace SeeCool.GISFramework.SvrFramework
{
    public class NorcontrolReceiver : VTSPlugin.TcpipReceiver
    {
        private Parser _parser = new Parser();

        private Dictionary<string, NorcontrolData> _dic = new Dictionary<string, NorcontrolData>();

        protected override void onConnected()
        {
            base.onConnected();
            _parser = new Parser();
        }
        public event Action<NorcontrolData> OnReceivedData;

        protected override void onRecv(byte[] buf, int len)
        {
            USNT.DataParser.WIS.Target[] msgs = _parser.Parse(buf, 0, len);
            foreach (USNT.DataParser.WIS.Target t in msgs)
            {
                //if (t.Movable)
                {
                    NorcontrolData nt = new NorcontrolData();
                    nt.Src = "";
                    nt.Id = t.Id.ToString();
                    nt.Time = DateTime.Now;

                    if (String.IsNullOrEmpty(t.Name))
                    {
                        if (_dic.ContainsKey(nt.Id))
                            nt.Name = _dic[nt.Id].Name;
                    }
                    else
                        nt.Name = t.Name;

                    if (t.MMSI != 0)
                        nt.MMSI = t.MMSI;
                    else if (_dic.ContainsKey(nt.Id))
                        nt.MMSI = _dic[nt.Id].MMSI;

                    if (t.ShipCargoType != 0)
                        nt.ShipCargoType = t.ShipCargoType;
                    else if (_dic.ContainsKey(nt.Id))
                        nt.ShipCargoType = _dic[nt.Id].ShipCargoType;

                    nt.Shape = new GeoPointShape(t.Longitude, t.Latitude);
                    nt.SOG = t.SOG;
                    nt.COG = t.COG;
                    nt.Heading = t.TrueHeading;
                    nt.Movable = t.Movable;
                    nt.SensorType = t.SensorType;
                    nt.AISTypeMask = t.AisTypeMask;
                    nt.OrgType = t.Type;
                    nt.ROT = t.ROT;

                    string ss = t.CallSign.Replace(",", " ");
                    if (String.IsNullOrEmpty(ss))
                    {
                        if (_dic.ContainsKey(nt.Id))
                            nt.CallSign = _dic[nt.Id].CallSign;
                    }
                    else
                        nt.CallSign = ss;

                    ss = t.Destination.Replace(",", " ");
                    if (String.IsNullOrEmpty(ss))
                    {
                        if (_dic.ContainsKey(nt.Id))
                            nt.Destination = _dic[nt.Id].Destination;
                    }
                    else
                        nt.Destination = ss;
                    if (t.ETA == DateTime.MinValue)
                    {
                        if (_dic.ContainsKey(nt.Id))
                            nt.ETA = _dic[nt.Id].ETA;
                    }
                    else
                        nt.ETA = t.ETA;
                    if (t.IMO_Number == 0)
                    {
                        if (_dic.ContainsKey(nt.Id))
                            nt.IMO_Number = _dic[nt.Id].IMO_Number;
                    }
                    else
                        nt.IMO_Number = t.IMO_Number;

                    if (t.Length > 0)
                        nt.Length = t.Length;
                    else if (_dic.ContainsKey(nt.Id))
                        nt.Length = _dic[nt.Id].Length;

                    if (t.MaxSeaGauge > 0)
                        nt.MaxSeaGauge = t.MaxSeaGauge;
                    else if (_dic.ContainsKey(nt.Id))
                        nt.MaxSeaGauge = _dic[nt.Id].MaxSeaGauge;

                    if (t.NavStatus != 15)
                        nt.NavStatus = t.NavStatus;
                    else if (_dic.ContainsKey(nt.Id))
                        nt.NavStatus = _dic[nt.Id].NavStatus;

                    nt.Persons = t.Persons;

                    if (t.RefToLarboard > 0)
                        nt.RefToLarboard = t.RefToLarboard;
                    else if (_dic.ContainsKey(nt.Id))
                        nt.RefToLarboard = _dic[nt.Id].RefToLarboard;
                    if (t.Width > 0)
                        nt.Width = t.Width;
                    else if (_dic.ContainsKey(nt.Id))
                        nt.Width = _dic[nt.Id].Width;

                    if (t.RefToProw > 0)
                        nt.RefToProw = t.RefToProw;
                    else if (_dic.ContainsKey(nt.Id))
                        nt.RefToProw = _dic[nt.Id].RefToProw;
                    if (t.Length > 0)
                        nt.Length = t.Length;
                    else if (_dic.ContainsKey(nt.Id))
                        nt.Length = _dic[nt.Id].Length;

                    if (t.Width > 0)
                        nt.Width = t.Width;
                    else if (_dic.ContainsKey(nt.Id))
                        nt.Width = _dic[nt.Id].Width;

                    if (!_dic.ContainsKey(nt.Id))
                        _dic.Add(nt.Id, nt);
                    else
                        _dic[nt.Id] = nt;

                    if (nt.MMSI != 0)
                        nt.Id = "MMSI:" + nt.MMSI.ToString();


                    if (OnReceivedData != null)
                        OnReceivedData(nt);

                    ////UpdateJYXXInfo(nt);//更新简要信息
                    //Framework.BroadcastByRegion(new NetCmd(FrameworkNetCmdType.SC_Realtime_String, nt.Format()), nt.Shape);
                    //StorageService service = Framework.GetService("Storage") as StorageService;
                    //if (service != null)
                    //{
                    //    service.SaveRealtime(nt);
                    //    service.SaveHistory(nt);
                    //}
                }
            }
        }
    }

}
