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
    /// ImportAreaByDataBaseClient.xaml 的交互逻辑
    /// </summary>
    public partial class ImportAreaByDataBaseClient : Window
    {
        public string Data { get; private set; }

        public ImportAreaByDataBaseClient(string title, string data)
        {
            InitializeComponent();
            this.Title = title;
            tbData.Text = data;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            Data = tbData.Text;
            if (string.IsNullOrWhiteSpace(Data))
                MessageBox.Show("请输入有效字符！！");
            else
            {
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
