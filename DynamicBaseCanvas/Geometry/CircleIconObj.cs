using System.Windows.Controls;
using System.Windows.Shapes;

namespace DynamicBaseCanvas
{
    public class CircleIconObj : GeometryObj
    {
        double _radius;
        int _strokeThickness;
        bool _showTriangle;

        Shape _icon;
        TriangleIconObj _triangle;

        public CircleIconObj(double radius, int strokeThickness, System.Windows.Media.SolidColorBrush stroke = null, System.Windows.Media.SolidColorBrush fill = null, bool needFill = true)
            : base(stroke, fill, needFill)
        {
            initIcon(radius, strokeThickness);
        }

        public bool ShowTriangle 
        {
            get { return _showTriangle; }
            set 
            {
                if (_showTriangle == value)
                    return;
                _showTriangle = value;
                updateTriangle();
            }
        }
        public override bool IsFill 
        { 
            get{return base.IsFill;} 
            set
            {
                if(base.IsFill != value)
                {
                    base.IsFill = value;
                    updateFill();
                }
            }
        }


        public override double OpacityInfo
        {
            get { return base.OpacityInfo; }
            set
            {
                base.OpacityInfo = value;
                updateFill();
            }
        }

        private void initIcon(double radius, int strokeThickness)
        {
            _radius = radius;
            _strokeThickness = strokeThickness;
            _icon = newEllipse(radius);
            _icon.Fill = FillColor;
            _icon.Stroke = StrokeColor;
            _icon.StrokeThickness = strokeThickness;
            this.Children.Add(_icon);
        }

        Ellipse newEllipse(double radius)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Height = radius * 2 + 1;
            ellipse.Width = radius * 2 + 1;
            Canvas.SetLeft(ellipse, -radius);
            Canvas.SetTop(ellipse, -radius);
            return ellipse;
        }

        private void updateTriangle()
        {
            if (ShowTriangle)
            {
                if (_triangle == null)
                {
                    _triangle = new TriangleIconObj(_radius, _strokeThickness, FillColor);
                    this.Children.Add(_triangle);
                }
                _icon.Fill = null;
            }
            else
            {
                if (_triangle != null)
                    this.Children.Remove(_triangle);
                _triangle = null;
                updateFill();
            }
        }

        private void updateFill()
        {
            if(_icon != null)
            {
                if (IsFill && !ShowTriangle)
                    _icon.Fill = FillColor;
                else
                    _icon.Fill = null;
            }
        }
    }
}