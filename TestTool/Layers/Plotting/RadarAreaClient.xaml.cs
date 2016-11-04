using Common.Logging;
using GISV4Immigration;
using Seecool.Radar;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using VTSCore.Layers.Base;
using VTSCore.Layers.Radar;

namespace VTSCore.Layers.Plotting
{
    public class AreaTypeShowObj
    {
        public string TypeName { get; set; }
        public bool IsChecked { get; set; }
    }

    /// <summary>
    /// RadarAreaClient.xaml 的交互逻辑
    /// </summary>
    public partial class RadarAreaClient : Window
    {
        PlottingAreaSettingInfomation _radarAreas;
        List<PlottingArea> _plottingAreaList;
        List<PlottingNameObj> _plottingAreaNameList = new List<PlottingNameObj>();

        string ConfigPath;
        ILog LogService { get { return LogManager.GetLogger(GetType()); } }
        public RadarAreaClient()
        {
            InitializeComponent();
            string path = System.IO.Directory.GetParent(System.Windows.Forms.Application.LocalUserAppDataPath).FullName;
            ConfigPath = System.IO.Path.Combine(path, "雷达区域数据库默认配置.xml");

            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            _radarAreas = PlottingAreaSettingInfomation.Instance;
            _radarsInfo = RadarsSettingInfo.Instance;

            _plottingAreaList = _radarAreas.Data.PlottingAreas;
            _radarAreas.PropertyChanged += _radarAreas_PropertyChanged;

            _radarsInfo.PropertyChanged += radarInfo_PropertyChanged;
            initListView();
            radarAreasListView.ToolTip = "双击修改当前雷达区域名称," + Environment.NewLine + "右键菜单可进行居中显示等操作";
            btSave.ToolTip = "将当前所有配置中雷达区域保存" + Environment.NewLine + "已关联雷达的区域上传至雷达服务器，" + Environment.NewLine + "未关联区域保存至本地";
        }

        public void SelectedIndex(int index)
        {
            _radarAreas.SelectedIndex = index;
            radarAreasListView.SelectedIndex = index;
            var lvap = new ListViewAutomationPeer(radarAreasListView);
            var svap = lvap.GetPattern(PatternInterface.Scroll) as ScrollViewerAutomationPeer;
             //////////////////////////此处添加你想要对TreeView自身滚动条的操作///////////////////////////
            (svap.Owner as ScrollViewer).ScrollToVerticalOffset(index);    //向下调节垂直滚动条的位置;
        }

        private void radarAreasListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            resetSelectedIndex(radarAreasListView.SelectedIndex);
            loadAreaTypeShow();
            loadRadarInfo();
        }

