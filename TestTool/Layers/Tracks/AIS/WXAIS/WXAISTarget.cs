using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TargetInfomation;

namespace VTSCore.Layers.Tracks
{
    public class WXAISTarget: MovableTarget
    {
        public int Heading = -1;
        public int NavStatus;
        public string Src;
        public int TimeStamp;
        public int AtoNType;
        public bool PositionAccuracy;
        public bool RAIM_Flag;

        public WXAISTarget(int mmsi) :
            base()
        {
            MMSI = mmsi;
        }

        public override string GetId()
        {
            return MMSI.ToString();
        }

        public override int GetHeading()
        {
            if (Heading >= 0 && Heading < 360)
                return Heading;
            else
                return (int)Math.Round(COG);
        }

        public override string GetDescription()
        {
            return string.Format("WXAIS: MMSI({0}),{1}", MMSI, base.GetDescription());

        }

        public override string GetTitle()
        {
            return string.Format("WXAIS MMSI({0})", MMSI);
        }
    }
}
