using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicBaseCanvas
{
    public interface IDrawsObj
    {
        string Name { get; set; }
        double Lon { get; set; }
        double Lat { get; set; }
    }
}
