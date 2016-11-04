using CCTVClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTSCore.Layers.Tracks.CCTV
{
    class CCTVNodeTree
    {
        private Dictionary<ulong, VideoParser.Video> _trackVideos = new Dictionary<ulong, VideoParser.Video>();
        
        public void UpdateTree(VideoParser.Node node)
        {
            lock (_trackVideos)
            {
                _trackVideos.Clear();
                AddTrackVideo(node);
            }
        }

        public bool IsSameNode(VideoParser.Node node)
        {
            lock (_trackVideos)
            {
                if (_trackVideos.Count == 0)
                    return false;
                CCTVNodeTree newNode = new CCTVNodeTree();
                newNode.AddTrackVideo(node);
                if (isSameVideos(newNode.GetAllTrackVideos()))
                    return true;
                return false;
            }
        }

        private bool isSameVideos(VideoParser.Video[] videos)
        {
            if (_trackVideos.Count != videos.Length)
                return false;
            for (int i = 0; i < videos.Length; i++)
            {
                if (!_trackVideos.ContainsKey(videos[i].Id))
                    return false;
            }
            return true;
        }
        
        public VideoParser.Video[] GetAllTrackVideos()
        {
            lock (_trackVideos)
                return _trackVideos.Values.ToArray();
        }

        public void AddTrackVideo(VideoParser.Node node)
        {
            VideoParser.Server server = node as VideoParser.Server;
            if (server != null)
            {
                foreach (VideoParser.Node child in server.Childs)
                    AddTrackVideo(child);
            }
            else
            {
                VideoParser.Front front = node as VideoParser.Front;
                if (front != null)
                {
                    foreach (VideoParser.Video child in front.Childs)
                        addTrackVideo(child);
                }
            }
        }

        private void addTrackVideo(VideoParser.Video video)
        {
            if (isTrackable(video))
                _trackVideos.Add(video.Id, video);
        }

        static bool isTrackable(VideoParser.Video video)
        {
            return video != null && video.PanTiltUnit != null
                && (video.PanTiltUnit.Protocol == "USNT-ICU1"
                || video.PanTiltUnit.Protocol == "USNT-TRX18D"
                || video.PanTiltUnit.Protocol == "FY-SP2018"
                || video.PanTiltUnit.Trackable);
        }
    }
}
