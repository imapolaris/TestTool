using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TargetInfomation
{
    public abstract class MovableTarget : IMovableTarget, INotifyPropertyChanged, IDisposable
    {
        public static TimeSpan TimeOutSpan = TimeSpan.FromMinutes(60);
        public Queue<MovableTarget> History = new Queue<MovableTarget>();
        
        public abstract string GetId();
        public int MMSI { get; set; }

        public double Lat { get; set; }
        public double Lon { get; set; }
        public double SOG { get; set; }
        public double COG { get; set; }
        public string Name = string.Empty;
        public int Type = 0;
        public bool TimeOut { get; set; }
        public DateTime UpdateTime { get; set; }
        public DateTime ReceiverTime { get; set; }

        public object OriginalObject;

        public MovableTarget()
        {
            Lat = -91;
            Lon = -181;
            UpdateTime = DateTime.Now;
            TimeOut = false;
            ReceiverTime = DateTime.Now;
        }

        public abstract int GetHeading();

        public virtual string GetDescription()
        {
            return string.Format("MMSI({0}),船名({1}),经度({2}),纬度({3}),航速({4}节),航向({5}°)", MMSI, Name, Lon.ToString("F6"), Lat.ToString("F6"), SOG.ToString("F2"), COG.ToString("F1"));
        }

        public abstract string GetTitle();

        public virtual bool IsTimeout()
        {
            return DateTime.Now - UpdateTime > TimeOutSpan;
        }

        public virtual void Update(MovableTarget target)
        {
            if (!string.IsNullOrEmpty(target.Name))
                Name = target.Name;
            MMSI = target.MMSI;
            Lon = target.Lon;
            Lat = target.Lat;
            SOG = target.SOG;
            COG = target.COG;
            UpdateTime = target.UpdateTime;
            OriginalObject = target.OriginalObject;
            removeTimeOutData();
            updateHistoryData(target.ShallowClone());
            if (_historyChanged)
                fireUpdate();
            else
                fireUpdateLastData();
            _historyChanged = false;
        }

        private void updateHistoryData(MovableTarget movableTarget)
        {
            if (History.Count == 0)
                historyEnqueue(movableTarget);
            else
            {
                var last = History.Last();
                if (Math.Abs(last.Lon - movableTarget.Lon) < 0.00001 && Math.Abs(last.Lat - movableTarget.Lat) < 0.00001)
                    last.UpdateTime = movableTarget.UpdateTime;
                else if (last.UpdateTime == movableTarget.UpdateTime)
                    return;
                else
                    historyEnqueue(movableTarget);
            }
        }

        public void Update(double lon, double lat, double sog, double cog, DateTime time)
        {
            Lon = lon;
            Lat = lat;
            SOG = sog;
            COG = cog;
            UpdateTime = time;
        }

        public virtual MovableTarget Clone()
        {
            var target = (MovableTarget)MemberwiseClone();
            target.History = new Queue<MovableTarget>();
            if(History.Count > 0)
            {
                foreach (var his in History)
                    target.History.Enqueue(his.ShallowClone());
            }
            return target;
        }

        public virtual MovableTarget ShallowClone()
        {
            var target = (MovableTarget)MemberwiseClone();
            target.History = new Queue<MovableTarget>();
            target.OriginalObject = null;
            return target;
        }

        public void Invisible()
        {
            firePropertyChanged("Invisible");
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        private void firePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            OriginalObject = null;
            if(History != null)
                History.Clear();
            TimeOut = true;
            Invisible();
        }

        bool _historyChanged;

        void historyEnqueue(MovableTarget movableTarget)
        {
            History.Enqueue(movableTarget);
            _historyChanged = true;
        }

        void removeTimeOutData()
        {
            while (History.Count > 0)
            {
                if (History.Peek().IsTimeout())
                {
                    History.Dequeue();
                    _historyChanged = true;
                }
                else
                    break;
            }
        }

        void fireUpdate()
        {
            firePropertyChanged("UpdateData");
        }

        void fireUpdateLastData()
        {
            firePropertyChanged("UpdateDataLast");
        }
    }
}