        void _radarAreas_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PresPlottingName")
                updatePresPlottingName();
            else if (e.PropertyName == "特殊区域关联修改")
            {
                if (radarAreasListView.SelectedIndex >= 0)
                    _plottingAreaNameList[radarAreasListView.SelectedIndex].Source = _plottingAreaList[radarAreasListView.SelectedIndex].DataSource;
            }
                //updateRadarInfo();
        }

        private void initListView()
        {
            int selectedIndex = radarAreasListView.SelectedIndex;
            _plottingAreaNameList.Clear();
            radarAreasListView.Items.Clear();
            for (int i = 0; i < _plottingAreaList.Count; i++)
            {
                _plottingAreaNameList.Add(new PlottingNameObj() { Name = _plottingAreaList[i].RadarArea.Name, Source = _plottingAreaList[i].DataSource });
                radarAreasListView.Items.Add(_plottingAreaNameList[i]);
            }
            radarAreasListView.SelectedIndex = selectedIndex;
            loadRadarInfo();
        }

        void updatePresPlottingName()
        {
            if (_plottingAreaNameList.Count == _plottingAreaList.Count && _radarAreas.SelectedIndex >= 0)
            {
                _plottingAreaNameList[_radarAreas.SelectedIndex].Name = _plottingAreaList[_radarAreas.SelectedIndex].RadarArea.Name;
                _plottingAreaNameList[_radarAreas.SelectedIndex].Source = _plottingAreaList[_radarAreas.SelectedIndex].DataSource;
            }
        }

        private void resetSelectedIndex(int index)
        {
            _radarAreas.SelectedIndex = index;
        }

        #region 雷达区域设置模块

        private void centeredMenu_Click(object sender, RoutedEventArgs e)
        {
            centeredRadarArea();
        }
        private void renamingMenu_Click(object sender, MouseButtonEventArgs e)
        {
            object item = ListViewBaseInfo.GetElementFromPoint((ItemsControl)sender, e.GetPosition((ItemsControl)sender));
            if (item != null)
                renamingRadarArea();
        }

        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            addNewRadarArea();
            initListView();
        }

        private void btDelete_Click(object sender, RoutedEventArgs e)
        {
            _radarAreas.RemoveAt(radarAreasListView.SelectedIndex);
            initListView();
        }

        private void renamingMenu_Click(object sender, RoutedEventArgs e)
        {
            renamingRadarArea();
        }

        private void addNewRadarArea(string nameDefault = "")
        {
            try
            {
                var winAdd = new VTSCore.Layers.Base.ModifyValueClient("新增雷达区域名称设置", nameDefault);
                if (winAdd.ShowDialog().Value)
                {
                    if (_radarAreas.IsEffectiveName(winAdd.Heading))
                    {
                        RadarRegion region = new RadarRegion() { Name = winAdd.Heading, IsMask = true };
                        importNewRadarArea(region);
                        initListView();
                        SelectedIndex(radarAreasListView.Items.Count - 1);
                    }
                    else
                        addNewRadarArea(winAdd.Heading);
                }
            }
            catch(Exception ex)
            {
                LogService.Error("导入区域错误!" + Environment.NewLine + ex.ToString());
                MessageBox.Show("导入区域错误!" + Environment.NewLine + ex.Message);
            }
        }

        private void importNewRadarArea(RadarRegion region)
        {
            addRadarRegion(region);
            linkedAllRadar();
        }

        private void linkedAllRadar()
        {
            foreach (var radar in _radarsInfo.Radars)
                _radarsInfo.GetRadarIndexFromName(radar.RadarName);
        }

        private void addRadarRegion(RadarRegion region)
        {
            var plotArea = new PlottingArea(_radarAreas.Locator, region);
            _radarAreas.Add(plotArea);
        }

        private void centeredRadarArea()
        {
            if (radarAreasListView.SelectedIndex >= 0 && _radarAreas.PlotPres != null)
            {
                if (!_radarAreas.PlotPres.MoveToCentered())//居中，检查居中是否失败，失败则提示。
                    MessageBox.Show("居中失败，请检查是否配置雷达区域点");
            }
        }

        private void renamingRadarArea()
        {
            if (radarAreasListView.SelectedIndex >= 0)
            {
                var winAdd = new VTSCore.Layers.Base.ModifyValueClient("雷达区域重命名", _radarAreas.PresPlottingName);
                if (winAdd.ShowDialog().Value)
                {
                    if (_radarAreas.PresPlottingName != winAdd.Heading)
                    {
                        if (_radarAreas.IsEffectiveName(winAdd.Heading))
                            _radarAreas.PresPlottingName = winAdd.Heading;
                    }
                }
            }
        }
        #endregion

        #region 雷达部分

        RadarsSettingInfo _radarsInfo;
        List<LinkingRadarObj> _radarsLinkInfo = new List<LinkingRadarObj>();
        private void LinkRadarCheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            int uid = Convert.ToInt32(cb.Tag.ToString()); //获取该行id 
            if(!_radarsInfo.Radars[uid].IsEnable)
            {
                if (cb.IsChecked.Value)
                {
                    loadRadarInfo();
                    string name = _radarsInfo.Radars[uid].RadarName;
                    if (string.IsNullOrWhiteSpace(name))
                        name = _radarsInfo.Radars[uid].RadarAddressEve.RpcEndPoint;
                    MessageBox.Show("雷达\"" + name + "\"未启用! 关联失败！", "警告", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                }
                return;
            }
            _radarAreas.UpdateRadarRegionLink(_radarsInfo.Radars[uid].RadarName, cb.IsChecked.Value);
        }
        
        private void radarInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AddRadar" || e.PropertyName == "RemoveRadar" || e.PropertyName == "ReadAllConfig" || e.PropertyName == "RadarEnableChanged")
                loadRadarInfo();
        }

        void loadRadarInfo()
        {
            int selectedIndex = radarsListView.SelectedIndex;
            _radarsLinkInfo.Clear();
            radarsListView.Items.Clear();
            for (int i = 0; i < _radarsInfo.Radars.Length; i++)
            {
                bool isLinking = false;
                if (_radarAreas.PlotPres != null && _radarAreas.PlotPres.IsLinking(_radarsInfo.Radars[i].RadarName))
                    isLinking = true;

                _radarsLinkInfo.Add(new LinkingRadarObj() { Uid = i, Heading = _radarsInfo.Radars[i].RadarName, IsLinking = isLinking, IsEnabled = _radarsInfo.Radars[i].IsEnable });
                radarsListView.Items.Add(_radarsLinkInfo[i]);
            }
            if (selectedIndex >= 0)
                radarsListView.SelectedIndex = selectedIndex;
        }

        #endregion

        #region 雷达区域类型

        List<AreaTypeShowObj> _areaType = new List<AreaTypeShowObj>();

        private void AllAreaTypeCheckBox_Click(object sender, RoutedEventArgs e)
        {
            updateAllRadarAreaType((sender as CheckBox).IsChecked.Value);
        }

        private void AreaTypeCheckBox_Click(object sender, RoutedEventArgs e)
        {
            updateRadarAreaType();
        }

        void loadAreaTypeShow()
        {
            _areaType = new List<AreaTypeShowObj>();
            radarAreaShowListView.Items.Clear();
            if(radarAreasListView.SelectedIndex >= 0)
            {
                var area = _plottingAreaList[radarAreasListView.SelectedIndex];
                _areaType.Add(new AreaTypeShowObj() { IsChecked = area.RadarArea.IsMask, TypeName = "屏蔽区" });
                _areaType.Add(new AreaTypeShowObj() { IsChecked = area.RadarArea.ManualIdenfity, TypeName = "非自动录取区" });
                _areaType.Add(new AreaTypeShowObj() { IsChecked = area.RadarArea.PassThrough, TypeName = "模拟航行区" });
            }
            for(int i = 0; i < _areaType.Count; i++)
            {
                radarAreaShowListView.Items.Add(_areaType[i]);
            }
        }
        
        private void updateRadarAreaType()
        {
            if (radarAreasListView.SelectedIndex >= 0)
                _radarAreas.SetPlotPresType(_areaType[0].IsChecked, _areaType[1].IsChecked, _areaType[2].IsChecked);
        }

        private void updateAllRadarAreaType(bool value)
        {
            for (int i = 0; i < _areaType.Count; i++)
            {
                _areaType[i].IsChecked = value;
            }
            updateRadarAreaType();
            loadAreaTypeShow();
        }

        #endregion

        private void Window_Closed(object sender, EventArgs e)
        {
            _radarAreas.SelectedIndex = -1;
            VTSCore.Common.WindowFrontor.Instance.FrontWindow();
        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            _radarAreas.SaveRadarAreaData();
        }

        private void btReset_Click(object sender, RoutedEventArgs e)
        {
            _radarAreas.Clear();
            foreach(var radar in _radarsInfo.Radars)
            {
                radar.ResetRadarRegions();
            }
            _radarAreas.InputLocalRadarAreaData();
            initListView();
        }

        private void btImportFromData_Click(object sender, RoutedEventArgs e)
        {
            importFromData();
        }

        private void btImportFromGISV4LDZBQDb_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string dataBaseString = getDefaultDataBasePath();
                ImportAreaByDataBaseClient client = new ImportAreaByDataBaseClient("专题数据库导入", dataBaseString);
                if (client.ShowDialog().Value)
                {
                    VTSCore.Data.Common.ConfigFile<string>.SaveToFile(ConfigPath, client.Data);
                    RadarRegionInfo[] infos = Immigrate.LoadRadarRegions(client.Data);
                    addRadarRegions(infos);
                    initListView();
                    MessageBox.Show(string.Format("成功导入{0}个区域！", infos.Length));
                }
            }
            catch (Exception ex)
            {
                LogService.Warn(ex.ToString());
                MessageBox.Show("请输入有效的信息!!" + Environment.NewLine + "例：Server=127.0.0.1;Database=GISV4_SUBJECT;User Id=sa;Pwd=sa" + Environment.NewLine + ex.Message);
            }
        }

        private void addRadarRegions(RadarRegionInfo[] regions)
        {
            foreach (var region in regions)
            {
                while (!_radarAreas.IsCanAddName(region.Region.Name))
                    region.Region.Name += '*';
                addRadarRegion(region.Region);
                if (region.AllRadar)
                    linkedAllRadar();
                else
                    linkRadarRegionToRadar(region.RelateRadar);
            }
        }

        string getDefaultDataBasePath()
        {
            string path = VTSCore.Data.Common.ConfigFile<string>.FromFile(ConfigPath);
            if (path == null)
                path = "Server=127.0.0.1;Database=GISV4_SUBJECT;User Id=sa;Pwd=sa";
            return path;
        }
        ImportAreaByStringClient _importAreaByStringClient;
        private void importFromData()
        {
            try
            {
                if (!VTSCore.Common.WindowStateDetect.ShowWindow(_importAreaByStringClient))
                {
                    _importAreaByStringClient = new ImportAreaByStringClient();
                    _importAreaByStringClient.Show();
                    System.Windows.Interop.HwndSource winformWindow = (System.Windows.Interop.HwndSource.FromDependencyObject(this) as System.Windows.Interop.HwndSource);
                    if (winformWindow != null)
                        new System.Windows.Interop.WindowInteropHelper(_importAreaByStringClient) { Owner = winformWindow.Handle };
                    _importAreaByStringClient.OnSaving += OnSavingByString;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("请输入需要导入的数据，并用','号隔开!!（区域名称,经度,纬度,经度,纬度,…）" + Environment.NewLine + ex.Message);
            }
        }

        private void OnSavingByString()
        {
            if(_importAreaByStringClient != null)
            {
                RadarRegion region = new RadarRegion() { Name = _importAreaByStringClient.Heading, Polygon = _importAreaByStringClient.Polygon, IsMask = true };
                importNewRadarArea(region);
                initListView();
                SelectedIndex(radarAreasListView.Items.Count - 1);
                centeredRadarArea();
                _importAreaByStringClient.OnSaving -= OnSavingByString;
                _importAreaByStringClient.Close();
                _importAreaByStringClient = null;
            }
        }

        private void btExportXml_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog file = new System.Windows.Forms.OpenFileDialog();
            file.Filter = "xml文件|*.xml|所有文件|*.*";
            file.RestoreDirectory = true;
            if (file.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var regions = VTSCore.Data.Common.ConfigFile<RegionAndRadars[]>.FromFile(file.FileName);
                foreach (var region in regions)
                    addRadarRegions(region);
                initListView();
                MessageBox.Show(string.Format("成功导入{0}个区域！", regions.Length));
            }
        }

        private void btImportXml_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog file = new System.Windows.Forms.SaveFileDialog();
            file.Filter = "xml文件|*.xml|所有文件|*.*";
            file.RestoreDirectory = true;
            if (file.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                List<RegionAndRadars> regions = new List<RegionAndRadars>();
                foreach (var area in _plottingAreaList)
                {
                    List<string> radarnames = new List<string>();
                    for (int i = 0; i < _radarsInfo.Radars.Length; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(_radarsInfo.Radars[i].RadarName) && area.IsLinking(_radarsInfo.Radars[i].RadarName))
                        {
                            radarnames.Add(_radarsInfo.Radars[i].RadarName);
                        }
                    }
                    regions.Add(new RegionAndRadars(area.RadarArea,radarnames.ToArray()));
                }
                VTSCore.Data.Common.ConfigFile<RegionAndRadars[]>.SaveToFile(file.FileName, regions.ToArray());
                MessageBox.Show(string.Format("成功导出{0}个区域！", regions.Count));
            }
        }
        private void addRadarRegions(RegionAndRadars region)
        {
            while (!_radarAreas.IsCanAddName(region.Name))
                region.Name += '*';
            addRadarRegion(region);
            foreach (var name in region.RadarNames)
                linkRadarRegionToRadar(name);
        }

        void linkRadarRegionToRadar(string radarName)
        {
            int index = _radarsInfo.GetRadarIndexFromName(radarName);
            if (index >= 0 && _radarsInfo.Radars[index].IsEnable)
                _radarAreas.UpdateRadarRegionLink(radarName, true);
        }
    }
}
