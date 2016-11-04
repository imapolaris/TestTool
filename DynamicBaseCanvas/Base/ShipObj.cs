using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicBaseCanvas
{
    [Serializable]
    public abstract class ShipObj : DynamicMovableDrawsObj
    {
        protected ShipObj() : base() { }

        public int Heading;
        public int MMSI;
        
        public int GetHeading()
        { return Heading; }
    }
}
