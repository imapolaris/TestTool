using SeeCool.GISFramework.Object;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTSCore.Layers.Tracks
{
    /// <summary>
    /// LiangJF 01/12/2012
    /// 专题船舶信息对象
    /// DataType,DataSrc,DataId是数据库表的保留字
    /// </summary>
    [Serializable]
    public class SubjectShip : SubjectObj
    {
        private static List<string> _listReserved = new List<string>();

        public string ShipType;
        public string ShipSrc;
        public string ShipId;

        static SubjectShip()
        {
            _listReserved.Add("DataType");
            _listReserved.Add("DataSrc");
            _listReserved.Add("DataId");
        }

        protected override bool isReservedKeyword(string key)
        {
            return base.isReservedKeyword(key);
        }

        protected override void onLoad()
        {
            base.onLoad();

            ShipType = (string)getValue("DataType");
            ShipSrc = (string)getValue("DataSrc");
            if (ShipSrc == null)
                ShipSrc = String.Empty;
            ShipId = (string)getValue("DataId");
        }

        protected override void onSave(DataRow dr)
        {
            base.onSave(dr);

            dr["DataType"] = ShipType;
            if (!String.IsNullOrEmpty(ShipSrc))
                dr["DataSrc"] = ShipSrc;
            else
                dr["DataSrc"] = DBNull.Value;
            dr["DataId"] = ShipId;
        }
    }
}
