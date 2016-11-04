using Common.Logging;
using Seecool.Radar.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using VTSCore.Data.Common;
using VTSCore.Layers.Maps;

namespace VTSCore.Layers.Plotting
{
    class PlottingAreaList : Canvas
    {
        List<PlottingArea> _plottingAreas = new List<PlottingArea>();
        public List<PlottingArea> PlottingAreas { get { return _plottingAreas; } }
        public PlottingArea PlotPres { get; set; }
        int _presIndex = -1;

        ILog LogService { get { return LogManager.GetLogger(GetType()); } }
        public PlottingAreaList()
        {
            string path = System.IO.Directory.GetParent(System.Windows.Forms.Application.LocalUserAppDataPath).FullName;
            _configPath = System.IO.Path.Combine(path, "RadarAreaColorConfig.xml");
            resetMatchColorInfomation();
        }
        public int SelectedIndex
        {
            get { return _presIndex; }
            set
            {
                if (_presIndex != value)
                {
                    LockPrevPlottingStatus();
                    _presIndex = value;
                    if (_presIndex >= 0 && _presIndex < Count)
                    {
                        PlotPres = _plottingAreas[value];
                        if (PlotPres.Count > 2)
                            PlotPres.PlotStatus = PlottingStatus.编辑模式;
                        else
                            PlotPres.PlotStatus = PlottingStatus.新建模式;
                    }
                    else
                        PlotPres = null;
                }
            }
        }

        public int Count { get { return _plottingAreas.Count; } }

        public void Add(PlottingArea pa)
        {
            LockPrevPlottingStatus();
            _plottingAreas.Add(pa);
            _presIndex = Count - 1;
            PlotPres = _plottingAreas[Count - 1];
            this.Children.Add(_plottingAreas[Count - 1]);
            updateRadarAreaShow(_plottingAreas[Count - 1]);
        }

        public void RemoveAt(int index)
        {
            _presIndex = -1;
            this.Children.RemoveAt(index);

            _plottingAreas.RemoveAt(index);
            PlotPres = null;
        }

        private void LockPrevPlottingStatus()
        {
            if (PlotPres != null)
                PlotPres.PlotStatus = PlottingStatus.锁定模式;
        }
        public void OnRefreshed(PointD position)
        {
            for (int i = 0; i < Count; i++)
            {
                if((IsMask && _plottingAreas[i].RadarArea.IsMask) || (ManualExtract && _plottingAreas[i].RadarArea.ManualIdenfity) || (SimuVoyage && _plottingAreas[i].RadarArea.PassThrough))
                    _plottingAreas[i].OnRefreshed();
            }
            if (PlotPres != null)
                PlotPres.RevisePointData(position, PlotPres.SelectedIndex);
        }

        public bool HaveSelected
        {
            get { return PlotPres != null; }
        }

        #region 设置特殊区域显示条件

        public bool IsMask
        {
            get { return _radarAreaColors[0].IsVisible; }
            set
            {
                if (IsMask != value)
                {
                    _radarAreaColors[0].IsVisible = value;
                    updateRadarAreasShow();
                }
            }
        }

        public bool ManualExtract
        {
            get { return _radarAreaColors[1].IsVisible; }
            set
            {
                if (ManualExtract != value)
                {
                    _radarAreaColors[1].IsVisible = value;
                    updateRadarAreasShow();
                }
            }
        }

        public bool SimuVoyage
        {
            get { return _radarAreaColors[2].IsVisible; }
            set
            {
                if (SimuVoyage != value)
                {
                    _radarAreaColors[2].IsVisible = value;
                    updateRadarAreasShow();
                }
            }
        }

        public void ResetPlotPresType(bool isMask, bool manualExtract, bool simuVoyage)
        {
            if (PlotPres != null)
            {
                PlotPres.RadarArea.IsMask = isMask;
                PlotPres.RadarArea.ManualIdenfity = manualExtract;
                PlotPres.RadarArea.PassThrough = simuVoyage;
                updateRadarAreaShow(PlotPres);
            }
        }

        void updateRadarAreasShow()
        {
            if (!_isResetRadarAreasInfo)
                saveMatchColorInfomation();
            for (int i = 0; i < _plottingAreas.Count; i++)
                updateRadarAreaShow(_plottingAreas[i]);
        }

        bool _isResetRadarAreasInfo = false;

