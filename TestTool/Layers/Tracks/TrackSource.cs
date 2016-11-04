using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTSCore.Layers.Tracks
{
    public enum TrackType
    {
        None = -1,
        AIS = 0,
        AtlasVTS,
        Atlas2VTS,
        HittVTS,
        NorcontrolVTS,
        SofrelogVTS,
        视酷VTS,
        视酷雷达识别信号,
        视酷视频分析信号,
        湖州GPS,
        山东GPS,
        卫星AIS,
    }
    public class TrackDrawerSource
    {
        public static TracksCanvasDrawer NewTrackDrawer(TrackType type, VTSCore.Layers.Maps.LocatorAndBorder locator)
        {
            TracksCanvasDrawer drawer = null;
            switch (type)
            {
                case TrackType.AIS:
                    drawer = new AISTrackDrawer(locator);
                    break;
                case TrackType.AtlasVTS:
                    drawer = new VTSAtlasDrawer(locator);
                    break;
                case TrackType.Atlas2VTS:
                    drawer = new VTSAtlas2Drawer(locator);
                    break;
                case TrackType.HittVTS:
                    drawer = new VTSHittTrackDrawer(locator);
                    break;
                case TrackType.NorcontrolVTS:
                    drawer = new VTSNorcontrolTrackDrawer(locator);
                    break;
                case TrackType.SofrelogVTS:
                    drawer = new VTSSofrelogDrawer(locator);
                    break;
                case TrackType.视酷VTS:
                    drawer = new RadarMuxerTrackDrawer(locator);
                    break;
                case TrackType.视酷雷达识别信号:
                    drawer = new SeecoolRadarTrackDrawer(locator);
                    break;
                case TrackType.视酷视频分析信号:
                    drawer = new SeecoolVideoTrackDrawer(locator);
                    break;
                case TrackType.湖州GPS:
                    drawer = new HZGPSTrackDrawer(locator);
                    break;
                case TrackType.山东GPS:
                    drawer = new SDGPSDrawer(locator);
                    break;
                case TrackType.卫星AIS:
                    drawer = new WXAISDrawer(locator);
                    break;
            }
            return drawer;
        }
    }
}
