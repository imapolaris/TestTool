using Seecool.Radar;
using Seecool.Radar.Unit;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using VTSCore.Layers.Maps;
using VTSCore.Layers.Radar;
using VTSCore.Layers.Tracks;

namespace VTSCore.Layers.Plotting
{
    public enum PlottingStatus
    {
        新建模式,
        编辑模式,
        锁定模式
    }

    /// <summary>
    /// 标绘区域设置模块
    /// </summary>
    public class PlottingArea : Canvas
    {
        public RadarRegion RadarArea = new RadarRegion();
        List<PointD> _mapPoints;
        Polygon _polygon;
        List<Ellipse> _ellipses;
        ILocator _locator;

        public PlottingArea(ILocator locator, RadarRegion region)
        {
            _locator = locator;
            _plottingStatus = PlottingStatus.新建模式;
            _mapPoints = new List<PointD>();
            _ellipses = new List<Ellipse>();
            NewPolygon();
            initAllBorder();
            this.ToolTip = "雷达区域";
            initFronRegion(region);
        }

        public int Count { get { return _mapPoints.Count; } }

        public void UpdateToolTip()
        {
            this.ToolTip = RadarArea.Name;
        }

        int _selectedIndex = -1;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (_selectedIndex == value)
                    return;
                unselectedPoint(_selectedIndex);
                _selectedIndex = value;
                selectedPoint(_selectedIndex);
            }
        }

        #region 雷达区域工作模式调整

        PlottingStatus _plottingStatus;
        public PlottingStatus PlotStatus
        {
            get { return _plottingStatus; }
            set 
            {
                if(_plottingStatus != value)
                {
                    if (_plottingStatus == PlottingStatus.新建模式)
                        UnEdit();
                    else if (value == PlottingStatus.新建模式)
                        Edit();
                    _plottingStatus = value;
                    Visibility visibility = (PlotStatus == PlottingStatus.锁定模式 ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible);
                    if (_ellipses.Count > 0 && _ellipses[0].Visibility != visibility)
                    {
                        foreach (var elli in _ellipses)
                        {
                            elli.Visibility = visibility;
                        }
                    }
                    if (_plottingStatus != PlottingStatus.编辑模式)
                        SelectedIndex = -1;
                }
            }
        }

        void UnEdit()
        {
            if (Count >= 2)
            {
                removeAt(Count - 1);
                if (_polygon.Points[Count - 1] == _polygon.Points[Count - 2])
                    removeAt(Count - 1);
            }
        }

        private void Edit()
        {
            if (Count >= 1)
                addPointData(_mapPoints[Count - 1], Count);
        }

        #endregion 雷达特殊区域工作模式调整

        #region 特殊区域编辑
        /// <summary>
        /// 按顺序增加经纬度点时更新标绘区域
        /// </summary>
        /// <param video="pointD">新增点经纬度坐标（新增位置默认为最后位置）</param>
        /// <returns></returns>
        public void Push(PointD[] positions)
        {
            if (positions == null)
                return;
            for (int i = 0; i < positions.Length; i++)
            {
                addPointData(positions[i], Count);
            }
            updateAllBorder();
            RadarArea.Polygon = _mapPoints.ToArray();
        }

        public void Push(PointD position)
        {
            if (_plottingStatus != PlottingStatus.新建模式)
                return;
            if (RevisePointData(position, Count - 1) == false)
            {
                addPointData(position, Count);
                this.Visibility = System.Windows.Visibility.Visible;
            }
            addPointData(position, Count);
            RadarArea.Polygon = _mapPoints.ToArray();
            updateBorder(position.X, position.Y);
        }
        /// <summary>
        /// 编辑模式下用于增点操作
        /// </summary>
        /// <param video="position">经纬度坐标</param>
        /// <param video="index">新增点增点位置</param>
        public void Push(PointD position, int index)
        {
            if (_plottingStatus != PlottingStatus.编辑模式)
                return;
            if (IsEffectedSelectedIndex(index))
            {
                addPointData(position, index + 1);
                updateBorder(position.X, position.Y);
            }
        }

        public void RemoveSelectedPoint()
        {
            var lastPosition = _mapPoints[SelectedIndex];
            removeAt(SelectedIndex);
            if (needUpdateBorder(lastPosition.X, lastPosition.Y))
                updateAllBorder();
            SelectedIndex = -1;
        }

        public bool RevisePointData(PointD position, int index)
        {
            if (!IsEffectedSelectedIndex(index))
                return false;
            Point point = _locator.MapToScreen(position.X, position.Y);
            var lastPosition = _mapPoints[index];
            _mapPoints[index] = position;
            RadarArea.Polygon = _mapPoints.ToArray();
            _polygon.Points[index] = point;
            drawEllipse(_ellipses[index], point);
            updateBorder(position.X, position.Y);
            if (needUpdateBorder(lastPosition.X, lastPosition.Y))
                updateAllBorder();
            return true;
        }

        private void removeAt(int index)
        {
            if (IsEffectedSelectedIndex(index))
            {
                _polygon.Points.RemoveAt(index);
                _mapPoints.RemoveAt(index);
                RadarArea.Polygon = _mapPoints.ToArray();
                this.Children.Remove(_ellipses[index]);
                _ellipses.RemoveAt(index);
                RadarArea.Polygon = _mapPoints.ToArray();
            }
        }
        
        public int IndexOnPoints(Point point)
        {
            double distanceInf = 100;
            int index = -1;
            for (int i = 0; i < Count; i++)
            {
                Point pt = _polygon.Points[i];
                double dis = Math.Sqrt((point.X - pt.X) * (point.X - pt.X) + (point.Y - pt.Y) * (point.Y - pt.Y));
                if (dis < distanceInf)
                {
                    distanceInf = dis;
                    index = i;
                }
            }
            if (distanceInf <= 5)
                return index;
            else
                return -1;
        }

        public int IndexOnLines(Point point)
        {
            double distInf = 100;
            int index = -1;
            for (int i = 0; i < Count; i++)
            {
                double dist = offsetPoint(i, (i + 1) % Count, point);
                if (dist < distInf)
                {
                    index = i;
                    distInf = dist;
                }
            }
            if (distInf <= 3)
                return index;
            else
                return -1;
        }

        private bool IsEffectedSelectedIndex(int index)
        {
            return index >= 0 && index < Count;
        }

        void addPointData(PointD position, int index)
        {
            Point point = _locator.MapToScreen(position.X, position.Y);
            Ellipse e = NewEllipse();
            drawEllipse(e, point);
            this.Children.Add(e);
            _mapPoints.Insert(index, position);
            _polygon.Points.Insert(index, point);
            _ellipses.Insert(index, e);
            SelectedIndex = index;
        }

        #endregion 特殊区域编辑

        #region 特殊区域控制

        /// <summary>
        /// 鼠标位置变动，更新标绘区域
        /// </summary>
        /// <param video="pointD">经纬度</param>
        public void UpdateMousePosition(PointD position)
        {
            if (_plottingStatus != PlottingStatus.新建模式)
                return;
            if (Count > 0)
            {
                _polygon.Points[Count - 1] = _locator.MapToScreen(position.X, position.Y);
                drawEllipse(_ellipses[Count - 1], _polygon.Points[Count - 1]);
            }
        }

        public void OnRefreshed()
        {
            if (Count > 0 && InScreenByRectBorder())
            {
                this.Visibility = System.Windows.Visibility.Visible;
                for (int i = 0; i < Count; i++)
                {
                    _polygon.Points[i] = _locator.MapToScreen(_mapPoints[i].X, _mapPoints[i].Y);
                    drawEllipse(_ellipses[i], _polygon.Points[i]);
                }
            }
            else
                this.Visibility = System.Windows.Visibility.Collapsed;
        }

        public bool MoveToCentered()
        {
            if (_lonInf > _lonSup || _latInf > _latSup)
                return false;
            double lon = (_lonInf + _lonSup) / 2;
            double lat = (_latInf + _latSup) / 2;
            double scale = _locator.Scale;
            double screenLon = _locator.MapSize.Width / 60 / Math.Cos(_locator.Center.Lat * Math.PI / 180);
            double screenLat = _locator.MapSize.Height / 60;
            double scaleChanged = Math.Max((_lonSup - _lonInf) / screenLon, (_latSup - _latInf) / screenLat) * 1.125;
            scale *= scaleChanged;
            _locator.Locate(scale, lon, lat);
            return true;
        }

        double _lonInf;
        double _lonSup;
        double _latInf;
        double _latSup;

        private void initAllBorder()
        {
            _lonInf = 181;
            _lonSup = -181;
            _latInf = 91;
            _latSup = -91;
        }

        void updateBorder(double lon, double lat)
        {
            if (_latInf > lat)
                _latInf = lat;
            if (_latSup < lat)
                _latSup = lat;
            if (_lonInf > lon)
                _lonInf = lon;
            if (_lonSup < lon)
                _lonSup = lon;
        }

        bool needUpdateBorder(double lon, double lat)
        {
            if (_latInf < lat && _latSup > lat && _lonInf < lon && _lonSup > lon)
                return false;
            else
                return true;
        }

        void updateAllBorder()
        {
            initAllBorder();
            foreach(var point in _mapPoints)
                updateBorder(point.X, point.Y);
        }

        bool InScreenByRectBorder()
        {
            double radiusLon = _locator.MapSize.Width / 60 / Math.Cos(_locator.Center.Lat * Math.PI / 180) / 2;
            double radiusLat = _locator.MapSize.Height / 60 / 2;
            double screenLeft = _locator.Center.Lon - radiusLon;
            double screenRight = _locator.Center.Lon + radiusLon;
            double screenTop = _locator.Center.Lat + radiusLat;
            double screenBottom = _locator.Center.Lat - radiusLat;
            if (screenLeft >= _lonSup || screenRight <= _lonInf || screenTop <= _latInf || screenBottom >= _latSup)
                return false;
            return true;
        }

        double offsetPoint(int indexBegin, int indexEnd, Point point)
        {
            double dist1 = Distance(_polygon.Points[indexBegin], point);
            double dist2 = Distance(_polygon.Points[indexEnd], point);
            double dist = Distance(_polygon.Points[indexBegin], _polygon.Points[indexEnd]);
            return dist1 + dist2 - dist;
        }

        static double Distance(Point p1, Point p2)
        {
            p2.Offset(-p1.X, -p1.Y);
            return Math.Sqrt((p2.X * p2.X + p2.Y * p2.Y));
        }

        private void drawEllipse(Ellipse e, Point point)
        {
            Canvas.SetLeft(e, point.X - e.Width / 2);
            Canvas.SetTop(e, point.Y - e.Height / 2);
        }

        private void initFronRegion(RadarRegion region)
        {
            if (region == null)
                return;
            PlotStatus = PlottingStatus.编辑模式;
            RadarArea = region;
            UpdateToolTip();
            Push(region.Polygon);
            this.ToolTip = region.Name;

        }

        #endregion 雷达区域控制

        #region 相关联雷达信息

        List<string> _radarNames = new List<string>();
        string[] RadarNames { get { return _radarNames.ToArray(); } }

        public bool IsNotLinkedAnyRadar { get { return _radarNames.Count == 0; } }

        public string DataSource { get { return (RadarNames.Length > 0 ? "雷达" : "本地"); } }

        public bool IsLinking(string radarName)
        {
            return findConfigRadarIndex(radarName) >= 0;
        }

        public bool IsCannotAddLinking(string radarName)
        {
            return findConfigRadarIndex(radarName) != -1;//非空，且未发现
        }

        public bool AddConfigRadar(string radarName)
        {
            if (IsCannotAddLinking(radarName))
                return false;
            _radarNames.Add(radarName);
            return true;
        }

        public bool RemoveConfigRadar(string radarName)
        {
            int index = findConfigRadarIndex(radarName);
            if (index < 0)
                return false;
            _radarNames.RemoveAt(index);
            return true;
        }

        public void ChangedRadarName(string lastName, string newName)
        {
            int index = findConfigRadarIndex(lastName);
            if (index >= 0)
                _radarNames[index] = newName;
        }

        int findConfigRadarIndex(string radarName)
        {
            if (string.IsNullOrWhiteSpace(radarName))
                return -2;
            for (int i = 0; i < _radarNames.Count; i++)
            {
                if (_radarNames[i] == radarName)
                    return i;
            }
            return -1;
        }

        #endregion 相关雷达信息

        #region 图像显示相关

        private void NewPolygon()
        {
            _polygon = new Polygon();
            _polygon.Stroke = Brushes.SpringGreen;
            _polygon.Fill = Brushes.Red;
            _polygon.Opacity = 0.1;
            this.Children.Add(_polygon);
        }

        public double PolygonOpacity { set { _polygon.Opacity = value; } }

        public Color FillColor { set { _polygon.Fill = new SolidColorBrush(value); } }

        public Color StrokeColor { set { _polygon.Stroke = new SolidColorBrush(value); } }

        private Ellipse NewEllipse()
        {
            Ellipse e = new Ellipse();
            e.Fill = _fillColorDefault;
            e.Height = 5;
            e.Width = 5;
            e.StrokeThickness = 5;
            return e;
        }
        SolidColorBrush _fillColorDefault = System.Windows.Media.Brushes.Red;
        SolidColorBrush _fillColorSelected = System.Windows.Media.Brushes.Blue;
        private void selectedPoint(int selectedIndex)
        {
            if (selectedIndex >= 0 && selectedIndex < _ellipses.Count)
            {
                _ellipses[selectedIndex].Fill = _fillColorDefault;
            }
        }

        private void unselectedPoint(int selectedIndex)
        {
            if(selectedIndex >= 0 && selectedIndex < _ellipses.Count)
            {
                _ellipses[selectedIndex].Fill = _fillColorSelected;
            }
        }
        #endregion 图像显示相关

        public bool FillContains(Point pt)
        {
            if (this.Visibility == System.Windows.Visibility.Visible && _polygon.Points.Count > 0)
            {
                PathFigure pthFigure = new PathFigure();
                pthFigure.StartPoint = _polygon.Points[0];
                pthFigure.Segments.Add(new PolyLineSegment((IEnumerable<Point>)_polygon.Points, false));
                PathGeometry pthGeometry = new PathGeometry();
                pthGeometry.Figures.Add(pthFigure);
                if (pthGeometry.FillContains(pt))
                    return true;
            }
            return false;
        }
    }
}
