using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TargetInfomation;

namespace VTSCore.Layers.Tracks
{
    public class ScRadarTarget : MovableTarget
    {
        public int Id { get; set; }
        public ScRadarTarget(int id)
            : base()
        {
            Id = id;
        }

        public override string GetId()
        {
            return Id.ToString();
        }

        public override int GetHeading()
        {
            return (int)Math.Round(COG);
        }

        public override string GetDescription()
        {
            return string.Format("ScRadar： ID({0}),MMSI({1}){2}", GetId(), MMSI, base.GetDescription());
        }

        public override string GetTitle()
        {
            return string.Format("ScRadar MMSI({0})", MMSI);
        }
    }
}
