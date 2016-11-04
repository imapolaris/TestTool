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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VTSCore.Layers;

namespace TestTool
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
        public MainWindow()
		{
            LogService.InfoFormat("软件启动……");
            Application.Current.DispatcherUnhandledException += Application_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            VTSCore.Common.WindowFrontor.Instance.PropertyChanged+=frontWindowPropertyChanged;
		}
        ILog LogService { get { return LogManager.GetLogger(GetType()); } }

        private void frontWindowPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.Focus();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = e.ExceptionObject as Exception;
                string errorMsg = "非WPF窗体线程异常 : \n\n";
                LogService.Error(errorMsg + ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(errorMsg + ex.Message);
            }
            catch
            {
                LogService.Error("不可恢复的WPF窗体线程异常，应用程序将退出！");
                MessageBox.Show("不可恢复的WPF窗体线程异常，应用程序将退出！");
            }  
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = e.Exception;
                string errorMsg = "WPF窗体线程异常 : \n\n";
                LogService.Error(errorMsg + ex.Message + Environment.NewLine + ex.StackTrace);
                LogService.Error(errorMsg + ex.Message);
            }
            catch
            {
                LogService.Error("不可恢复的WPF窗体线程异常，应用程序将退出！");
                MessageBox.Show("不可恢复的WPF窗体线程异常，应用程序将退出！");
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            layer.SaveIfChanged();
            if (MessageBox.Show("确认退出软件？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) != MessageBoxResult.Yes)
                e.Cancel = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            layer.Dispose();
            LogService.InfoFormat("软件退出。");
            killProcess();
        }
        void killProcess()
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
	}
}
