using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VTSCore.Commands
{
	public static class Setting
	{
		public static RoutedUICommand OperatorSetting { get; private set; }
        public static RoutedUICommand SystemSetting { get; private set; }
		public static RoutedUICommand NewHarbor { get; private set; }
		public static RoutedUICommand NewRestrictedArea { get; private set; }
		public static RoutedUICommand NewResponsibilityArea { get; private set; }
		public static RoutedUICommand Test { get; private set; }
        
		static Setting()
		{
			Util.CreateAllCommands(typeof(Setting));
		}
	}
}
