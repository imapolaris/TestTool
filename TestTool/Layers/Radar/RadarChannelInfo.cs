using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTSCore.Layers.Radar
{
    public class RadarChannelInfo : INotifyPropertyChanged
    {
        string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    FirePropertyChanged("Name");
                }
            }
        }

        string filter;
        public string Filter
        {
            get { return filter; }
            set {
                if (filter != value)
                {
                    filter = value;
                    FirePropertyChanged("Filter");
                }
            }
        }

        int port;
        public int LegacyPort
        {
            get { return port; }
            set
            {
                if(port != value)
                {
                    port = value;
                    FirePropertyChanged("LegacyPort");
                }
            }
        }

        string rate;
        public string Rate
        {
            get { return rate; }
            set
            {
                if (rate != value)
                {
                    rate = value;
                    FirePropertyChanged("Rate");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void FirePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
