using SeeCool.GISFramework.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TargetInfomation;

namespace VTSCore.Layers.Tracks
{
    public class HZGPSTarget : MovableDynamicTarget
    {
        public HZGPSTarget()
            : base()
        {
        }

        public override string GetId()
        {
            return Name;
        }

        public override string GetDescription()
        {
            return string.Format("HZGPS：{0}", base.GetDescription());
        }

        public override string GetTitle()
        {
            return string.Format("HZGPS 船名({0})", Name);
        }
    }
}
