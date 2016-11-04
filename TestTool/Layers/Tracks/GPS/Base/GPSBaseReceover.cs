using SeeCool.GISFramework.Object;
using SeeCool.GISFramework.SvrFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTSCore.Layers.Tracks
{
    public class GPSBaseReceover : PluginObject
    {
        public event Action<ShipObj> OnReceivedData;

        public string Url { get; set; }

        protected void fireOnDynamic(ShipObj obj)
        {
            if (OnReceivedData != null)
                OnReceivedData(obj);
        }

        protected override void doShutdown()
        {
            
        }

        protected override void doStartup()
        {
            
        }
    }
}
