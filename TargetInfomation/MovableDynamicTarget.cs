using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TargetInfomation
{
    public abstract class MovableDynamicTarget : MovableTarget
    {
        public MovableDynamicTarget()
            : base()
        {
        }

        public override int GetHeading()
        {
            return (int)Math.Round(COG);
        }

        public override string GetId()
        {
            return MMSI.ToString();
        }

        public override string GetTitle()
        {
            return string.Format("MMSI({0})", MMSI);
        }
    }
}
