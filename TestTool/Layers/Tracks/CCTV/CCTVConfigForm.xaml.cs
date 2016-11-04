using Common.Logging;
using System;
using System.Windows;

namespace VTSCore.Layers.Tracks.CCTV
{
    /// <summary>
    /// CCTVConfigForm.xaml 的交互逻辑
    /// </summary>
    public partial class CCTVConfigForm : Window
    {
        public ConfigCCTV Config { get; private set; }
        ILog LogService { get { return LogManager.GetLogger(GetType()); } }

        public CCTVConfigForm(ConfigCCTV config)
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            Config = config;
            CCTVConfigFormLoad();
        }

        private void CCTVConfigFormLoad()
        {
            tbIp.Text = Config.Ip;
            cbCCTVMode.SelectedIndex = getIndex(Config.Bandwidth);
        }

        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Config.Ip = tbIp.Text;
                Config.Bandwidth = getBandwidth(cbCCTVMode.SelectedIndex);
                this.DialogResult = true;
                this.Close();
            }
            catch(Exception ex)
            {
                LogService.Error(ex.ToString());
                MessageBox.Show(ex.ToString());
            }
        }

        private void btQuit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private int getIndex(int bandwidth)
        {
            if (bandwidth < 512000)
                return 2;
            else if (bandwidth < 2000000)
                return 1;
            else
                return 0;
        }

        private int getBandwidth(int index)
        {
            switch (index)
            {
                case 1:
                    return 512000;
                case 2:
                    return 128000;
                default:
                    return 2000000;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            VTSCore.Common.WindowFrontor.Instance.FrontWindow();
        }
    }
}
