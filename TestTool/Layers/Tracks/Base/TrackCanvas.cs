using DynamicBaseCanvas;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using TargetInfomation;
using VTSCore.Data.Common;
using VTSCore.Layers.Maps;

namespace VTSCore.Layers.Tracks
{
    public class TrackCanvas : Canvas, IDisposable
    {
        LocatorAndBorder _locator;
        MovableTarget _target;
        GeometryMovableObj _geometry;
        //Polyline _trackLine;
        TrackLineShape _trackLineShape;

        public TargetDisplayStatus DisplayStatusPrev 
        {
            get { return _geometry.DisplayStatusPrev; } 
        }

        public GeometryType EnumerateType { get; private set; }
        System.Windows.Media.SolidColorBrush _fill;
        System.Windows.Media.SolidColorBrush _stroke;
        public TrackCanvas(LocatorAndBorder locator, GeometryType enumerateType, System.Windows.Media.SolidColorBrush stroke, System.Windows.Media.SolidColorBrush fill, bool isFill)
        {
            _locator = locator;
            _stroke = stroke;
            _fill = fill;
            EnumerateType = enumerateType;
            _geometry = new GeometryMovableObj(EnumerateType, _stroke, _fill, isFill);
            this.Children.Add(_geometry);
            _trackLineShape = new TrackLineShape(System.Windows.Media.Brushes.Red);
            this.Children.Add(_trackLineShape);
        }

        public void UpdateDynamicEvent(MovableTarget target)
        {
            if(_target == null)
                _target = target.Clone();
            _target.Update(target);
            updateTargetShow();
            updateLastestHistoryPoint();
        }

        TargetDisplayStatus _displayStatus = TargetDisplayStatus.UnShow;
        public void UpdateShow()
        {
            updateTargetShow();
            updateHistoryLines();
        }

        public void UpdateStaticEvent(MovableTarget target)
        {
            if (_target == null)
            {
                _target = target.Clone();
            }
            else
            {
                if (target.Name != string.Empty)
                    _target.Name = target.Name;
                _target.Type = target.Type;
                if (_target is AisTarget)
                    (_target as AisTarget).Length = (target as AisTarget).Length;
            }
        }

        public IMovableTarget GetTarget()
        {
            return _target;
        }
        
        public void Invisible()
        {
            if (_target != null)
            {
                _target.TimeOut = true;
                _target.Invisible();
            }
            _geometry.Invisible();
        }
        
        public TrackCanvas Clone()
        {
            TrackCanvas clone = new TrackCanvas(_locator, EnumerateType, _stroke, _fill, IsFill);
            clone._target = _target.Clone();
            clone._geometry.Update(_geometry.MMSI, (int)_geometry.Sog, _geometry.Cog, _geometry.Heading, _geometry.DisplayStatusPrev);
            clone.Opacity = this.Opacity;
            clone.UpdateShow();
            clone.Selected = Selected;
            return clone;
        }

        public void Dispose()
        {
            if (_target != null)
                _target.Dispose();
            _target = null;
            this.Children.Clear();
            _trackLineShape.Clear();
        }

        public double OpacityInfo
        {
            get { return _geometry.OpacityInfo; }
            set { _geometry.OpacityInfo = value; }
        }

        public bool IsFill
        {
            get { return _geometry.IsFill; } 
            set { _geometry.IsFill = value; } 
        }
        
        private void updateTargetShow()
        {
            if (_target == null)
                return;
            _displayStatus = TargetDisplayStatus.UnShow;
            if (_locator.InScreen(_target.Lon, _target.Lat))
                _displayStatus = _locator.DisplayStatus;
            _geometry.Update(_target.MMSI, (int)(_target.SOG * _locator.ScaleTrans), _target.COG, _target.GetHeading(), _displayStatus);
            if (_displayStatus == TargetDisplayStatus.UnShow)
                return;
            PointInScreen = _locator.Locator.MapToScreen(_target.Lon, _target.Lat);
            Canvas.SetLeft(_geometry, PointInScreen.X);
            Canvas.SetTop(_geometry, PointInScreen.Y);
        }

        public void UpdateHistoryInfo(bool showHistoryTrackLine)
        {
            if (showHistoryTrackLine)
            {
                addHistoryPoint();
            }
            else
            {
                _trackLineShape.Clear();
            }
        }

        private void updateHistoryLines()
        {
            if(MenuBarsBaseInfo.Instance.ShowAllHistoryTrackLine || Selected)
            {
                if (_displayStatus != TargetDisplayStatus.UnShow && _target != null)
                {
                    updateAllHistoryPoints();
                    _trackLineShape.Visibility = System.Windows.Visibility.Visible;
                    return;
                }
            }
            _trackLineShape.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void updateAllHistoryPoints()
        {
            List<Point> points = new List<Point>();
            var targets = _target.History.ToArray();
            for (int i = 0; i < targets.Length; i++)
            {
                Point screen = _locator.Locator.MapToScreen(targets[i].Lon, targets[i].Lat);
                if (i == 0 || effectDistance(points[points.Count - 1], screen))
                    points.Add(screen);
            }
            points.Add(_locator.Locator.MapToScreen(_target.Lon, _target.Lat));
            _trackLineShape.Update(points);
        }

        private void updateLastestHistoryPoint()
        {
            if ((MenuBarsBaseInfo.Instance.ShowAllHistoryTrackLine || Selected) && (_displayStatus != TargetDisplayStatus.UnShow && _target != null))
            {
                if (_trackLineShape.Visibility != System.Windows.Visibility.Visible)
                    updateAllHistoryPoints();
                else
                {
                    Point screenPt = _locator.Locator.MapToScreen(_target.Lon, _target.Lat);
                    _trackLineShape.Add(screenPt);
                }
                _trackLineShape.Visibility = System.Windows.Visibility.Visible;
            }
            else
                _trackLineShape.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void addHistoryPoint()
        {
            if (_target == null)
                return;
            if (_displayStatus == TargetDisplayStatus.UnShow)
                return;
            updateHistoryLines();
        }

        private bool effectDistance(Point point1, Point point2)
        {
            return (Math.Abs(point1.X - point2.X) > 2 || Math.Abs(point1.Y - point2.Y) > 2);
        }

        public Point PointInScreen { get; private set; }

        public bool Selected 
        {
            get { return _geometry.Selected; }
            set
            {
                _geometry.Selected = value;
                if (!_geometry.Selected)
                    _trackLineShape.Clear();
                else
                    updateHistoryLines();
            }
        }
    }
}