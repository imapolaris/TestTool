using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace DynamicBaseCanvas
{
    public class TriangleIconObj: GeometryObj
    {
        Shape _icon;
        public TriangleIconObj(double radius, int strokeThickness, System.Windows.Media.SolidColorBrush stroke = null, System.Windows.Media.SolidColorBrush fill = null, bool needFill = true)
            : base(stroke, fill, needFill)
        {
            initIcon(radius, strokeThickness);
        }
        private void initIcon(double radius, int strokeThickness)
        {
            _icon = newPolygon(radius);
            _icon.Fill = FillColor;
            _icon.Stroke = StrokeColor;
            _icon.StrokeThickness = strokeThickness;
            this.Children.Add(_icon);
        }

        Polygon newPolygon(double radius)
        {
            Polygon polygon = new Polygon();
            polygon.Points.Add(new Point(0, -radius));
            polygon.Points.Add(new Point(-radius * 0.6, radius * 0.8));
            polygon.Points.Add(new Point(radius * 0.6, radius * 0.8));
            return polygon;
        }
    }
}
