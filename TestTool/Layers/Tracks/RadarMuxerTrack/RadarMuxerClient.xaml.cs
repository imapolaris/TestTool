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

namespace VTSCore.Layers.Tracks
{
    /// <summary>
    /// RadarMuxerClient.xaml 的交互逻辑
    /// </summary>
    public partial class RadarMuxerClient : Window
    {
        public string SubEndpoint { get; private set; }
        public string RpcEndpoint { get; private set; }
        public RadarMuxerClient(string subEndpoint, string rpcEndpoint)
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            SubEndpoint = subEndpoint;
            RpcEndpoint = rpcEndpoint;
            initClient();
        }

        void initClient()
        {
            tbSubEndpoint.Text = SubEndpoint;
            tbRpcEndpoint.Text = RpcEndpoint;
        }

        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            SubEndpoint = tbSubEndpoint.Text;
            RpcEndpoint = tbRpcEndpoint.Text;
            this.DialogResult = true;
            this.Close();
        }

        private void btQuit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            VTSCore.Common.WindowFrontor.Instance.FrontWindow();
        }
    }
}