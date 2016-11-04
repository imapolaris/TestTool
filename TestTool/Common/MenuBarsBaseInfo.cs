using System;
using System.ComponentModel;
using TargetInfomation;

namespace VTSCore.Data.Common
{
    public class MenuBarsBaseInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static MenuBarsBaseInfo Instance { get; private set; }
        static MenuBarsBaseInfo()
        {
            Instance = new MenuBarsBaseInfo();
        }
        private MenuBarsBaseInfo()
        { }

        bool _lockAll;
        bool _tracking;
        bool _distanceMeasurement;
        bool _showAllHistoryTrackLine;
        IMovableTarget _selectedTarget;
        IMovableTarget _trackingTarget;

        public IMovableTarget TrackingTarget
        {
            get { return _trackingTarget; }
            set
            {
                if (_trackingTarget != value)
                {
                    _trackingTarget = value;
                    FirePropertyChanged("TrackingTarget");
                    if (_trackingTarget == null)
                        Tracking = false;
                }
            }
        }
        public IMovableTarget SelectedTarget
        {
            get { return _selectedTarget; }
            set
            {
                if (_selectedTarget != value)
                {
                    _selectedTarget = value;
                    FirePropertyChanged("SelectedTarget");
                }
            }
        }

        public bool LockAll
        {
            get { return _lockAll; }
            set
            {
                if (_lockAll != value)
                {
                    _lockAll = value;
                    FirePropertyChanged("LockAll");
                }
            }
        }

        public bool Tracking
        {
            get { return _tracking; }
            set
            {
                if (_tracking != value)
                {
                    if (value == false || (value && SelectedTarget != null && !LockAll))
                    {
                        _tracking = value;
                        if (value)
                            TrackingTarget = SelectedTarget;
                        else
                            TrackingTarget = null;
                        FirePropertyChanged("Tracking");
                    }
                }
            }
        }

        bool featureSelectShape;
        public bool FeatureSelectShape
        {
            get { return featureSelectShape; }
            set
            {
                featureSelectShape = value;
                FirePropertyChanged("FeatureSelectShape");
            }
        }

        public bool DistanceMeasurement
        {
            get { return _distanceMeasurement; }
            set
            {
                if (_distanceMeasurement != value)
                {
                    _distanceMeasurement = value;
                    FirePropertyChanged("DistanceMeasurement");
                }
            }
        }
        bool _onlyShowIdentifiedTrack;
        public bool OnlyShowIdentifiedTrack
        {
            get { return _onlyShowIdentifiedTrack; }
            set
            {
                if(OnlyShowIdentifiedTrack != value)
                {
                    _onlyShowIdentifiedTrack = value;
                    FirePropertyChanged("OnlyShowIdentifiedTrack");
                }
            }
        }
        
        public bool ShowAllHistoryTrackLine
        {
            get { return _showAllHistoryTrackLine; }
            set
            {
                if(_showAllHistoryTrackLine != value)
                {
                    _showAllHistoryTrackLine = value;
                    FirePropertyChanged("ShowAllHistoryTrackLine");
                }
            }
        }
        public void CCTVConfigSetting()
        {
            FirePropertyChanged("CCTVConfigSetting");
        }

        public void SignalSourceSetting()
        {
            FirePropertyChanged("SignalSourceSetting");
        }

        public void RadarParameterSetting()
        {
            FirePropertyChanged("RadarParameterSetting");
        }

        public void PlottingAreaParameterSetting()
        {
            FirePropertyChanged("PlottingAreaParameterSetting");
        }

        public void CCTVTreeView()
        {
            FirePropertyChanged("CCTVTreeView");
        }

        public void ClearCache()
        {
            FirePropertyChanged("ClearCache");
        }

        private void FirePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RadarColorSchemes()
        {
            FirePropertyChanged("RadarColorSchemes");
        }

        public void TrackLength()
        {
            FirePropertyChanged("TrackLength");
        }

        public void StartUpPosition()
        {
            FirePropertyChanged("StartUpPosition");
        }
    }
}
