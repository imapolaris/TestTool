using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VTSCore.Data.Common;
using VTSCore.Utility;

namespace VTSCore.Layers.Maps.Parts
{
	//提供Seamap的基本信息，供其它组件模块调用
	class SeaMapInfo
	{
		SeaMap _seaMap;
		public SeaMapInfo(SeaMap seaMap)
		{
			this._seaMap = seaMap;

			this.MapFrame = seaMap;
		}

		public FrameworkElement MapFrame { get; private set; }
	}
}
