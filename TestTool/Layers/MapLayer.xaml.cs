using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Reactive.Linq;
using VTSCore.Data.Common;

namespace VTSCore.Layers
{
	/// <summary>
	/// MapLayer.xaml 的交互逻辑
	/// </summary>
	public partial class MapLayer : UserControl, IDisposable
	{
		public MapLayer()
		{
			InitializeComponent();
		}

		[Export]
		VTSCore.Utility.IMouseEventSource _mouseEventSource;

        [Export]
        System.Windows.Controls.ContextMenu _contextMenu;

        ActivatingStatus activatingStatus;
		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			if (VTSCore.WindowUtil.IsDesingMode())
				return;
            activatingStatus = ActivatingStatus.Instance;
            rect.Fill = Brushes.Red;
            rect.Opacity = 0.1;
            rect.Visibility = System.Windows.Visibility.Collapsed;
            _contextMenu = contextMenu;
			_mouseEventSource = new VTSCore.Utility.MouseEventSource(this);

            _mouseEventSource.MouseRightDown.Subscribe(rightMouse);
			using (var catalog = new AssemblyCatalog(this.GetType().Assembly))
			using (var container = new CompositionContainer(catalog))
			{
                container.ComposeParts(this, map, plot, radar, lines, tracks);
			}

			DispatchDragDrop();
		}

        private void rightMouse(Point point)
        {
            _contextMenu.Items.Clear();
        }

		void DispatchDragDrop()
		{
            var triggerButton = MouseButton.Left;
            var dragDrop = this.GetDragDropEventSource(triggerButton);
            dragDrop.Subscribe(processDragDrop);
		}

        ChartsStatus _chartStatus = ChartsStatus.移动海图;

        void processDragDrop(IObservable<Point> dragDrop)
        {
            if (Unavailable())
                return;
            if (processDragDropControl(dragDrop))
                return;
            Mouse.Capture(this);
            dragDrop = dragDrop.Skip(1).Zip(dragDrop, (i1, i2) => new Point(i1.X - i2.X, i1.Y - i2.Y));
            dragDrop.Subscribe(
                p =>
                {
                    if (activatingStatus.TragStatus == TraggingStatus.海图模式 && activatingStatus.ChartStatus == ChartsStatus.移动海图)
                    {
                        transFrom.X += p.X;
                        transFrom.Y += p.Y;
                    }
                },

                async () =>
                {
                    Mouse.Capture(null);
                    if (transFrom.X == 0 && transFrom.Y == 0)
                        return;
                    await map.Locator.Offset(transFrom.X, transFrom.Y);
                    transFrom.X = 0;
                    transFrom.Y = 0;
                });
        }

        bool processDragDropControl(IObservable<Point> dragDrop)
        {
            bool beginControlKey = ControlKey() && !ShiftKey();
            bool beginShiftKey = !ControlKey() && ShiftKey();
            if (beginControlKey || beginShiftKey)
            {
                _chartStatus = activatingStatus.ChartStatus;
                activatingStatus.ChartStatus = ChartsStatus.缩放海图;
                Mouse.Capture(this);
                Point pointBegin = Mouse.GetPosition(this);
                Point point = new Point();

                rect.Margin = new Thickness(pointBegin.X, pointBegin.Y, 0, 0);
                rect.Width = 0;
                rect.Height = 0;
                rect.Visibility = System.Windows.Visibility.Visible;
                dragDrop = dragDrop.Skip(1).Zip(dragDrop, (i1, i2) => new Point(i1.X - i2.X, i1.Y - i2.Y));
                dragDrop.Subscribe(
                    p =>
                    {
                        if (activatingStatus.ChartStatus == ChartsStatus.缩放海图)
                        {
                            point.X += p.X;
                            point.Y += p.Y;
                            rect.Width = Math.Abs(point.X);
                            rect.Height = Math.Abs(point.Y);
                            if (point.X < 0 || point.Y < 0)
                                rect.Margin = new Thickness(pointBegin.X + Math.Min(0, point.X), pointBegin.Y + Math.Min(0, point.Y), 0, 0);
                        }
                    },

                    async () =>
                    {
                        Mouse.Capture(null);
                        MapPoint position = map.Locator.ScreenToMap(pointBegin.X + point.X / 2, pointBegin.Y + point.Y / 2);
                        double zoom = Math.Max(Math.Abs(point.X) / this.ActualWidth, Math.Abs(point.Y) / this.ActualHeight);
                        double scale = map.Locator.Scale;
                        zoom = Math.Max(zoom, 0.01);
                        scale =  beginControlKey ? scale * zoom : scale / zoom;
                        await map.Locator.Locate(scale, position);
                        activatingStatus.ChartStatus = _chartStatus;
                        rect.Visibility = System.Windows.Visibility.Collapsed;
                    });
                return true;
            }
            return false;
        }

        private static bool ControlKey()
        {
            return (System.Windows.Forms.Control.ModifierKeys & System.Windows.Forms.Keys.Control) == System.Windows.Forms.Keys.Control;
        }

        private static bool ShiftKey()
        {
            return (System.Windows.Forms.Control.ModifierKeys & System.Windows.Forms.Keys.Shift) == System.Windows.Forms.Keys.Shift;
        }

        private bool Unavailable()
        {
            return activatingStatus.ChartStatus != ChartsStatus.移动海图;
        }

        public void SaveIfChanged()
        {
            plot.SaveIfChanged();
            radar.SaveIfChanged();
        }

        public async void Dispose()
        {
            await System.Threading.Tasks.Task.Yield();
            radar.Dispose();
        }
    }
}
