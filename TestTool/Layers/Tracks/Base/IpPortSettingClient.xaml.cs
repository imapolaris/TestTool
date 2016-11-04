using Common.Logging;
using System;
using System.Windows;

namespace VTSCore.Layers.Tracks
{
    /// <summary>
    /// IpPortSettingClient.xaml 的交互逻辑
    /// </summary>
    public partial class IpPortSettingClient : Window
    {
        public string Ip { get; private set; }
        public string Port { get; private set; }
        ILog LogService { get { return LogManager.GetLogger(GetType()); } }
        public IpPortSettingClient(string ip, string port, string fileName)
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.Title = fileName;
            Ip = ip;
            Port = port;
            load();
        }

        private void load()
        {
            tbIp.Text = Ip;
            tbPort.Text = Port.ToString();
        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string port = tbPort.Text;
                this.DialogResult = true;
                Ip = tbIp.Text;
                Port = port;
                this.Close();
            }
            catch(Exception ex)
            {
                LogService.Error(ex.ToString());
                MessageBox.Show(ex.Message);
            }
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
