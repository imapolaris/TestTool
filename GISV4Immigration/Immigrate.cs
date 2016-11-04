using Seecool.Radar;
using SeeCool.GISFramework.Object;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace GISV4Immigration
{
    public static class Immigrate
    {
        static RadarRegionInfo[] _emptyRegionInfo = new RadarRegionInfo[0];

        public static RadarRegionInfo[] LoadRadarRegions(string connectionString)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                    return getRegions(con).ToArray();
            }
            catch
            {
                return _emptyRegionInfo;
            }
        }

        private static IEnumerable<RadarRegionInfo> getRegions(SqlConnection con)
        {
            string sqlCmd = "select * from [DBT_FEATURE] where Type='LDZBQ'or Type='LDBWDQ'";
            using (SqlDataAdapter sda = new SqlDataAdapter(sqlCmd, con))
            {
                DataTable dt = new DataTable();
                sda.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    string shape = dr["Shape"] as string;
                    string name = dr["Name"] as string;
                    string type = dr["Type"] as string;
                    if (shape != null && name != null && type != null)
                    {
                        GeoAreaShape gas = null;
                        try
                        {
                            gas = GeometryShape.Parse(shape) as GeoAreaShape;
                        }
                        catch
                        {
                            gas = null;
                        }

                        if (gas != null && gas.Polygon.Points.Count > 0)
                        {
                            string relatedRadar = string.Empty;
                            object idObj = dr["Id"];
                            if (idObj is int)
                                relatedRadar = getRelatedRadar(con, (int)idObj);

                            RadarRegion region = new RadarRegion();
                            region.Name = name;
                            region.Polygon = gas.Polygon.Points[0].Points.Select(pt => new Seecool.Radar.Unit.PointD(pt.X, pt.Y)).ToArray();
                            region.IsMask = type.ToUpper() == "LDZBQ";
                            region.ManualIdenfity = type.ToUpper() == "LDBWDQ";
                            yield return new RadarRegionInfo(region, relatedRadar);
                        }
                    }
                }
            }
        }

        private static string getRelatedRadar(SqlConnection con, int featureId)
        {
            DataTable dt = new DataTable();
            try
            {
                string sqlCmd = string.Format("select * from [DBT_ATTRIBUTE] where AttrType='GLLD' and FeatureId='{0}'", featureId);
                using (SqlDataAdapter sda = new SqlDataAdapter(sqlCmd, con))
                {
                    sda.Fill(dt);
                    if (dt.Rows.Count > 0)
                        return dt.Rows[0]["AttrValue"] as string;
                }
            }
            catch
            {
            }

            return string.Empty;
        }

        public static bool SaveRadarRegions(string connectionString, RadarRegionInfo[] regions)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    clearRegions(con);
                    setRegions(con, regions);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        private static void clearRegions(SqlConnection con)
        {
            //string sqlCmd1 = "delete from [DBT_FEATURE] where Type='LDZBQ' or Type='LDBWDQ'";
            //string sqlCmd2 = "delete from [DBT_ATTRIBUTE] where AttrType='GLLD'";
        }

        private static void setRegions(SqlConnection con, RadarRegionInfo[] regions)
        {
        }

    }
}
