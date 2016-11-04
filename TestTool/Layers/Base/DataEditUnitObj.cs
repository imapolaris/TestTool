using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTSCore.Layers.Base
{
    class DataEditUnitObj : INotifyPropertyChanged
    {
        public int Uid { get; set; }
        public string Heading { get; set; }
        string baseValue;
        public string BaseValue
        {
            get { return baseValue; }
            set
            {
                if(baseValue != value)
                {
                    baseValue = value;
                    FirePropertyChanged("BaseValue");
                }
            }
        }
        public string EditingValue { get; set; }

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
