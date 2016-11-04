using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using TargetInfomation;
using VTSCore.Data.Common;
using VTSCore.Layers.Maps;

namespace VTSCore.Layers.Tracks
{
    public class SelectingTargetCmd
    {
        public static SelectingTargetCmd Default = new SelectingTargetCmd();
        public double Distance = 10;

        TrackCanvas _track;
        public TrackCanvas SelectedTrack
        {
            get { return _track; }
            set
            {
                if (_track == value)
                    return;
                disposeSelectedTrack();
                updateSelectedTrack(value);
                updateStatusString();
            }
        }

        void SelectingTargetCanvas_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UpdateData" || e.PropertyName == "UpdateDataLast")
                updateStatusString();
            else if (e.PropertyName == "Invisible")
                SelectedTrack = null;
        }

        private void updateSelectedTrack(TrackCanvas track)
        {
            _track = track;
            if (_track != null)
            {
                _track.Selected = true;
                IMovableTarget target = _track.GetTarget();
                MenuBarsBaseInfo.Instance.SelectedTarget = target;
                if (MenuBarsBaseInfo.Instance.SelectedTarget != null)
                    (MenuBarsBaseInfo.Instance.SelectedTarget as MovableTarget).PropertyChanged += SelectingTargetCanvas_PropertyChanged;
            }
            else
                MenuBarsBaseInfo.Instance.SelectedTarget = null;
        }

        private void disposeSelectedTrack()
        {
            if (_track != null)
            {
                (MenuBarsBaseInfo.Instance.SelectedTarget as MovableTarget).PropertyChanged -= SelectingTargetCanvas_PropertyChanged;
                _track.Selected = false;
            }
            _track = null;
        }

        private void updateStatusString()
        {
            StatusBarBaseInfomation.Instance.Selected = "选中";
            if (_track != null)
                StatusBarBaseInfomation.Instance.Selected += ": " + _track.GetTarget().GetDescription();
        }
    }
}
