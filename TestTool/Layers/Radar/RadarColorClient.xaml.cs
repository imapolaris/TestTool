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
using VTSCore.Layers.Base;

namespace VTSCore.Layers.Radar
{
    /// <summary>
    /// RadarColorClient.xaml 的交互逻辑
    /// </summary>
    public partial class RadarColorClient : Window
    {
        RadarColorTableDataInfo _colors;
        List<DataEditUnitObj> _scheme = new List<DataEditUnitObj>();
        public RadarColorClient()
        {
            InitializeComponent();
            _colors = RadarColorTableDataInfo.Instance;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            loadColorSchemesListView();
        }

        private void loadColorSchemesListView()
        {
            colorSchemesListView.Items.Clear();
            for (int i = 0; i < _colors.Count; i++)
                colorSchemesListView.Items.Add(_colors.GetTableDataConfig(i));
        }

        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            addScheme();
        }

        private void btDelete_Click(object sender, RoutedEventArgs e)
        {
            deleteScheme();
        }
        
        private void colorSchemesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updateSelectedScheme();
        }

        private void updateSelectedScheme()
        {
            if(colorSchemesListView.SelectedIndex >= 0)
            {
                initColoeSchemesListView(_colors.GetTableDataConfig(colorSchemesListView.SelectedIndex));
            }
            else
            {
                _scheme.Clear();
                colorSchemeListView.Items.Clear();
            }   
        }

        private void initColoeSchemesListView(ColorTableDataConfig colorTableDataConfig)
        {
            if (_scheme.Count == 0)
            {
                _scheme.Add(new DataEditUnitObj() { Heading = "名称", BaseValue = colorTableDataConfig.Heading });
                _scheme.Add(new DataEditUnitObj() { Heading = "余晖级数", BaseValue = colorTableDataConfig.TrailState.ToString() });
                _scheme.Add(new DataEditUnitObj() { Heading = "回波起始", BaseValue = colorTableDataConfig.FrontStart.ToString() });
                _scheme.Add(new DataEditUnitObj() { Heading = "回波结束", BaseValue = colorTableDataConfig.FrontEnd.ToString() });
                _scheme.Add(new DataEditUnitObj() { Heading = "余晖起始", BaseValue = colorTableDataConfig.TrailStart.ToString() });
                _scheme.Add(new DataEditUnitObj() { Heading = "余晖结束", BaseValue = colorTableDataConfig.TrailEnd.ToString() });

                for (int i = 0; i < _scheme.Count; i++)
                    colorSchemeListView.Items.Add(_scheme[i]);
            }
            else if (_scheme.Count >= 6)
            {
                int index = 0;
                _scheme[index++].BaseValue = colorTableDataConfig.Heading;
                _scheme[index++].BaseValue = colorTableDataConfig.TrailState.ToString();
                _scheme[index++].BaseValue = colorTableDataConfig.FrontStart;
                _scheme[index++].BaseValue = colorTableDataConfig.FrontEnd;
                _scheme[index++].BaseValue = colorTableDataConfig.TrailStart;
                _scheme[index++].BaseValue = colorTableDataConfig.TrailEnd;
            }
        }

        private void colorSchemesListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
             object item = ListViewBaseInfo.GetElementFromPoint((ItemsControl)sender, e.GetPosition((ItemsControl)sender));
            if (item != null)
                editorScheme();
        }

        private void addScheme()
        {
            ColorTableDataConfig config = new ColorTableDataConfig() { Heading = "方案 " + (_colors.Count + 1).ToString() };
            RadarColorSchemeEditorClient client = new RadarColorSchemeEditorClient(config);
            if (client.ShowDialog().Value)
            {
                _colors.Add(client.Config);
                loadColorSchemesListView();
                colorSchemesListView.SelectedIndex = colorSchemesListView.Items.Count - 1;
            }
        }

        private void deleteScheme()
        {
            int index = colorSchemesListView.SelectedIndex;
            if (index >= 0)
            {
                string text = "确认删除\"" + _colors.GetTableDataConfig(index).Heading + "\"" + "？";
                if(MessageBox.Show(text, "提示", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    _colors.RemoveAt(index);
                    loadColorSchemesListView();
                }
            }
        }

        private void editorScheme()
        {
            int index = colorSchemesListView.SelectedIndex;
            if (index >= 0)
            {
                RadarColorSchemeEditorClient client = new RadarColorSchemeEditorClient(_colors.GetTableDataConfig(index));
                if (client.ShowDialog().Value)
                    _colors.Editor(index, client.Config);
                loadColorSchemesListView();
                colorSchemesListView.SelectedIndex = index;
            }
        }

        private void btQuit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
