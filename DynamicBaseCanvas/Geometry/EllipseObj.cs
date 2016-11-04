using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DynamicBaseCanvas
{
    public class EllipseObj : GeometryObj
    {
        Ellipse _ellipse;

        public EllipseObj(int radius, SolidColorBrush stroke = null, SolidColorBrush fill = null)
            : base(stroke, fill, true)
        {
            _ellipse = new Ellipse();
            _ellipse.Fill = fill;
            _ellipse.Stroke = stroke;
            _ellipse.Height = radius * 2 + 1;
            _ellipse.Width = radius * 2 + 1;
            Canvas.SetLeft(_ellipse, -radius);
            Canvas.SetTop(_ellipse, -radius);
            this.Children.Add(_ellipse);
        }
    }
}