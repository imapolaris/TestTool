using Common.Logging;
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

namespace VTSCore.Layers.Base
{
    /// <summary>
    /// TimeOutSettingClient.xaml 的交互逻辑
    /// </summary>
    public partial class TimeOutSettingClient : Window
    {
        ILog LogService { get { return LogManager.GetLogger(GetType()); } }
        public TimeOutSettingClient(string data)
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            tbData.Text = data;
        }

        public double Data { get; private set; }
        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Data = double.Parse(tbData.Text);
                this.DialogResult = true;
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
