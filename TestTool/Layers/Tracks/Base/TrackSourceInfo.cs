using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTSCore.Layers.Tracks
{
    
    public class TrackSource
    {
        public string Type { get; set; }
        public string Setting { get; set; }
        public string Remarks { get; set; }
        public bool IsStartDefault { get; set; }
        public bool IsVisible { get; set; }

    }
    public class TrackSourceInfo: INotifyPropertyChanged
    {
        public int Uid { get; set; }

        public string Type { get; set; }

        string setting = "";
        public string Setting { get { return setting; }
            set
            {
                if(setting != value)
                {
                    setting = value;
                    FirePropertyChanged("Setting");
                }
            }
        }

        bool isEnable = false;
        public bool IsEnable
        {
            get { return isEnable; }
            set
            {
                if (isEnable != value)
                {
                    isEnable = value;
                    FirePropertyChanged("IsStartUsing");
                }
            }
        }
        
        public bool isVisible = false;
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                if (isVisible != value)
                {
                    isVisible = value;
                    FirePropertyChanged("IsVisible");
                }
            }
        }

        string remarks = "";
        public string Remarks { get { return remarks; }
            set
            {
                if(remarks != value)
                {
                    remarks = value;
                    FirePropertyChanged("Remarks");
                }
            }
        }

        public bool AreEqual(TrackSourceInfo source)
        {
            return source != null && Type == source.Type && Setting == source.Setting && IsEnable == source.IsEnable && Remarks == source.Remarks && IsVisible == source.IsVisible;
        }

        public TrackSourceInfo Clone()
        {
            return (TrackSourceInfo)MemberwiseClone();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void FirePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
