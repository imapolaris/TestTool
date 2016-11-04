using Seecool.Radar.Unit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTSCore.Data.Common
{
    class StatusBarBaseInfomation: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        PointD position = new PointD(-181, -91);
        string scale = "比例尺";
        string tracking = "跟踪";
        string selected = "选中";
        public static StatusBarBaseInfomation Instance { get; private set; }

        static StatusBarBaseInfomation()
        {
            Instance = new StatusBarBaseInfomation();
        }

        private StatusBarBaseInfomation()
        {

        }

        public PointD Position
        {
            get { return position; }
            set
            {
                if (position.X != value.X || position.Y != value.Y)
                {
                    position = value;
                    FirePropertyChanged("Position");
                    string newPositionString = getPositionString(position);
                    if (newPositionString != _positionString)
                    {
                        _positionString = newPositionString;
                        FirePropertyChanged("PositionString");
                    }
                }
            }
        }

        string _positionString;
        public String PositionString
        {
            get { return _positionString; }
        }

        public string Scale
        {
            get { return scale; }
            set
            {
                if (scale != value)
                {
                    scale = value;
                    FirePropertyChanged("Scale");
                }
            }
        }
        public string Tracking
        {
            get { return tracking; }
            set
            {
                if (tracking != value)
                {
                    tracking = value;
                    FirePropertyChanged("Tracking");
                }
            }
        }

        public string Selected
        {
            get { return selected; }
            set
            {
                if (selected != value)
                {
                    selected = value;
                    FirePropertyChanged("Selected");
                }
            }
        }

        string getPositionString(PointD position)
        {
            char lonChar = 'E';
            char latChar = 'N';
            if (position.X < 0)
                lonChar = 'W';
            if (position.Y < 0)
                latChar = 'S';
            double lonD = Math.Abs(position.X);
            double lonM = (lonD - (int)lonD) * 60;
            double lonS = (lonM - (int)lonM) * 60;

            double latD = Math.Abs(position.Y);
            double latM = (latD - (int)latD) * 60;
            double latS = (latM - (int)latM) * 60;
            string str = string.Format("{0}°{1}′{2}″{3}, {4}°{5}′{6}″{7}", (int)lonD, ((int)lonM).ToString("00"), lonS.ToString("00.00"), lonChar, (int)latD, ((int)latM).ToString("00"), latS.ToString("00.00"), latChar);
            return str;
        }

        private void FirePropertyChanged(string propertyName)
        {
            if(this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
