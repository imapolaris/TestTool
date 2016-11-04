using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicBaseCanvas
{
    public interface IDynamicObj
    {
        bool IsTimeout { get; }
        DateTime Time { get; set; }

        string Format();
        void Parse(string[] data);
    }
}
