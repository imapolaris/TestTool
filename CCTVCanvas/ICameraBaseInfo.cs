using CCTVClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCTVCanvas
{
    public interface ICameraBaseInfo
    {
        void Update(VideoParser.Video video);
        void Update(int length, float zoomFactor);
    }
}
