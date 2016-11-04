using System.ComponentModel;

namespace VTSCore.Data.Common
{
    public enum ChartsStatus
    {
        移动海图,
        缩放海图,
        移动雷达,
        拖拽雷达,
    }
    public enum TraggingStatus
    {
        海图模式,
        标绘模式
    }
    public class ActivatingStatus :INotifyPropertyChanged
    {
        ChartsStatus _chartStatus;
        TraggingStatus _tragStatus;
        public static ActivatingStatus Instance { get; private set; }
        static ActivatingStatus()
        {
            Instance = new ActivatingStatus();
        }
        private ActivatingStatus()
        {
            _chartStatus = ChartsStatus.移动海图;
            _tragStatus = TraggingStatus.海图模式;
        }
        public ChartsStatus ChartStatus
        {
            get { return _chartStatus; }
            set 
            {
                if(_chartStatus != value)
                {
                    _chartStatus = value;
                    FirePropertyChanged("ChartsStatus");
                }
            }
        }
        public TraggingStatus TragStatus
        {
            get { return _tragStatus; }
            set 
            {
                if(_tragStatus != value)
                {
                    _tragStatus = value;
                    FirePropertyChanged("TraggingStatus");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void FirePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    
}