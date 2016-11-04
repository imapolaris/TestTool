using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DynamicBaseCanvas
{
    public class TrackLineShape : Canvas
    {
        Polyline _trackLine;
        List<EllipseObj> _ellipses = new List<EllipseObj>();
        System.Windows.Media.SolidColorBrush _brush;
        public TrackLineShape(System.Windows.Media.SolidColorBrush brush)// = System.Windows.Media.Brushes.Red
        {
            _brush = brush;
            _trackLine = new Polyline();
            _trackLine.Stroke = brush;
            this.Children.Add(_trackLine);
        }

        public void Update(List<Point> points)
        {
            Clear();
            _trackLine.Points = new System.Windows.Media.PointCollection(points);
            lock(_ellipses)
            {
                foreach (var pt in points)
                    addElli(pt);
            }
        }

        private void addElli(Point pt)
        {
            EllipseObj elli = new EllipseObj(3, _brush);
            Canvas.SetLeft(elli, pt.X-1);
            Canvas.SetTop(elli, pt.Y-1);
            this.Children.Add(elli);
            _ellipses.Add(elli);
        }

        public void Add(Point pt)
        {
            if (needAddPoint(pt))
            {
                lock(_ellipses)
                {
                    _trackLine.Points.Add(pt);
                    addElli(pt);
                }
            }
        }

        public void Clear()
        {
            _trackLine.Points.Clear();
            lock(_ellipses)
            {
                foreach (var elli in _ellipses)
                    this.Children.Remove(elli);
                _ellipses.Clear();
            }
        }
        
        private bool needAddPoint(Point pt)
        {
            int count = _trackLine.Points.Count;
            return (count == 0 || effectDistance(_trackLine.Points[count - 1], pt));
        }

        private bool effectDistance(Point point1, Point point2)
        {
            return (Math.Abs(point1.X - point2.X) > 2 || Math.Abs(point1.Y - point2.Y) > 2);
        }

    }
}
