using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace DynamicBaseCanvas
{
    public enum TargetDisplayStatus
    {
        UnShow,
        SmallIcon,
        LargeIcon
    }

    public enum GeometryType
    {
        Triangle,
        Circle,
        Rectangle
    }

    public class GeometryMovableObj: Canvas
    {
        GeometryObj _largeIcon;
        GeometryObj _smallIcon;
        CircleIconObj _selectedIcon;
        LineObj _sogLine;
        public TargetDisplayStatus DisplayStatusPrev { get; private set; }

        public GeometryMovableObj(GeometryType type, System.Windows.Media.SolidColorBrush stroke, System.Windows.Media.SolidColorBrush fill, bool needFill)
        {
            if (type == GeometryType.Circle)
                _largeIcon = new CircleIconObj(7, 2, stroke, fill, needFill);
            else if (type == GeometryType.Rectangle)
                _largeIcon = new RectangleObj(16, stroke, fill, needFill);
            else
                _largeIcon = new TriangleIconObj(12, 1, stroke, fill, needFill);
            _smallIcon = new RectangleObj(3, Brushes.Green, Brushes.Green);
            this.Children.Add(_largeIcon);
            this.Children.Add(_smallIcon);
            _sogLine = new LineObj();
            this.Children.Add(_sogLine);
            invisible();
        }
        
        public void Update(int mmsi, int sog, double cog, int heading, TargetDisplayStatus status)
        {
            if (status != DisplayStatusPrev)
            {
                Invisible();
                Visible(status);
            }
            if (DisplayStatusPrev == TargetDisplayStatus.LargeIcon)
            {
                Sog = sog;
                Cog = cog;
                Heading = heading;
                MMSI = mmsi;
            }
        }
        int _mmsi;
        public int MMSI
        {
            get { return _mmsi; }
            private set 
            {
                _mmsi = value;
                if(_largeIcon is CircleIconObj)
                {
                    (_largeIcon as CircleIconObj).ShowTriangle = (_mmsi > 0);
                }
            }
        }
        double _cog;
        public double Cog 
        {
            get { return _cog; }
            private set 
            {
                _cog = value;
                _sogLine.RenderTransform = new System.Windows.Media.RotateTransform(value);
            } 
        }
        int _heading;
        public int Heading 
        {
            get { return _heading; }
            private set
            {
                _heading = value;
                _largeIcon.TransformHeading = _heading;
            }
        }
        public double Sog
        { 
            get { return _sogLine.Length; }
            private set { _sogLine.Length = value; }
        }
        public void Invisible()
        {
            if (DisplayStatusPrev != TargetDisplayStatus.UnShow)
                invisible();
        }

        private void Visible(TargetDisplayStatus status)
        {
            DisplayStatusPrev = status;
            switch (DisplayStatusPrev)
            {
                case TargetDisplayStatus.UnShow:
                    break;
                case TargetDisplayStatus.SmallIcon:
                    _smallIcon.Visibility = System.Windows.Visibility.Visible;
                    break;
                case TargetDisplayStatus.LargeIcon:
                    _largeIcon.Visibility = System.Windows.Visibility.Visible;
                    _sogLine.Visibility = System.Windows.Visibility.Visible;
                    break;
            }
        }

        private void invisible()
        {
            _smallIcon.Visibility = System.Windows.Visibility.Collapsed;
            _largeIcon.Visibility = System.Windows.Visibility.Collapsed;
            _sogLine.Visibility = System.Windows.Visibility.Collapsed;
            DisplayStatusPrev = TargetDisplayStatus.UnShow;
        }

        public double OpacityInfo
        {
            get { return _largeIcon.OpacityInfo; }
            set{_largeIcon.OpacityInfo = value;}
        }

        public bool IsFill 
        {
            get { return _largeIcon.IsFill; } 
            set { _largeIcon.IsFill = value; } 
        }
        bool _isSelected;
        public bool Selected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                if (_isSelected)
                {
                    removeSelectedIcon();
                    _selectedIcon = new CircleIconObj(20, 1, System.Windows.Media.Brushes.Blue, null, false);
                    _largeIcon.Children.Add(_selectedIcon);
                }
                else
                    removeSelectedIcon();
            }
        }

        private void removeSelectedIcon()
        {
            if (_selectedIcon != null)
                _largeIcon.Children.Remove(_selectedIcon);
            _selectedIcon = null;
        }
    }
}
