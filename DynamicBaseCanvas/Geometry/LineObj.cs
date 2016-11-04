using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace DynamicBaseCanvas
{
    public class LineObj : Canvas
    {
        Line _line;
        public LineObj()
        {
            _line = new Line();
            _line.Stroke = System.Windows.Media.Brushes.Black;
            _line.X1 = 0;
            _line.X2 = 0;
            _line.Y1 = 0;
            _line.Y2 = 0;
            this.Children.Add(_line);
        }

        public double Length 
        { 
            get { return -_line.Y2; } 
            set { _line.Y2 = -value; } 
        }
    }
}