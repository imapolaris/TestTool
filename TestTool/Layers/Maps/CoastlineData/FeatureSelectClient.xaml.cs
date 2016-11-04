using SeeCool.GISFramework.Object;
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
using VTSCore.Layers.Maps;

namespace TestTool.Layers.Maps
{
    class CompactFeatureShowEvent
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string FileName { get; set; }
        public object Tag { get; set; }
    }
    /// <summary>
    /// FeatureSelectClient.xaml 的交互逻辑
    /// </summary>
    public partial class FeatureSelectClient : Window, IDisposable
    {
        private List<CompactFeatureObj> _cache = new List<CompactFeatureObj>();
        List<CompactFeatureShowEvent> _dataTable = new List<CompactFeatureShowEvent>();


        public Action<string> OnResetRegion;
        public FeatureSelectClient()
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            mouseRightList.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void RefreshData(List<CompactFeatureObj> list)
        {
            fireResetRegion(null);
            _cache = list;
            updateTableBaseData();
        }

        private void updateTableBaseData()
        {
            _dataTable.Clear();
            featureSelectedListView.Items.Clear();
            foreach (var obj in _cache)
            {
                if (!cbShowAll.IsChecked.Value && string.IsNullOrEmpty(obj.Name.Trim()))
                    continue;
                var cf = new CompactFeatureShowEvent();
                cf.Name = obj.Name;
                cf.Type = obj.FeatureType;
                cf.FileName = obj.Src;
                cf.Tag = obj;
                _dataTable.Add(cf);
                featureSelectedListView.Items.Add(cf);
            }
        }

        private void cbShowAll_Click(object sender, RoutedEventArgs e)
        {
            updateTableBaseData();
        }

        private void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                object item = VTSCore.Layers.Base.ListViewBaseInfo.GetElementFromPoint((ItemsControl)sender, e.GetPosition((ItemsControl)sender));
                if (item != null)
                    centerShow();
            }
        }

        private void centeredMenu_Click(object sender, RoutedEventArgs e)
        {
            centerShow();
        }

        private void ExportGIMenu_Click(object sender, RoutedEventArgs e)
        {
            CompactFeatureObj obj = getCompactFeatureObj();
            if (obj != null)
            {
                StringShowClient client = new StringShowClient(obj.Shape.ToString(), "空间地理信息", obj.Name);
                client.ShowDialog();
            }
        }

        private void centerShow()
        {
            CompactFeatureObj obj = getCompactFeatureObj();
            if (obj != null)
                fireResetRegion(obj.Shape.ToString());
        }

        CompactFeatureObj getCompactFeatureObj()
        {
            if (featureSelectedListView.SelectedIndex >= 0)
                return (CompactFeatureObj)_dataTable[featureSelectedListView.SelectedIndex].Tag;
            return null;
        }

        void fireResetRegion(string shape)
        {
            if(OnResetRegion != null)
                OnResetRegion(shape);
        }
        
        public void Dispose()
        {
            fireResetRegion(null);
            VTSCore.Data.Common.MenuBarsBaseInfo.Instance.FeatureSelectShape = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Dispose();
            VTSCore.Common.WindowFrontor.Instance.FrontWindow();
        }

        private void featureSelectedListView_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            mouseRightList.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void featureSelectedListView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            mouseRightList.Visibility = System.Windows.Visibility.Visible;
        }
    }
}
