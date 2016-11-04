using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicBaseCanvas
{
    [Serializable]
    public abstract class CompositeDynamicObj : DynamicDrawsObj
    {
        public List<DynamicDrawsObj> RelatedObjects;

        protected CompositeDynamicObj()
        {
            RelatedObjects = new List<DynamicDrawsObj>();
        }
    }
}
