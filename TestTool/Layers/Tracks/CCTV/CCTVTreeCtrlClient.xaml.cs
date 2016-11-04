using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CCTVClient;
using Common.Logging;

namespace VTSCore.Layers.Tracks.CCTV
{
    /// <summary>
    /// CCTVTreeCtrlClient.xaml 的交互逻辑
    /// </summary>
    public partial class CCTVTreeCtrlClient : Window
    {
        public CCTVTreeCtrlClient()
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }
        ILog LogService { get { return LogManager.GetLogger(GetType()); } }

        public CCTVClient.VideoParser.Node VideoNode{get;set;}
        public Action<VideoParser.Video> OnPlayVadio;

        private void TreeView_Loaded(object sender, RoutedEventArgs e)
        {
            if (VideoNode == null)
                return;
            cctvTreeView.Items.Add(updateNode(VideoNode));
            cctvTreeView.IsHitTestVisible = true;
        }

        private TreeViewItem updateNode(VideoParser.Node node)
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = node.Name;
            item.Tag = node;
            item.IsExpanded = true;
            VideoParser.Server server = node as CCTVClient.VideoParser.Server;
            if (server != null)
            {
                foreach (VideoParser.Node child in server.Childs)
                    item.Items.Add(updateNode(child));
            }
            else
            {
                VideoParser.Front front = node as VideoParser.Front;
                if (front != null)
                {
                    foreach (VideoParser.Video child in front.Childs)
                    {
                        item.Items.Add(getVideoNode(child, front.Online));
                    }
                }
            }
            return item;
        }

        TreeViewItem getVideoNode(VideoParser.Video node, bool online)
        {
            TreeViewItem item = new TreeViewItem();
            item.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            item.Header = node.Name;
            item.Tag = node;
            if (online)
            {
                item.Foreground = Brushes.Black;
                item.ToolTip = "在线";
            }
            else
            {
                item.Foreground = Brushes.DarkGray;
                item.ToolTip = "不在线";
            }
            return item;
        }

        private void cctvTreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.Source is TreeView)
                {
                    var node = e.Source as TreeView;
                    if (node.SelectedValue is TreeViewItem)
                    {
                        var value = node.SelectedValue as TreeViewItem;
                        if (!value.HasItems)
                        {
                            if (OnPlayVadio != null)
                            {
                                var video = value.Tag as VideoParser.Video;
                                if(video != null)
                                    OnPlayVadio(video);
                            }
                            //Console.WriteLine(video.Header.ToString() + "  " + video.Tag.ToString());
                        }
                    }
                    //Console.WriteLine(node.Header.ToString() + "  \t" + ((ulong)node.Tag).ToString() + "  " + node.IsSelected);
                }
            }
            catch(Exception ex)
            {
                LogService.Error(ex.ToString());
                MessageBox.Show(ex.Message);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            VTSCore.Common.WindowFrontor.Instance.FrontWindow();
        }

    }
}
