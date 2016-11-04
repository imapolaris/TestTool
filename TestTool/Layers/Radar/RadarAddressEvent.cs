using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTSCore.Layers.Radar
{
    public class RadarAddressEvent : RadarConnection, INotifyPropertyChanged
    {
        public int Uid { get; set; }

        string heading = "";
        public string Heading
        {
            get { return heading; }
            set
            {
                if (value == null)
                    value = "";
                if (heading != value)
                {
                    heading = value;
                    FirePropertyChanged("Heading");
                }
            }
        }

        public override string Ip
        {
            get
            {
                return base.Ip;
            }
            set
            {
                base.Ip = value;
                FirePropertyChanged("Ip");
            }
        }

        public override int Port
        {
            get
            {
                return base.Port;
            }
            set
            {
                base.Port = value;
                FirePropertyChanged("Port");
            }
        }

        public override string RpcEndPoint
        {
            get
            {
                return base.RpcEndPoint;
            }
            set
            {
                base.RpcEndPoint = value;
                FirePropertyChanged("RpcEndPoint");
            }
        }

        public override int ColorTableIndex
        {
            get
            {
                return base.ColorTableIndex;
            }
            set
            {
                base.ColorTableIndex = value;
                FirePropertyChanged(nameof(ColorTableIndex));
            }
        }

        public RadarAddressEvent(RadarConnection radar)
            : base(radar)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void FirePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
