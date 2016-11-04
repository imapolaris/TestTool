using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VTSCore.Commands;

namespace VTSCore.Layers.Maps.Parts
{
	class MoveByKey : IPart
	{
		[Import]
		SeaMapInfo _seaMapInfo = null;

		[Import]
		ILocator _locator = null;

		public void Init()
		{
			DispatchKeyEvent();
		}

		async void DispatchKeyEvent()
		{
			var map = _seaMapInfo.MapFrame;

			var movePoints = new Dictionary<Key, Point>();
			movePoints[Key.Left] = new Point(1, 0);
			movePoints[Key.Right] = new Point(-1, 0);
			movePoints[Key.Up] = new Point(0, 1);
			movePoints[Key.Down] = new Point(0, -1);

			var directionKeyDown = from i in Observable.FromEventPattern<KeyEventArgs>(map, "KeyDown")
								   let key = i.EventArgs.Key
								   where movePoints.ContainsKey(key)
								   select movePoints[key];

			while (true)
			{
				var p = await directionKeyDown.FirstAsync();

				var dx = p.X;
				var dy = p.Y;

				bool isShiftKeyDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
				if (isShiftKeyDown)
				{
					dx *= map.ActualWidth / 25;
					dy *= map.ActualHeight / 25;
				}
				else
				{
					dx *= map.ActualWidth / 4;
					dy *= map.ActualHeight / 4;
				}


				await _locator.Offset(dx, dy);

				//防止方向键移动焦点到其它控件上
				map.Focus();
			}
		}
	}
}
