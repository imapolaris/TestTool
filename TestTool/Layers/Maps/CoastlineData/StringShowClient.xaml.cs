using Common.Logging;
using System;
using System.Windows;

namespace VTSCore.Layers.Maps
{
    /// <summary>
    /// StringShowClient.xaml 的交互逻辑
    /// </summary>
    public partial class StringShowClient : Window
    {
        string _exportName;
        ILog LogService { get { return LogManager.GetLogger(GetType()); } }
        public StringShowClient(string showString, string title, string exportName = null)
        {
            InitializeComponent();
            tbData.Text = showString;
            this.Title = title;
            _exportName = exportName;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog file = new System.Windows.Forms.SaveFileDialog();
            file.Filter = "txt文件|*.txt|所有文件|*.*";
            file.RestoreDirectory = true;
            if (!string.IsNullOrWhiteSpace(_exportName))
                file.FileName = _exportName;
            if (file.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fn = file.FileName;
                System.IO.File.WriteAllText(fn, tbData.Text);
            }
        }

        private void btCopy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.Clear();
                Clipboard.SetDataObject(tbData.Text, true);
                MessageBox.Show("已复制到剪贴板。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch(Exception ex)
            {
                LogService.Error("操作剪贴板发生异常，未能复制到剪贴板。\r\n" + ex.ToString());
                MessageBox.Show("操作剪贴板发生异常，未能复制到剪贴板。");
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            VTSCore.Common.WindowFrontor.Instance.FrontWindow();
        }
    }
}
