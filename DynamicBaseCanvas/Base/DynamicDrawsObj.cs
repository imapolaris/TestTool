using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicBaseCanvas
{
    public abstract class DynamicDrawsObj: IDynamicObj, IDrawsObj
    {
        public bool IsTimeout 
        {
            get
            {
                return (Time - DateTime.Now > TimeSpan.FromSeconds(300));
            }
        }

        public DateTime Time { get; set; }

        public string Format()
        {
            return "";
        }

        public void Parse(string[] data)
        {
        }

        public string Name { get; set; }

        public double Lon { get; set; }

        public double Lat { get; set; }
    }
}
