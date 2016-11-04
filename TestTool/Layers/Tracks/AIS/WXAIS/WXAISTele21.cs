using System;
using System.Collections.Generic;
using System.Text;

namespace SeeCool.GISFramework.Object
{
    [Serializable]
    public class WXAISTele21 : CompositeDynamicGeometryObj
    {
        public int EPFD_Type;
        public int Measure_A;
        public int Measure_B;
        public int Measure_C;
        public int Measure_D;
        public bool OffPosition;
        public bool PositionAccuracy;
        public bool RAIM_Flag;
        public int TimeStamp;
        public int AtoNType;

        public override string Type
        {
            get { return "WXAISTELE21"; }
        }

        public override bool IsTimeout
        {
            get
            {
                TimeSpan ts = DateTime.Now - this.Time;
                int std = 60 * 60 * 3;
                return ts.TotalSeconds > std;
            }
        }

        public override string Format()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("WXAIS,21,");
            sb.Append(this.Id);
            sb.Append(",");
            sb.Append(this.Time.ToString());
            sb.Append(",");
            sb.Append(this.Name);
            sb.Append(",");
            GeoPointShape gps = this.Shape as GeoPointShape;
            sb.Append(gps.Point.X.ToString("F6"));
            sb.Append(",");
            sb.Append(gps.Point.Y.ToString("F6"));
            sb.Append(",");
            sb.Append(this.EPFD_Type.ToString());
            sb.Append(",");
            sb.Append(this.Measure_A.ToString());
            sb.Append(",");
            sb.Append(this.Measure_B.ToString());
            sb.Append(",");
            sb.Append(this.Measure_C.ToString());
            sb.Append(",");
            sb.Append(this.Measure_D.ToString());
            sb.Append(",");
            sb.Append(this.OffPosition.ToString());
            sb.Append(",");
            sb.Append(this.PositionAccuracy.ToString());
            sb.Append(",");
            sb.Append(this.RAIM_Flag.ToString());
            sb.Append(",");
            sb.Append(this.TimeStamp.ToString());
            sb.Append(",");
            sb.Append(this.AtoNType.ToString());
            sb.Append(",");
            string result = sb.ToString();
            return result;
        }

        public override void Parse(string[] data)
        {
            int index = 2;
            Id = data[index++];
            Time = DateTime.Parse(data[index++]);
            Name = data[index++];

            double x = double.Parse(data[index++]);
            double y = double.Parse(data[index++]);
            Shape = new GeoPointShape(x, y);

            EPFD_Type = Int32.Parse(data[index++]);
            Measure_A = Int32.Parse(data[index++]);
            Measure_B = Int32.Parse(data[index++]);
            Measure_C = Int32.Parse(data[index++]);
            Measure_D = Int32.Parse(data[index++]);
            OffPosition = Boolean.Parse(data[index++]);
            PositionAccuracy = Boolean.Parse(data[index++]);
            RAIM_Flag = Boolean.Parse(data[index++]);
            TimeStamp = Int32.Parse(data[index++]);
            AtoNType = Int32.Parse(data[index++]);
        }

        public static string GetTypeString(int type)
        {
            switch (type)
            {
                case 0:
                    return "固定航标";
                case 1:
                    return "基准点";
                case 2:
                    return "RACON";
                case 3:
                    return "近海建筑物";
                case 4:
                    return "备用";
                case 5:
                    return "灯塔,无扇区";
                case 6:
                    return "灯塔,有扇区";
                case 7:
                    return "导标前灯";
                case 8:
                    return "导标后灯";
                case 9:
                    return "信标,北方位";
                case 10:
                    return "信标,东方位";
                case 11:
                    return "信标,南方位";
                case 12:
                    return "信标,西方位";
                case 13:
                    return "信标,左侧";
                case 14:
                    return "信标,右侧";
                case 15:
                    return "信标,最佳航道左侧";
                case 16:
                    return "信标,最佳航道右侧";
                case 17:
                    return "信标，孤立危险物";
                case 18:
                    return "信标,安全水域";
                case 19:
                    return "专用标志";
                case 20:
                    return "北方位标志";
                case 21:
                    return "东方位标志";
                case 22:
                    return "南方位标志";
                case 23:
                    return "西方位标志";
                case 24:
                    return "左侧标志";
                case 25:
                    return "右侧标志";
                case 26:
                    return "最佳航道左侧";
                case 27:
                    return "最佳航道右侧";
                case 28:
                    return "孤立危险物";
                case 29:
                    return "安全水域";
                case 30:
                    return "专用标志";
                case 31:
                    return "灯船/大型自动化助航浮标(LANBY)/钻井平台";
            }
            return "未知";
        }
    }
}
