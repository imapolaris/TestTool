using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using VTSCore.Data.Common;
using VTSCore.Layers.Maps;

namespace VTSCore.Layers.Lines
{
    public class DistanceMeasurementLines : Canvas
    {
        ILocator Locator{get; set;}
        List<MapPoint> _mapPoints;
        List<Line> _lines;
        List<TextBlock> _distances;
        List<Ellipse> _ellipses;
        int PointsCount { get { return _mapPoints.Count; } }
        double _distance = 0;
        Line presLine;
        TextBlock presDistance;
        Ellipse presEllipse;
        public DistanceMeasurementLines(ILocator locator)
        {
            Locator = locator;
            _mapPoints = new List<MapPoint>();
            _lines = new List<Line>();
            _distances = new List<TextBlock>();
            _ellipses = new List<Ellipse>();
            presLine = new Line();
            presDistance = new TextBlock();
            presEllipse = new Ellipse();
            this.Children.Add(presLine);
            this.Children.Add(presDistance);
            this.Children.Add(presEllipse);
        }
        public void Push(MapPoint position)
        {
            _mapPoints.Add(position);
            Ellipse e = new Ellipse();
            _ellipses.Add(e);
            drawEllipse(e, position);
            this.Children.Add(e);
            if (PointsCount > 1)
            {
                _lines.Add(new Line());
                this.Children.Add(_lines[_lines.Count - 1]);
                _distance += PointsBaseInfomation.GetDelta(_mapPoints[PointsCount - 2], _mapPoints[PointsCount - 1]);
                TextBlock text = new TextBlock();
                text.Text = getDistanceText(_distance);
                _distances.Add(text);
                this.Children.Add(_distances[_distances.Count - 1]);
                drawLine(_mapPoints[PointsCount - 2], _mapPoints[PointsCount - 1], _lines[_lines.Count - 1]);
                drawTextAnnotation(_distances[_distances.Count - 1], _mapPoints[PointsCount - 1]);
            }
        }

        private string getDistanceText(double distance)
        {
            return string.Format("{0:F4} NM", distance);
        }
        public void UpdateMouseAll(MapPoint position)
        {
            if (PointsCount > 0)
            {
                for (int i = 0; i < PointsCount - 1; i++)
                {
                    drawLine(_mapPoints[i], _mapPoints[i + 1], _lines[i]);
                    drawTextAnnotation(_distances[i], _mapPoints[i + 1]);
                }
                for (int i = 0; i < PointsCount; i++)
                    drawEllipse(_ellipses[i], _mapPoints[i]);
                UpdateMousePosition(position);
            }
        }
        public void UpdateMousePosition(MapPoint position)
        {
            if (PointsCount > 0 && !IsStop)
            {
                double dist = PointsBaseInfomation.GetDelta(_mapPoints[PointsCount - 1], position);
                drawLine(_mapPoints[PointsCount - 1], position, presLine);
                presDistance.Text = getDistanceText(_distance + dist);
                drawTextAnnotation(presDistance, position);
                drawEllipse(presEllipse, position);
            }
        }

        private void drawLine(MapPoint mapPointBegin, MapPoint mapPointEnd, Line myLine)
        {
            myLine.Stroke = System.Windows.Media.Brushes.Linen;
            Point pointBegin = Locator.MapToScreen(mapPointBegin.Lon, mapPointBegin.Lat);
            Point pointEnd = Locator.MapToScreen(mapPointEnd.Lon, mapPointEnd.Lat);
            myLine.X1 = pointBegin.X;
            myLine.X2 = pointEnd.X;
            myLine.Y1 = pointBegin.Y;
            myLine.Y2 = pointEnd.Y;
        }

        private void drawTextAnnotation(TextBlock textBlock, MapPoint mapPoint)
        {
            Point point = Locator.MapToScreen(mapPoint.Lon, mapPoint.Lat);
            textBlock.Height = 20;
            textBlock.Foreground = System.Windows.Media.Brushes.Linen;
            Canvas.SetLeft(textBlock, point.X + 3);
            Canvas.SetTop(textBlock, point.Y - 15);
        }

        private void drawEllipse(Ellipse e, MapPoint position)
        {
            e.Fill = System.Windows.Media.Brushes.Red;
            e.Height = 5;
            e.Width = 5;
            e.StrokeThickness = 5;
            Point pt = Locator.MapToScreen(position.Lon, position.Lat);
            Canvas.SetLeft(e, pt.X - e.Width / 2);
            Canvas.SetTop(e, pt.Y - e.Height / 2);
        }
        bool _isStop;
        public bool IsStop 
        {
            get { return _isStop; }
            set 
            {
                if (_isStop == value)
                    return;
                _isStop = value;
                if(IsStop)
                {
                    this.Children.Remove(presLine);
                    this.Children.Remove(presDistance);
                    this.Children.Remove(presEllipse);
                }
            } 
        }
    }
}
