using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TargetInfomation;

namespace VTSCore.Layers.Tracks
{
    public class ScVideoTarget : MovableTarget
    {
        int ID;
        public ScVideoTarget(int id)
            : base()
        {
            ID = id;
        }

        public override string GetId()
        {
            return ID.ToString();
        }

        public override int GetHeading()
        {
            return (int)Math.Round(COG);
        }

        public override string GetDescription()
        {
            return string.Format("视频识别目标：ID({0}),{1}", GetId(), base.GetDescription());
        }

        public override string GetTitle()
        {
            return string.Format("ScVideo ID({0})", GetId());
        }
    }
}
