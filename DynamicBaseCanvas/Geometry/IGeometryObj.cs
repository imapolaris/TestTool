using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace DynamicBaseCanvas
{
    public interface IGeometryObj
    {
        double TransformHeading { set; }
        double OpacityInfo {get;set;}
        bool IsFill { get; set; }
    }
}
