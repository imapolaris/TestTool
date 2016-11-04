using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicBaseCanvas
{
    [Serializable]
    public abstract class DynamicMovableDrawsObj: CompositeDynamicObj
    {
        public double COG;
        public double SOG;

        protected DynamicMovableDrawsObj():base()
        {

        }
    }
}
