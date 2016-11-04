using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VTSCore.Layers.Maps.Parts
{
	class SetAsCenter : IPart
	{
		[Import]
		SeaMapInfo _seaMapInfo = null;

		[Import]
		ILocator _locator = null;

		public void Init()
		{
			_seaMapInfo.MapFrame.AddCommandBinding(Commands.Zoom.SetAsCenter, SetAsCenter_Excuted);
		}

		[DllImport("User32")]
		public extern static void SetCursorPos(int x, int y);


		private async void SetAsCenter_Excuted(object sender, ExecutedRoutedEventArgs e)
		{
			var pos = Mouse.GetPosition(_seaMapInfo.MapFrame);
			await _locator.SetAsCenter(pos);

			//SetCursorPos((int)pos.X, (int)pos.Y);
		}
	}
}
