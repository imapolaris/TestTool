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
                    return "�̶�����";
                case 1:
                    return "��׼��";
                case 2:
                    return "RACON";
                case 3:
                    return "����������";
                case 4:
                    return "����";
                case 5:
                    return "����,������";
                case 6:
                    return "����,������";
                case 7:
                    return "����ǰ��";
                case 8:
                    return "������";
                case 9:
                    return "�ű�,����λ";
                case 10:
                    return "�ű�,����λ";
                case 11:
                    return "�ű�,�Ϸ�λ";
                case 12:
                    return "�ű�,����λ";
                case 13:
                    return "�ű�,���";
                case 14:
                    return "�ű�,�Ҳ�";
                case 15:
                    return "�ű�,��Ѻ������";
                case 16:
                    return "�ű�,��Ѻ����Ҳ�";
                case 17:
                    return "�ű꣬����Σ����";
                case 18:
                    return "�ű�,��ȫˮ��";
                case 19:
                    return "ר�ñ�־";
                case 20:
                    return "����λ��־";
                case 21:
                    return "����λ��־";
                case 22:
                    return "�Ϸ�λ��־";
                case 23:
                    return "����λ��־";
                case 24:
                    return "����־";
                case 25:
                    return "�Ҳ��־";
                case 26:
                    return "��Ѻ������";
                case 27:
                    return "��Ѻ����Ҳ�";
                case 28:
                    return "����Σ����";
                case 29:
                    return "��ȫˮ��";
                case 30:
                    return "ר�ñ�־";
                case 31:
                    return "�ƴ�/�����Զ�����������(LANBY)/�꾮ƽ̨";
            }
            return "δ֪";
        }
    }
}
