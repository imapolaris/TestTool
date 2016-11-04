using Seecool.Radar;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using VTSCore.Layers.Maps;

namespace VTSCore.Layers.Plotting
{
    class PlottingAreaSettingInfomation : INotifyPropertyChanged
    {
        public static PlottingAreaSettingInfomation Instance { get; private set; }

        static PlottingAreaSettingInfomation()
        {
            Instance = new PlottingAreaSettingInfomation();
        }

        private PlottingAreaSettingInfomation()
        {
            Data = new PlottingAreaList();
            _localRegions = LocalRegions.Instance;
        }
        LocalRegions _localRegions;
        public PlottingAreaList Data;
        public ILocator Locator { get; set; }

        public int SelectedIndex
        {
            get { return Data.SelectedIndex; }
            set
            {
                Data.SelectedIndex = value;
                FirePropertyChanged("重置标绘区选择项");
            }
        }

        public void Add(PlottingArea pa)
        {
            Data.Add(pa);
            SelectedIndex = Data.Count - 1;
            FirePropertyChanged("新增标绘区");
        }

        public void RemoveAt(int index)
        {
            if (index >= 0 && index < Data.Count)
            {
                Data.RemoveAt(index);
                FirePropertyChanged("删除标绘区");
            }
        }

        public void UpdateRadarAreaList()
        {
            FirePropertyChanged("UpdateRadarAreaList");
        }

        public void UpdateRadarRegions(string radarName, RadarRegion[] regions)
        {
            if (string.IsNullOrWhiteSpace(radarName))
                return;
            lock (Data)
            {
                foreach (RadarRegion region in regions)
                {
                    updateRegion(radarName, region);
                }
            }
        }

        private void updateRegion(string radarName, RadarRegion region)
        {
            for (int i = 0; i < Data.Count; i++)
            {
                if (Data.PlottingAreas[i].RadarArea.Name == region.Name)
                {
                    Data.PlottingAreas[i].AddConfigRadar(radarName);//关联
                    return;
                }
            }
            //未找到
            int selectedIndex = SelectedIndex;
            var plotArea = new PlottingArea(Locator, region);
            plotArea.AddConfigRadar(radarName);
            Add(plotArea);
            SelectedIndex = selectedIndex;
        }
        public string LastName { get; private set; }
        public string PresPlottingName
        {
            get
            {
                if (Data.HaveSelected)
                    return Data.PlotPres.RadarArea.Name;
                else
                    return null;
            }
            set
            {
                if (Data.HaveSelected && Data.PlotPres.RadarArea.Name != value)
                {
                    LastName = Data.PlotPres.RadarArea.Name;
                    Data.PlotPres.RadarArea.Name = value;
                    Data.PlotPres.UpdateToolTip();
                    FirePropertyChanged("PresPlottingName");
                }
            }
        }

        public PlottingStatus PlotStatus
        {
            get
            {
                if (PlotPres != null)
                    return Data.PlotPres.PlotStatus;
                else
                    return PlottingStatus.锁定模式;
            }
            set
            {
                if (Data.HaveSelected && Data.PlotPres.PlotStatus != value)
                {
                    var lastStatus = Data.PlotPres.PlotStatus;
                    Data.PlotPres.PlotStatus = value;
                    FirePropertyChanged("修改当前标绘区状态");
                }
            }
        }

        public void SaveRadarAreaData()
        {
            SaveInvalidRadarAreaData();
            FirePropertyChanged("更新雷达区域");
        }

        public bool IsMask
        {
            get { return Data.IsMask; }
            set { Data.IsMask = value; }
        }

        public bool ManualExtract
        {
            get { return Data.ManualExtract; }
            set { Data.ManualExtract = value; }
        }

        public bool SimuVoyage
        {
            get { return Data.SimuVoyage; }
            set { Data.SimuVoyage = value; }
        }

        public void SetPlotPresType(bool isMask, bool manualExtract, bool simuVoyage)
        {
            Data.ResetPlotPresType(isMask, manualExtract, simuVoyage);
            FirePropertyChanged("特殊区域类型修改");
        }

