using CCTVClient;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using VTSCore.Data.Common;
using VTSCore.Layers.Maps;

namespace VTSCore.Layers.Tracks.CCTV
{
    class CCTVsCanvas : Canvas
    {
        LocatorAndBorder _locator;
        CCTVCanvas[] cctvs = new CCTVCanvas[0];
        StatusBarBaseInfomation _statusBarInfomation;
        VideoParser.Video _selected = null;
        ILog LogService { get { return LogManager.GetLogger(GetType()); } }
        public CCTVsCanvas(LocatorAndBorder locator, VideoParser.Video[] video, Dictionary<ulong, VideoParser.Camera> videoRealtime)
        {
            _locator = locator;
            cctvs = new CCTVCanvas[video.Length];
            _statusBarInfomation = StatusBarBaseInfomation.Instance;
            lock (videoRealtime)
            {
                for (int i = 0; i < cctvs.Length; i++)
                {
                    VideoParser.Camera camera = null;
                    if (videoRealtime.ContainsKey(video[i].Id))
                        camera = videoRealtime[video[i].Id];
                    cctvs[i] = new CCTVCanvas(video[i], camera, _locator);
                    this.Children.Add(cctvs[i]);
                    update(cctvs[i]);
                }
            }
        }

        public void UpdateVideoNodes(VideoParser.Video[] videos)
        {
            foreach (var video in videos)
            {
                var cctv = GetCCTV(video.Id);
                if (cctv != null)
                    cctv.UpdateVideo(video);
            }
        }

        public void OnMapRefreshed()
        {
            for (int i = 0; i < cctvs.Length; i++)
                update(cctvs[i]);
        }

        public void UpdateCameraAngle(VideoParser.Camera camera)
        {
            for (int i = 0; i < cctvs.Length; i++)
            {
                if (cctvs[i].Video.Id == camera.Id)
                {
                    this.Dispatcher.BeginInvoke((Action)delegate()
                    {
                        cctvs[i].Camera = camera;
                    });
                    break;
                }
            }
        }

        public VideoParser.Video SelectedOnPoint(System.Windows.Point point, out double dist)
        {
            VideoParser.Video key = null;
            dist = 1000;
            double distPrev = 1000;
            for (int i = 0; i < cctvs.Length; i++)
            {
                if (cctvs[i].NeerPoint(point, ref distPrev))
                {
                    if (dist > distPrev)
                    {
                        dist = distPrev;
                        key = cctvs[i].Video;
                    }
                }
            }
            return key;
        }

        public VideoParser.Video Selected
        {
            get { return _selected; }
            set
            {
                if(Selected != null)
                    Unchecked(Selected.Id);
                Checked(value);
            }
        }

        public void UpdateStatus()
        {
            try
            {
                if(Selected == null)
                    return;
                var cctv = GetCCTV(Selected.Id);
                if (cctv != null)
                    updateSelectedString(cctv.String());
                else
                    updateSelectedString(null);
            }
            catch(Exception ex)
            {
                LogService.Error(ex.ToString());
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void update(CCTVCanvas cctv)
        {
            this.Dispatcher.BeginInvoke((Action)delegate()
            {
                if (_locator.InScreenAtCircle(cctv.Video.PanTiltUnit.Longitude, cctv.Video.PanTiltUnit.Latitude, CCTVCanvas.maxrange))
                {
                    cctv.OnMapRefreshed();
                    cctv.Visibility = System.Windows.Visibility.Visible;
                }
                else
                    cctv.Visibility = System.Windows.Visibility.Collapsed;
            });
        }

        private void Checked(VideoParser.Video video)
        {
            if (video == null)
                return;
            var cctv = GetCCTV(video.Id);
            if(cctv != null)
            {
                cctv.Selected = true;
                _selected = video;
            }
        }

        private void Unchecked(ulong value)
        {
            var cctv = GetCCTV(value);
            if (cctv != null)
                cctv.Selected = false;
            _selected = null;
        }

        private void updateSelectedString(string status)
        {
            if (status != null)
                _statusBarInfomation.Selected = string.Format("选中: {0}", status);
            else
                _statusBarInfomation.Selected = string.Format("选中");
        }

        private CCTVCanvas GetCCTV(ulong id)
        {
            if (id == 0)
                return null;
            lock (cctvs)
            {
                foreach (var cctv in cctvs)
                {
                    if (cctv.Video.Id == id)
                        return cctv;
                }
            }
            return null;
        }
    }
}