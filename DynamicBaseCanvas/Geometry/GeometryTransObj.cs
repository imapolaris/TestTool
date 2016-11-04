using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DynamicBaseCanvas
{
    public class GeometryTransObj : Canvas
    {
        UIElement _element;

        public GeometryTransObj(UIElement element)
        {
            _element = element;
            this.Children.Add(_element);
        }

        double _scale = 1;
        public double ScaleTransform
        {
            set
            {
                if (_element != null && _scale != value)
                {
                    _element.RenderTransform = new System.Windows.Media.ScaleTransform(value, value, 0, 0);
                    _scale = value;
                }
            }
        }

        double _angle = 0;
        public double RotateTransform
        {
            set
            {
                if (_element != null && _angle != value)
                {
                    _element.RenderTransform = new System.Windows.Media.RotateTransform(value);
                    _angle = value;
                }
            }
        }

    }
}