        public void ChangedRadarName(string lastName, string newName)
        {
            if (string.IsNullOrWhiteSpace(lastName))
                return;
            foreach (var area in Data.PlottingAreas)
                area.ChangedRadarName(lastName, newName);
        }

        public void UpdateRadarRegionLink(string radarName, bool linkStatus)
        {
            if (PlotPres != null && !string.IsNullOrWhiteSpace(radarName))
            {
                bool updateStatus = false;
                if (linkStatus)
                    updateStatus = PlotPres.AddConfigRadar(radarName);
                else
                    updateStatus = PlotPres.RemoveConfigRadar(radarName);
                if (updateStatus)
                    FirePropertyChanged("特殊区域关联修改");
            }
        }
        
        public bool IsCanAddName(string name)
        {
            return GetIndexFromName(name) == -2;
        }

        public bool IsEffectiveName(string name)
        {
            int invalidInt = GetIndexFromName(name);
            if (invalidInt == -2)
                return true;
            if (invalidInt == -1)
                System.Windows.MessageBox.Show("添加区域失败，名称不能为空或空格！");
            else if (invalidInt >= 0)
                System.Windows.MessageBox.Show("添加区域失败，该名称已存在！");
            return false;
        }
        /// <summary>
        /// 检查名称所在位置 
        /// （-2为未查找到且可增加，-1为无效名称,>=0则为名称所在序号）
        /// </summary>
        /// <param radarName="radarName">区域名称</param>
        /// <returns> 
        /// -2为未查找到且可增加该名称命名的区域
        /// -1为无效名称
        /// >=0则为名称所在序号
        /// </returns>
        public int GetIndexFromName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return -1;
            for (int i = 0; i < Data.PlottingAreas.Count; i++)
            {
                var radar = Data.PlottingAreas[i];
                if (radar.RadarArea.Name == name)
                    return i;
            }
            return -2;
        }

        public PlottingArea PlotPres { get { return Data.PlotPres; } }

        public void Clear()
        {
            while (Data.Count > 0)
                Data.RemoveAt(0);
        }

        public RadarRegion[] GetRadarRegionsFromName(string radarName)
        {
            List<RadarRegion> regions = new List<RadarRegion>();
            foreach (var area in Data.PlottingAreas)
            {
                if (area.IsLinking(radarName))
                    regions.Add(area.RadarArea);
            }
            return regions.ToArray();
        }

        public void InputLocalRadarAreaData()
        {
            var regions = _localRegions.Regions;
            foreach(var region in regions)
            {
                if (IsCanAddName(region.Name))
                    Add(new PlottingArea(Locator, region));
            }
            SelectedIndex = -1;
        }

        public void SaveInvalidRadarAreaData()
        {
            _localRegions.Regions = getNotAnyLinkedRadarRegions();
        }

        public List<string> GetRadarAreasName(System.Windows.Point pt)
        {
            List<string> names = new List<string>();
            foreach (var data in Data.PlottingAreas)
            {
                if (data.FillContains(pt))
                    names.Add(data.RadarArea.Name);
            }
            return names;
        }
        /// <summary>
        /// 若存在“无任何关联的雷达区域”发生改变，则提示用户保存
        /// </summary>
        public void SetRegionsIfChanged()
        {
            saveLocalRegionsIfChanged();
            FirePropertyChanged("SetRegionsIfChanged");
        }

        private void saveLocalRegionsIfChanged()
        {
            var userRegions = getNotAnyLinkedRadarRegions();
            if (!RadarRegionsInfo.AreEqual(userRegions, _localRegions.Regions))
            {
                DialogResult msgBox = System.Windows.Forms.MessageBox.Show("本地雷达区域参数发生变更,是否保存?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                if (msgBox == DialogResult.Yes)
                    _localRegions.Regions = userRegions;
            }
        }
        
        private RadarRegion[] getNotAnyLinkedRadarRegions()
        {
            List<RadarRegion> regions = new List<RadarRegion>();
            foreach (var region in Data.PlottingAreas)
            {
                if (region.IsNotLinkedAnyRadar)
                    regions.Add(region.RadarArea);
            }
            return regions.ToArray();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void FirePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}