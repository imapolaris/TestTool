using SeeCool.GISFramework.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TargetInfomation;

namespace VTSCore.Layers.Tracks
{
    public class VTSAtlasTarget : MovableDynamicTarget
    {
        public int Id;
        public VTSAtlasTarget(int id)
            : base()
        {
            Id = id;
        }

        public override string GetId()
        {
            return Id.ToString();
        }

        public override string GetDescription()
        {
            return string.Format("Atlas VTS：ID({0}), MMSI({1}),{2}", GetId(), MMSI, base.GetDescription());
        }

        public override string GetTitle()
        {
            return string.Format("Atlas VTS MMSI({0})", MMSI);
        }
    }
}
