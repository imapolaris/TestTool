using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VTSCore.Commands
{
	public static class TrafficEnv
	{
		public static RoutedUICommand New { get; private set; }

		public static RoutedUICommand Blink { get; private set; }		//闪呀闪

		//public static RoutedUICommand Start { get; private set; }
		//public static RoutedUICommand EditProperty { get; private set; }
		//public static RoutedCommand VisibilitySetting { get; private set; }

		static TrafficEnv()
		{
			Util.CreateAllCommands(typeof(TrafficEnv));
		}
	}
}
