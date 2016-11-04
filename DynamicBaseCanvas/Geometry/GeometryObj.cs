using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace DynamicBaseCanvas
{
    public abstract class GeometryObj : Canvas, IGeometryObj
    {
        public System.Windows.Media.SolidColorBrush FillColor { get; private set; }
        public System.Windows.Media.SolidColorBrush StrokeColor { get; private set; }

        public virtual bool IsFill { get; set; }

        public GeometryObj(System.Windows.Media.SolidColorBrush stroke, System.Windows.Media.SolidColorBrush fill, bool isFill)
        {
            StrokeColor = stroke;
            FillColor = fill;
            IsFill = isFill;
        }

        public virtual double TransformHeading
        {
            set 
            {
                this.RenderTransform = new System.Windows.Media.RotateTransform(value); 
            } 
        }

        public virtual double OpacityInfo
        {
            get { return this.Opacity; }
            set { this.Opacity = value; }
        }
    }
}