using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DynamicBaseCanvas
{
    public class RectangleObj: GeometryObj
    {
        Rectangle _rect;
        public RectangleObj(int side, System.Windows.Media.SolidColorBrush stroke = null, System.Windows.Media.SolidColorBrush fill = null, bool needFill = true)
            : base(stroke, fill, needFill)
        {
            _rect = new Rectangle();
            _rect.Width = side;
            _rect.Height = side;
            _rect.Fill = fill;
            _rect.Stroke = stroke;
            Canvas.SetLeft(_rect, -side / 2);
            Canvas.SetTop(_rect, -side / 2);
            this.Children.Add(_rect);
        }

        public override double TransformHeading { set { } }
    }
}
