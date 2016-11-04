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
    /// ModifyValueClient.xaml 的交互逻辑
    /// </summary>
    public partial class ModifyValueClient : Window
    {
        public string Heading { get; set; }
        public ModifyValueClient(string title, string value, string naming = "名称：")
        {
            InitializeComponent();
            this.Title = title;
            Heading = value;
            tbHeading.Text = value;
            lbHeading.Content = naming;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }
        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            Heading = tbHeading.Text;
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
