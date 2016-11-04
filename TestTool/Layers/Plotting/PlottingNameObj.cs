using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTSCore.Layers.Plotting
{
    class PlottingNameObj : INotifyPropertyChanged
    {
        string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (Name != value)
                {
                    _name = value;
                    FirePropertyChanged("Name");
                }
            }
        }

        string _source;
        public string Source
        {
            get { return _source; }
            set
            {
                if (Source != value)
                {
                    _source = value;
                    FirePropertyChanged("Source");
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


    public class LinkingRadarObj : INotifyPropertyChanged
    {
        public int Uid { get; set; }

        string heading;
        public string Heading 
        {
            get { return heading; }
            set
            {
                if(Heading != value)
                {
                    heading = value;
                    FirePropertyChanged("Heading");
                    FirePropertyChanged("IsEnabled");
                }
            }
        }

        bool isLinking;
        public bool IsLinking
        {
            get { return isLinking; }
            set
            {
                if (IsLinking != value)
                {
                    isLinking = value;
                    FirePropertyChanged("IsLinking");
                }
            }
        }
        bool _isEnabled;
        public bool IsEnabled 
        { 
            set
            {
                _isEnabled = value;
            }
            get 
            { 
                return _isEnabled && !string.IsNullOrWhiteSpace(Heading); 
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
