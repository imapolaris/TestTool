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
    /// StringSettingClient.xaml 的交互逻辑
    /// </summary>
    public partial class StringSettingClient : Window
    {
        public StringSettingClient(string stringStart, string name, string title = "配置窗口")
        {
            InitializeComponent();
            tbData.Text = stringStart;
            lbName.Content = name;
            this.Title = title;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }
        public string Data { get; private set; }
        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(tbData.Text))
            {
                Data = tbData.Text;
                this.DialogResult = true;
                this.Close();
            }
        }

        private void btQuit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
