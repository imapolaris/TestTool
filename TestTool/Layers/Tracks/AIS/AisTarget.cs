using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TargetInfomation;

namespace VTSCore.Layers.Tracks
{
    public class AisTarget : MovableTarget
    {
        public int Heading = -1;
        public int Length = 0;

        public AisTarget(int mmsi) :
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
            string length = "";
            if (Length > 0)
                length = string.Format(", 船长({0}米)", Length);
            string heading = "";
            if (Heading >= 0 && Heading < 360)
                heading = string.Format(", 船首向({0}°)", GetHeading());
            return string.Format("AIS ID({0}),MMSI({1}),{2}{3}{4}", GetId(), MMSI, base.GetDescription(), length, heading);
        }

        public override string GetTitle()
        {
            return string.Format("AIS MMSI({0})", MMSI);
        }
    }
}
