using CCTVClient;
using DynamicBaseCanvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCTVCanvas
{
    public class CameraGeometryObj : GeometryObj, ICameraBaseInfo
    {
        public CameraGeometryObj(VideoParser.Video video, VideoParser.Camera camera)
        {
            
        }

        public void Update(CCTVClient.VideoParser.Video video)
        {
        }

        public void Update(int length, float zoomFactor)
        {
        }
    }
}