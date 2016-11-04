using SeeCool.GISFramework.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TargetInfomation;

namespace VTSCore.Layers.Tracks
{
    public class VTSNorcontrolTarget : MovableDynamicTarget
    {
        public string Id;
        public VTSNorcontrolTarget(string id)
            : base()
        {
            Id = id;
        }

        public override string GetId()
        {
            return Id;
        }

        public override string GetDescription()
        {
            return string.Format("Norcontrol VTS ：ID({0}),{1}", GetId(), base.GetDescription());
        }

        public override string GetTitle()
        {
            return string.Format("Norcontrol VTS ID({0})", Id);
        }
    }
}
