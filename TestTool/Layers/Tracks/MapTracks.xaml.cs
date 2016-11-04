using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VTSCore.Data.Common;
using VTSCore.Layers.Maps;

namespace VTSCore.Layers.Tracks
{
    /// <summary>
    /// MapAIS.xaml 的交互逻辑
    /// </summary>
    public partial class MapTracks : Canvas
    {
        public MapTracks()
        {
            InitializeComponent();
        }

        [Import]
        VTSCore.Utility.IMouseEventSource _mouseEventSource;
        
        [Import]
        Maps.IMapNotification _mapNotification = null;

        [Import]
        System.Windows.Controls.ContextMenu _contextMenu;

        TracksDraws _track;
        LocatorAndBorder _locatorBorder;
        protected override async void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (WindowUtil.IsDesingMode())
                return;
            await Task.Yield();
            await _mapNotification.InitCompletion;

            _locatorBorder = LocatorAndBorder.Instance;
            _track = new TracksDraws(_locatorBorder.Locator);
            this.Children.Add(_track);
            _mouseEventSource.MouseDown.Subscribe(downMouse);
            _mouseEventSource.MouseDoubleClick.Subscribe(doubleMouse);
            _mouseEventSource.MouseRightDown.Subscribe(rightMouse);
            this.ClipToBounds = true;
        }

        #region 右键菜单

        private void rightMouse(Point point)
        {
            _track.MouseRightButton(point, _contextMenu);
        }
        void MapTracks_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        }
        #endregion

        #region 鼠标事件、海图重绘

        public void downMouse(Point point)
        {
            _track.CheckSelectedPoint(point);
        }
       
        private void doubleMouse(Point point)
        {
            _track.DoubleMouse(point);
        }
        #endregion
    }
}