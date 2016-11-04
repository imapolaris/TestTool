using Seecool.Radar.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VTSCore.Layers.Plotting
{
    /// <summary>
    /// ImportAreaByStringClient.xaml 的交互逻辑
    /// </summary>
    public partial class ImportAreaByStringClient : Window
    {
        string _heading;
        private List<PointD> _polygon = new List<PointD>();
        public string Heading { get { return _heading; } }

        public PointD[] Polygon { get { return _polygon.ToArray(); } }
        public Action OnSaving;
        public ImportAreaByStringClient()
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            initToolTip();
        }

        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            if (!PlottingAreaSettingInfomation.Instance.IsEffectiveName(tbName.Text))
                return;
            try
            {
                _heading = tbName.Text;
                string strPolygon = tbData.Text;
                char[] ch = new char[] { '\r', '\n' };
                string[] datas = strPolygon.Split(ch, StringSplitOptions.RemoveEmptyEntries);
                if (datas.Length > 0)
                {
                    PointD[] points = null;
                    if (datas[0] == "Polygon")//
                        points = RadarRegionFromString.GetRegion(datas);
                    else if (datas.Length == 1)
                        points = RadarRegionFromString.GetRegion(datas[0]).Polygon;

                    if (points != null)
                    {
                        _polygon.AddRange(points);
                        if(OnSaving != null)
                            OnSaving();
                        return;
                    }
                }
            }
            catch
            {
            }
            MessageBox.Show("导入数据失败,请检查区域数值输入是否有效！！");
        }

        private void btQuit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void initToolTip()
        {
            string str = "输入数据格式有两种：" + Environment.NewLine;
            str += "只有一行数据情况下:" + Environment.NewLine + "Polygon,经度,纬度,经度,纬度..." + Environment.NewLine;
            str += "多行数据模式下格式如下:" + Environment.NewLine;
            str += "Polygon" + Environment.NewLine;
            str += "120°04\'49.80\"E 30°50\'49.75\"N" + Environment.NewLine;
            str += "120°05\'12.74\"E 30°50\'49.75\"N" + Environment.NewLine;
            str += "120°05\'12.74\"E 30°51\'05.83\"N" + Environment.NewLine;
            str += "120°04\'49.80\"E 30°51\'05.83\"N" + Environment.NewLine;
            str += "120°04\'49.80\"E 30°50\'49.75\"N" + Environment.NewLine;
            tbData.ToolTip = str;
        }
    }
}
