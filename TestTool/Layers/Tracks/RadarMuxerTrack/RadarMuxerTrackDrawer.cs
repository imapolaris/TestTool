using System;
using System.Windows;
using VTSCore.Data.Common;
using VTSCore.Layers.Maps;

namespace VTSCore.Layers.Tracks
{
    public class RadarMuxerTrackDrawer : TracksCanvasDrawer
    {
        RadarMuxTrackReceiver _receiver = new RadarMuxTrackReceiver();
        MenuBarsBaseInfo _menuBarInfo;
        public RadarMuxerTrackDrawer(LocatorAndBorder locator)
            : base(locator)
        {
            SetColor(System.Windows.Media.Brushes.RoyalBlue, System.Windows.Media.Brushes.DarkGreen);
            EnumerateType = DynamicBaseCanvas.GeometryType.Circle;
            _receiver.TargetEvent += _receiver_TargetEvent;
            TimeOutHide = new TimeSpan(0, 0, 30);
            _menuBarInfo = MenuBarsBaseInfo.Instance;
            _menuBarInfo.PropertyChanged += _menuBarInfo_PropertyChanged;
        }

        void _menuBarInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "OnlyShowIdentifiedTrack")
                updateOnlyShowIdentifiedTrack();
        }

        public override void SetSetting(string setting)
        {
            string[] datas = setting.Split(',');
            if (datas.Length >= 2 && DataEligibleDetection.GetIpEndPoint(datas[0]) && DataEligibleDetection.GetIpEndPoint(datas[1]))
            {
                _receiver.Startup(datas[0], datas[1]);
            }
            else
                throw new InvalidCastException(setting + Environment.NewLine + "配置数据不合法，请重新配置！");
        }

        private void _receiver_TargetEvent(SeeCool.GISFramework.Object.RadarMuxTrack data)
        {
            lock (this)
            {
                if (data.Removed)
                    _tracksData.Remove(data.ID.ToString());
                else
                {
                    RadarMuxerTarget target = new RadarMuxerTarget(data.ID, data.MMSI);
                    target.Update(data.Longitude, data.Latitude, data.SOG, data.COG, DateTime.Now);
                    target.Name = data.Name;
                    target.OriginalObject = data;
                    target.ReceiverTime = data.DataTime;
                    _tracksData.UpdateDynamicEvent(target, data.Identified);
                }
            }
        }

        private void updateOnlyShowIdentifiedTrack()
        {
            lock(_tracksData)
            {
                _tracksData.UpdateIdentifiedTrack(_menuBarInfo.OnlyShowIdentifiedTrack);
            }
        }

        public void UpdateContextMenu(System.Windows.Controls.ContextMenu contextMenu, Point pt)
        {
            var targets = GetTargetsAtPoint(pt);
            if (targets.Length > 0)
                contextMenu.Items.Add(new System.Windows.Controls.Separator());
            foreach(var target in targets)
                _receiver.SetContextMenu(contextMenu, target as RadarMuxerTarget);
        }

        public override void Dispose()
        {
            base.Dispose();
            _receiver.ShutDown();
        }
    }
}
