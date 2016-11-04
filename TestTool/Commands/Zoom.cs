using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VTSCore.Commands
{
	public static class Zoom
	{
		public static RoutedUICommand ZoomOut { get; private set; }
		public static RoutedUICommand ZoomIn { get; private set; }
		public static RoutedUICommand Locate { get; private set; }
		public static RoutedUICommand Rotate { get; private set; }
        public static RoutedUICommand Ranges { get; private set; }
		public static RoutedUICommand SetAsCenter { get; private set; }

		static Zoom()
		{
			Util.CreateAllCommands(typeof(Zoom));

			//TODO 用attribute设置快捷键
			ZoomOut = create("放大", new KeyGesture(Key.Add));
			ZoomIn = create("缩小", new KeyGesture(Key.Subtract));
		}

		static RoutedUICommand create(string name, InputGesture gesture)
		{
			return new RoutedUICommand(name, "Zoom", typeof(Zoom), new InputGestureCollection(new InputGesture[] { gesture }));
		}
	}
}
