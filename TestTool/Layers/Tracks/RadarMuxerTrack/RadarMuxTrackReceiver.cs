using Common.Logging;
using Seecool.RemoteCall;
using Seecool.RemoteCall.PubSub;
using Seecool.RemoteCall.Serialization;
using SeeCool.GISFramework.Net;
using SeeCool.GISFramework.Object;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace VTSCore.Layers.Tracks
{
    public class RadarMuxTrackReceiver
    {
        public delegate void OnTarget(RadarMuxTrack track);
        public event OnTarget TargetEvent;

        ZmqSubClient<RadarMuxTrack> _subClient;
        ZmqRemoteCallClient _rpcClient = null;
        private IRadarMuxCommands _radarMuxCommands;
        ILog LogService { get { return LogManager.GetLogger(GetType()); } }
        public async void Startup(string subEndpoint, string rpcEndpoint)
        {
            await Task.Yield();
            IFormatter formatter = new JsonFormatter();
            _rpcClient = new ZmqRemoteCallClient(rpcEndpoint, formatter, TimeSpan.FromSeconds(3));
            _subClient = new ZmqSubClient<RadarMuxTrack>(subEndpoint, formatter, onRadarMuxTrack, "tracks");
            _radarMuxCommands = InterfaceProxy.CreateObject<IRadarMuxCommands>(_rpcClient, "Commands");
        }

        public void ShutDown()
        {
            if (_subClient != null)
            {
                _subClient.Dispose();
            }
            _subClient = null;
            _radarMuxCommands = null;
        }
        
        public void SetContextMenu(System.Windows.Controls.ContextMenu contextMenu, RadarMuxerTarget target)
        {
            if (target == null)
                return;
            MenuItem menu = new MenuItem() { Header = target.Name };
            if (string.IsNullOrWhiteSpace(target.Name))
                menu.Header = target.GetId();
            int id = target.ID;
            MenuItem item = new MenuItem { Header = "手动标注", Tag = id };
            item.Click += new RoutedEventHandler(manualIdentifyTrack_Click);
            menu.Items.Add(item);
            item = new MenuItem { Header = "取消手动标注", Tag = id };
            item.Click += new RoutedEventHandler(removeManualIdentify_Click);
            menu.Items.Add(item);
            item = new MenuItem { Header = "删除跟踪", Tag = id };
            item.Click += new RoutedEventHandler(removeTrack_Click);
            menu.Items.Add(item);
            contextMenu.Items.Add(menu);
        }

        private void manualIdentifyTrack_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            int id = (int)(menu.Tag);
            string header = (string)(menu.Parent as MenuItem).Header;
            var winAdd = new VTSCore.Layers.Base.ModifyValueClient("雷达融合目标标记名称", header);
            if (winAdd.ShowDialog().Value)
            {
                if (header != winAdd.Heading)
                    manualIdentifyTrack(id, winAdd.Heading);
            }
        }

        private void onRadarMuxTrack(RadarMuxTrack track)
        {
            OnTarget callback = TargetEvent;
            if (callback != null)
                callback(track);
        }

        private void removeManualIdentify_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            removeManualIdentify((int)(menu.Tag));
        }

        private void removeTrack_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            removeTrack((int)(menu.Tag));
        }

        async void manualIdentifyTrack(int id, string name)
        {
            await Task.Delay(0);
            try
            {
                if (_radarMuxCommands != null)
                {
                    _radarMuxCommands.ManualIdentifyTrack(id, name);
                }
            }
            catch(Exception ex)
            {
                LogService.Error("手动标注失败 \n" + ex.Message);
                MessageBox.Show("手动标注失败！");
            }
        }

        async void removeManualIdentify(int id)
        {
            await Task.Delay(0);
            try
            {
                if (_radarMuxCommands != null)
                {
                    _radarMuxCommands.RemoveManualIdenify(id);
                }
            }
            catch (Exception ex)
            {
                LogService.Error("移除手动标注失败\r\n" + ex.Message);
                MessageBox.Show("移除手动标注失败!");
            }
        }

        async void removeTrack(int id)
        {
            await Task.Delay(0);
            try
            {
                if (_radarMuxCommands != null)
                {
                    _radarMuxCommands.RemoveTrack(id);
                }
            }
            catch (Exception ex)
            {
                LogService.Error("删除目标失败\n" + ex.Message);
                MessageBox.Show("删除目标失败!");
            }
        }
    }
}
