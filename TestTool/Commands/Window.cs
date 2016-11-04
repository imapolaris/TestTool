using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VTSCore.Commands
{
	public class Window
	{
		public static RoutedUICommand New { get; private set; }
		public static RoutedUICommand NewTrack { get; private set; }		//重点跟踪窗口
		public static RoutedUICommand Close { get; private set; }
		public static RoutedUICommand CloseAll { get; private set; }
        public static RoutedUICommand ModifyTitle { get; private set; }

		static Window()
		{
			Util.CreateAllCommands(typeof(Window));
		}
	}
}
