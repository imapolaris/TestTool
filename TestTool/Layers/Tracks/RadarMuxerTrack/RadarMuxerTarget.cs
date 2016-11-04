using Common.Logging;
using SeeCool.GISFramework.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TargetInfomation;

namespace VTSCore.Layers.Tracks
{
    public class RadarMuxerTarget : MovableTarget
    {
        public int ID = 0;
        public static bool IsStartUpVerify { get; set; }
        MovableTargetVerify _verify;
        public RadarMuxTrack MuxTrack { get { return OriginalObject as RadarMuxTrack; } }
        ILog LogService { get { return LogManager.GetLogger(GetType()); } }

        public RadarMuxerTarget(int id, int mmsi) : base()
        {
            ID = id;
            MMSI = mmsi;
            _verify = new MovableTargetVerify();
        }

        public override string GetId()
        {
            return ID.ToString();
        }

        public override void Update(MovableTarget target)
        {
            base.Update(target);
            if (IsStartUpVerify && !_verify.Update(target.Lon, target.Lat, target.UpdateTime))
                LogService.Error("目标移动距离验证失败！" + this.GetDescription());
        }

        public override int GetHeading()
        {
            return (int)Math.Round(COG);
        }
        
        public override string GetDescription()
        {
            StringBuilder radars = new StringBuilder();
            foreach (int radar in MuxTrack.Radars)
            {
                radars.Append(radar);
                radars.Append(',');
            }
            if (radars.Length > 0)
                radars.Remove(radars.Length - 1, 1);
            return string.Format("视酷VTS：ID({0}),{1},雷达({2}),确认({3})", GetId(), base.GetDescription(), radars.ToString(), MuxTrack.Identified);
        }

        public override string GetTitle()
        {
            return string.Format("RadarMuxer ID,MMSI({0},{1})", GetId(), MMSI);
        }
    }
}
