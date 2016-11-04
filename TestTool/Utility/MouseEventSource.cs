using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VTSCore.Utility
{
	public interface IMouseEventSource
	{
		IObservable<Point> MouseDown { get; }
		IObservable<Point> MouseUp { get; }
		IObservable<Point> MouseMove { get; }
		IObservable<Point> MouseDoubleClick { get; }
		IObservable<Point> MouseDragDrop { get; }
		IObservable<int> MouseWheel { get; }
        IObservable<Point> MouseRightDown { get; }
	}

    public class MouseEventSource : IMouseEventSource
    {
        public MouseEventSource(FrameworkElement mouseEventSource)
        {
            MouseDown = from evt in Observable.FromEventPattern<MouseButtonEventArgs>(mouseEventSource, "MouseLeftButtonDown")
                        select evt.EventArgs.GetPosition(mouseEventSource);

            MouseDoubleClick = from evt in Observable.FromEventPattern<MouseButtonEventArgs>(mouseEventSource, "MouseLeftButtonDown")
                               where evt.EventArgs.ClickCount == 2
                               select evt.EventArgs.GetPosition(mouseEventSource);

            MouseUp = from evt in Observable.FromEventPattern<MouseButtonEventArgs>(mouseEventSource, "MouseLeftButtonUp")
                      select evt.EventArgs.GetPosition(mouseEventSource);

            MouseMove = from evt in Observable.FromEventPattern<MouseEventArgs>(mouseEventSource, "MouseMove")
                        select evt.EventArgs.GetPosition(mouseEventSource);

            MouseWheel = from evt in Observable.FromEventPattern<MouseWheelEventArgs>(mouseEventSource, "MouseWheel")
                         select evt.EventArgs.Delta;

            MouseRightDown = from evt in Observable.FromEventPattern<MouseButtonEventArgs>(mouseEventSource, "MouseRightButtonDown")
                             select evt.EventArgs.GetPosition(mouseEventSource);
        }

        public IObservable<Point> MouseDown { get; private set; }
        public IObservable<Point> MouseUp { get; private set; }
        public IObservable<Point> MouseMove { get; private set; }
        public IObservable<Point> MouseDoubleClick { get; private set; }
        public IObservable<Point> MouseDragDrop { get { return MouseMove.TakeUntil(MouseUp); } }
        public IObservable<int> MouseWheel { get; private set; }
        public IObservable<Point> MouseRightDown { get; private set; }
    }
}
