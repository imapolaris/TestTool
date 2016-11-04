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
using VTSCore.Layers.Common;

namespace VTSCore.Layers.Base
{
    /// <summary>
    /// ConfigConnectionSettingClient.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigConnectionSettingClient : Window
    {
        ConfigConnection _config = new ConfigConnection();
        public ConfigConnection Config
        {
            get { return _config; }
        }
        ILog LogService { get { return LogManager.GetLogger(GetType()); } }

        List<ListViewUnitValue> _unitList;
        public ConfigConnectionSettingClient(string windowName, ConfigConnection config = null)
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.Title = windowName;
            InitConfig(config);
        }

        public void InitConfig(ConfigConnection config)
        {
            if(config != null)
            {
                _config.Ip = config.Ip;
                _config.Port = config.Port;
                _config.RpcEndPoint = config.RpcEndPoint;
            }
            loadRadarInfo();
        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = 0;
                _config.Ip = _unitList[index++].Value;
                _config.Port = int.Parse(_unitList[index++].Value);
                _config.RpcEndPoint = _unitList[index++].Value;
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                LogService.Error(ex.ToString());
                MessageBox.Show(ex.Message);
            }
        }

        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void loadRadarInfo()
        {
            _unitList = new List<ListViewUnitValue>();
            _unitList.Add(new ListViewUnitValue() { Heading = "IP", Value = _config.Ip });
            _unitList.Add(new ListViewUnitValue() { Heading = "端口", Value = _config.Port.ToString() });
            _unitList.Add(new ListViewUnitValue() { Heading = "RpcEndPoint", Value = _config.RpcEndPoint });
            updateClientData();
        }

        private void updateClientData()
        {
            radarListView.Items.Clear();
            for (int i = 0; i < _unitList.Count; i++)
            {
                radarListView.Items.Add(_unitList[i]);
            }
        }
    }
}