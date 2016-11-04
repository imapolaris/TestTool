using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTSCore.Data.Common;

namespace VTSCore.Layers.Tracks
{
    public class VideoRecognizeConfig: ConfigBase
    {
        public VideoRecognizeConfig()
            : base()
        {
            Port = 60004;
        }
    }
}
