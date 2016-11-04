using Common.Logging;
using SeeCool.GISFramework.Object;
using SeeCool.GISFramework.Util;
using System;
using System.Collections.Generic;
using System.Data;
using TestTool.WebReference;

namespace VTSCore.Layers.Tracks
{
    public class HZGPSDataReceiver : GPSBaseReceover
    {
        private PeriodTimer _timer = null;
        private HZShipDataWebService _service = null;

        private Dictionary<string, DateTime> _dicCacheData = new Dictionary<string, DateTime>();

        //Url = "http://172.21.25.33:30611/HZShipDataWebService.asmx";
        ILog LogService { get { return LogManager.GetLogger(GetType()); } }
        protected override void onLoad()
        {
            base.onLoad();
            if (string.IsNullOrEmpty(Url))
                LogService.Error("获取湖州船舶数据的WebServiceUrl配置有误!");
            else
            {
                _service = new HZShipDataWebService();
                _service.Url = Url;
                int interval = 3000;//:默认刷新间隔为3秒;
                _timer = new PeriodTimer(interval, onRefreshGPSData);
            }
        }

        private void onRefreshGPSData(object state)
        {
            try
            {
                if (_service != null)
                {
                    string ex = string.Empty;
                    using (DataTable dt = _service.GetShipDataTable(out ex))
                    {
                        HZGPSData data = null;
                        int count = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            data = dataRowToGPSData(dr);
                            if (data != null && !string.IsNullOrEmpty(data.Name))
                            {
                                if (judegeNewData(data))
                                {
                                    fireOnDynamic(data);
                                    count++;
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception exp)
            {
                LogService.Error(exp.ToString());
            }
            GC.Collect();
        }

        private bool judegeNewData(HZGPSData obj)
        {
            if (_dicCacheData.ContainsKey(obj.Name))
            {
                var dic = _dicCacheData[obj.Name];
                if (dic != obj.Time)
                {
                    dic = obj.Time;
                    return true;
                }
                else
                    return false;

            }
            else
            {
                _dicCacheData.Add(obj.Name, obj.Time);
                return true;
            }
        }

        private HZGPSData dataRowToGPSData(DataRow dr)
        {
            HZGPSData data = new HZGPSData();
            if (dr["name"] != DBNull.Value)
                data.Id = dr["name"].ToString();
            if (dr["name"] != DBNull.Value)
                data.Name = dr["name"].ToString();
            double lon = 0, lat = 0;
            if (dr["x"] != DBNull.Value)
            {
                double.TryParse(dr["x"].ToString(), out lon);
            }
            else
                return null;
            if (dr["y"] != DBNull.Value)
            {
                double.TryParse(dr["y"].ToString(), out lat);
            }
            else
                return null;
            data.Shape = new GeoPointShape(lon, lat);
            if (dr["speed"] != DBNull.Value)
            {
                double.TryParse(dr["speed"].ToString(), out data.SOG);
            }
            if (dr["direction"] != DBNull.Value)
            {
                double.TryParse(dr["direction"].ToString(), out data.COG);
            }
            data.Heading = 511;
            if (dr["gpstime"] != DBNull.Value)
            {
                DateTime time;
                DateTime.TryParse(dr["gpstime"].ToString(), out time);
                data.Time = time;
                if ((DateTime.Now - time).TotalDays > 1)
                    return null;
            }
            if (dr["region"] != DBNull.Value)
            {
                string qy = dr["region"].ToString();
                if (qy == "01")
                    data.SSHQ = "杭州";
                if (qy == "02")
                    data.SSHQ = "湖州";
                if (qy == "03")
                    data.SSHQ = "嘉兴";
            }
            if (dr["isonline"] != DBNull.Value)
            {
                string zx = dr["isonline"].ToString();
                if (zx == "1")
                    data.IsOnline = true;
                else
                    data.IsOnline = false;
            }
            //FusionMgr.GetExtraIDs(data, out data.GID, out data.FID);
            return data;
        }
    }
}