        private void updateRadarAreaShow(PlottingArea radarArea)
        {
            if ((IsMask && radarArea.RadarArea.IsMask) || (ManualExtract && radarArea.RadarArea.ManualIdenfity) || (SimuVoyage && radarArea.RadarArea.PassThrough))
            {
                radarArea.OnRefreshed();
                RadarAreaMaskColor colorMuxer = new RadarAreaMaskColor()
                {
                    Opacity = 0,
                    FillColor = (Color)ColorConverter.ConvertFromString("#00000000"),
                    StrokeColor = (Color)ColorConverter.ConvertFromString("#00000000"),
                };
                
                if (IsMask && radarArea.RadarArea.IsMask)
                {
                    updateMuxerColor(ref colorMuxer, _radarAreaColors[0]);
                }
                if(ManualExtract && radarArea.RadarArea.ManualIdenfity)
                {
                    updateMuxerColor(ref colorMuxer, _radarAreaColors[1]);
                }
                if(SimuVoyage && radarArea.RadarArea.PassThrough)
                {
                    updateMuxerColor(ref colorMuxer, _radarAreaColors[2]);
                }
                radarArea.PolygonOpacity = colorMuxer.Opacity;
                radarArea.FillColor = colorMuxer.FillColor;
                radarArea.StrokeColor = colorMuxer.StrokeColor;
            }
            else
                radarArea.Visibility = System.Windows.Visibility.Hidden;
        }

        #endregion

        private void updateMuxerColor(ref RadarAreaMaskColor muxer, RadarAreaMaskColor resourse)
        {
            muxer.Opacity = Math.Max(muxer.Opacity, resourse.Opacity);
            muxer.FillColor = Color.Add(muxer.FillColor, resourse.FillColor);
            muxer.StrokeColor = Color.Add(muxer.StrokeColor, resourse.StrokeColor);
        }

        string _configPath;
        RadarAreaMaskColor[] _radarAreaColors = new RadarAreaMaskColor[3];
        private void resetMatchColorInfomation()
        {
            _isResetRadarAreasInfo = true;
            try
            {
                initMatchColorInfomation();
                var config = ConfigFile<ConfigRadarAreaColor[]>.FromFile(_configPath);
                if (config == null)
                    saveMatchColorInfomation();
                else
                {
                    for (int i = 0; i < config.Length; i++)
                    {
                        for (int j = 0; j < _radarAreaColors.Length; j++)
                        {
                            if (config[i].Heading == _radarAreaColors[j].Heading)
                            {
                                _radarAreaColors[j] = new RadarAreaMaskColor(config[i]);
                                break;
                            }
                        }
                    }
                    if (_radarAreaColors.Length >= 2)
                    {
                        IsMask = _radarAreaColors[0].IsVisible;
                        ManualExtract = _radarAreaColors[1].IsVisible;
                        SimuVoyage = _radarAreaColors[2].IsVisible;
                    }
                }
            }
            catch (Exception ex)
            {
                string stringShow = "在读取雷达特殊区域时，出错，文件地址：" + _configPath + Environment.NewLine;
                LogService.WarnFormat(stringShow + ex.ToString());
                System.Windows.MessageBox.Show(stringShow + ex.Message);
            }
            _isResetRadarAreasInfo = false;
        }

        private void saveMatchColorInfomation()
        {
            try
            {
                var config = new ConfigRadarAreaColor[_radarAreaColors.Length];
                for (int i = 0; i < _radarAreaColors.Length; i++)
                {
                    config[i] = new ConfigRadarAreaColor()
                    {
                        Heading = _radarAreaColors[i].Heading,
                        Opacity = _radarAreaColors[i].Opacity,
                        FillColor = _radarAreaColors[i].FillColor.ToString(),
                        StrokeColor = _radarAreaColors[i].StrokeColor.ToString(),
                        IsVisible = _radarAreaColors[i].IsVisible
                    };
                }
                ConfigFile<ConfigRadarAreaColor[]>.SaveToFile(_configPath, config);
            }
            catch(Exception ex)
            {
                string stringShow = "在写入雷达特殊区域时出错!" + Environment.NewLine;
                LogService.Error(stringShow + ex.ToString());
                System.Windows.MessageBox.Show(stringShow + ex.Message);
            }
        }

        private void initMatchColorInfomation()
        {
            _radarAreaColors[0] = new RadarAreaMaskColor() { Heading = "屏蔽区" };
            _radarAreaColors[1] = new RadarAreaMaskColor()
            {
                Heading = "非自动录取区",
                FillColor = (Color)ColorConverter.ConvertFromString("#FF009F00"),
                StrokeColor = (Color)ColorConverter.ConvertFromString("#FF00009F")
            };
            _radarAreaColors[2] = new RadarAreaMaskColor()
            {
                Heading = "模拟航行区",
                FillColor = (Color)ColorConverter.ConvertFromString("#FF0000FF"),
                StrokeColor = (Color)ColorConverter.ConvertFromString("#FFFF0000")
            };
        }
    }
}