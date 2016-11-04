using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;
using TargetInfomation;
using VTSCore.Data.Common;
using VTSCore.Layers.Maps;

namespace VTSCore.Layers.Tracks
{
    public class TrackingTargetCanvas : Canvas
    {
        Ellipse _circle;

        LocatorAndBorder _locator;
        IMovableTarget _target;
        StatusBarBaseInfomation _statusBarsInfo;
        public TrackingTargetCanvas(LocatorAndBorder locator)
        {
            _locator = locator;
            _statusBarsInfo = StatusBarBaseInfomation.Instance;
            _circle = newEllipse(System.Windows.Media.Brushes.Red);
            this.Children.Add(_circle);
            _locator.OnMapRefreshed += onMapRefreshed;
        }

        public IMovableTarget TrackingTarget
        {
            get { return _target; }
            set
            {
                if (_target == value)
                    return;
                _target = value;
                updateCanvas();
                if (TrackingTarget != null && TrackingTarget is MovableTarget)
                    (TrackingTarget as MovableTarget).PropertyChanged += onMovableTarget;
            }
        }

        private void onMapRefreshed()
        {
            updateCircle();
        }

        private void onMovableTarget(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UpdateData" || e.PropertyName == "UpdateDataLast")
                updateCanvas();
            else if (e.PropertyName == "Invisible")
                TrackingTarget = null;
        }

        private void updateCanvas()
        {
            updateCircle();
            updateStatusString();
        }

        private void updateCircle()
        {
            if (TrackingTarget != null && _locator.InScreen(TrackingTarget.Lon, TrackingTarget.Lat))
            {
                var point = _locator.Locator.MapToScreen(_target.Lon, _target.Lat);
                Canvas.SetLeft(_circle, point.X - 20);
                Canvas.SetTop(_circle, point.Y - 20);
                _circle.Visibility = System.Windows.Visibility.Visible;
            }
            else
                _circle.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void updateStatusString()
        {
            _statusBarsInfo.Tracking = "跟踪";
            if (TrackingTarget != null)
                _statusBarsInfo.Tracking += ": " + TrackingTarget.GetDescription();
        }

        private Ellipse newEllipse(System.Windows.Media.SolidColorBrush brush)
        {
            Ellipse e = new Ellipse();
            e.Stroke = brush;
            e.Height = 41;
            e.Width = 41;
            e.Visibility = System.Windows.Visibility.Collapsed;
            return e;
        }
    }
}
