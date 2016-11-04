using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTSCore.Common
{
    public class WindowFrontor : INotifyPropertyChanged
    {
        public static WindowFrontor Instance { get; private set; }
        static WindowFrontor()
        {
            Instance = new WindowFrontor();
        }

        public void FrontWindow()
        {
            FirePropertyChanged("FrontWindow");
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void FirePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
