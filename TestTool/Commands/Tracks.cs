using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VTSCore.Commands
{
	public static class Tracks
	{
		public static RoutedUICommand ShowInfo { get; private set; } 
		public static RoutedUICommand Track { get; private set; }
		public static RoutedUICommand TrackAngle { get; private set; }
		public static RoutedUICommand TrackListSummary { get; private set; }
		public static RoutedUICommand TrackListInfo { get; private set; }

		public static RoutedUICommand ShowLabel { get; private set; }
		public static RoutedUICommand ApplyDisplaySetting { get; private set; }
        public static RoutedUICommand ShowSubjectVessel { get; private set; }

		static Tracks()
		{
			Util.CreateAllCommands(typeof(Tracks));
		}
	}
}
