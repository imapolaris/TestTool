using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TargetInfomation
{
    public interface IMovableTarget
    {
        string GetId();
        int MMSI { get; }
        double Lat { get; }
        double Lon { get; }
        double SOG { get; }
        double COG { get; }
        int GetHeading();
        bool IsTimeout();
        DateTime UpdateTime { get; }
        string GetDescription();
        string GetTitle();
    }
}
