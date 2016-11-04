using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using TargetInfomation;
using VTSCore.Data.Common;
using VTSCore.Layers.Maps;

namespace VTSCore.Layers.Tracks
{
    public class SelectedTargetDraw : Canvas
    {
        TrackingTargetCanvas _trackingTarget;

        public static SelectedTargetDraw Instance { get; private set; }
        static SelectedTargetDraw()
        {
            Instance = new SelectedTargetDraw();
        }

        public void Init(LocatorAndBorder locator)
        {
            this.Children.Clear();
            _trackingTarget = new TrackingTargetCanvas(locator);
            this.Children.Add(_trackingTarget);
            MenuBarsBaseInfo.Instance.PropertyChanged += _menuBarsInfo_PropertyChanged;
        }

        public TrackCanvas SelectedTrack
        {
            get { return SelectingTargetCmd.Default.SelectedTrack; }
            set { SelectingTargetCmd.Default.SelectedTrack = value; }
        }

        public double SelectedDistance
        {
            get { return SelectingTargetCmd.Default.Distance; }
            set { SelectingTargetCmd.Default.Distance = value; }
        }

        private void _menuBarsInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Tracking")
            {
                if (_trackingTarget != null)
                    _trackingTarget.TrackingTarget = MenuBarsBaseInfo.Instance.Tracking && SelectedTrack!=null ? SelectedTrack.GetTarget() : null;
            }
            else if (e.PropertyName == "ClearCache")
            {
                MenuBarsBaseInfo.Instance.LockAll = false;
                SelectedTrack = null;
            }
        }
    }
}