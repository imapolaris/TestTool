using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace VTSCore.Layers.Radar
{
	public class ColorTableData
	{
		public Color FrontStart { get; set; }	//回波 
		public Color FrontEnd { get; set; }

		public Color TrailStart { get; set; }	//余晖
		public Color TrailEnd { get; set; }

		public int TrailState { get; set; }		//余晖级数

		public ColorTableData()
		{
			this.TrailState = 10;
			this.FrontStart = (Color)ColorConverter.ConvertFromString("#FF8DB3E3"); 
			this.FrontEnd = (Color)ColorConverter.ConvertFromString("#FFFFFFFF");
			this.TrailStart = (Color)ColorConverter.ConvertFromString("#FF006400");
			this.TrailEnd = (Color)ColorConverter.ConvertFromString("#FF8DB3E3");
		}

        public bool IsSameWith(ColorTableData color)
        {
            if (TrailState == color.TrailState && FrontStart == color.FrontStart && FrontEnd == color.FrontEnd && TrailStart == color.TrailStart && TrailEnd == color.TrailEnd)
                return true;
            return false;
        }
    }
}
