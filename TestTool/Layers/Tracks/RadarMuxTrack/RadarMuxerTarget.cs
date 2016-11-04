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

        public RadarMuxTrack MuxTrack { get { return OriginalObject as RadarMuxTrack; } }

        public RadarMuxerTarget(int id, int mmsi) : base()
        {
            ID = id;
            MMSI = mmsi;
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
            StringBuilder radars = new StringBuilder();
            foreach (int radar in MuxTrack.Radars)
            {
                radars.Append(radar);
                radars.Append(',');
            }
            if (radars.Length > 0)
                radars.Remove(radars.Length - 1, 1);
            return string.Format("雷达融合目标：ID({0}), MMSI({1}), 船名({2}), 经度({3}), 纬度({4}), 航速({5}节), 航向({6}), 雷达({7})", GetId(), MMSI, Name, Lon, Lat, SOG, COG, radars.ToString());
        }

        public override string GetTitle()
        {
            return string.Format("RadarMuxer ID,MMSI({0},{1})", GetId(), MMSI);
        }
    }
}
