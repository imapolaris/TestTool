using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VTSCore.Layers.Maps
{
    /// <summary>
    /// MapLoadingClient.xaml 的交互逻辑
    /// </summary>
    public partial class MapLoadingClient : Window
    {
        LoadingStatusInfo _loading;
        public MapLoadingClient(LoadingStatusInfo loading)
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            _loading = loading;
            _loading.PropertyChanged += _loading_PropertyChanged;
        }

        void _loading_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.Close();
        }
    }
}