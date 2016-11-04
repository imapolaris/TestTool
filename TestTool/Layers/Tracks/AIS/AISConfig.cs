using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTSCore.Data.Common;

namespace VTSCore.Layers.Tracks
{
    public class AISConfig : ConfigBase
    {
        public AISConfig(): base()
        {
            Port = 8040;
        }
    }
}
