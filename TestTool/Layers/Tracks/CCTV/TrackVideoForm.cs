using CCTVClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VTSCore.Layers.Tracks.CCTV
{
    public partial class TrackVideoForm : Form
    {
        private CCTVInfo _info;
        private int _bandwidth = 2000000;
        private TrackAdjustment _adjustment;

        public TrackVideoForm(Form owner, CCTVInfo info, int bandwidth, TrackAdjustment adjustment)
        {
            InitializeComponent();
            Owner = owner;
            _info = info;
            _bandwidth = bandwidth;
            _adjustment = adjustment;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void TrackVideoForm_Load(object sender, EventArgs e)
        {
            _display = new VideoDisplay2(this.Handle, videoPanel.Handle, this.BackColor);
            if (!_display.CanWork())
                _display = new VideoDisplay3(videoPanel);

            tbHeight.Text = _adjustment.Height.ToString();
            tbLatency.Text = _adjustment.Latency.ToString();
        }

        private void TrackVideoForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                stop();
                Hide();
                e.Cancel = true;
            }
            else
            {
                stop();

                if (_display != null)
                    _display.Dispose();
                _display = null;
            }
            VTSCore.Common.WindowFrontor.Instance.FrontWindow();
        }

        private ulong _videoID = 0;
        public void SetVideoID(ulong videoID, string title)
        {
            this.Text = title;
            if (videoID != _videoID)
            {
                stop();
                _videoID = videoID;
                start();
            }
            else if (_video == null)
                start();
        }

        private CCTVVideo _video;
        private IVideoDisplay _display;

        private void start()
        {
            _video = new CCTVVideo(_info, _videoID);
            _video.Bandwidth = _bandwidth;
            _video.VideoConnectedEvent += _video_VideoConnectedEvent;
            _video.VideoDisconnetedEvent += _video_VideoDisconnetedEvent;
            _video.VideoFrameEvent += _video_VideoFrameEvent;
            _video.Start();

            tsslStatus.Text = "状态：正在连接视频服务";
        }

        private void stop()
        {
            if (_video != null)
                _video.Stop();
            _video = null;
            if (_display!= null)
                _display.Clear();
        }

        void _video_VideoConnectedEvent()
        {
            tsslStatus.Text = "状态：已连接视频服务";
        }

        void _video_VideoDisconnetedEvent()
        {
            tsslStatus.Text = "状态：与视频服务的连接已断开";
        }

        void _video_VideoFrameEvent(int width, int height, byte[] data, int timeStamp)
        {
            Frame frame = new Frame();
            frame.Width = width;
            frame.Height = height;
            frame.Data = data;
            frame.TimeStamp = timeStamp;
            _display.UpdateVideoFrame(width, height, data, timeStamp);
        }

        private class Frame
        {
            public int Width;
            public int Height;
            public byte[] Data;
            public int TimeStamp;
        }

        private void panStop()
        {
            _info.CameraControl(_videoID, CCTVInfo.CameraAction.Stop, 0, 0);
        }

        private int _panSpeed = 240;
        private void btnUp_MouseDown(object sender, MouseEventArgs e)
        {
            _info.CameraControl(_videoID, CCTVInfo.CameraAction.Up, _panSpeed, 0);
        }

        private void btnUp_MouseUp(object sender, MouseEventArgs e)
        {
            panStop();
        }

        private void btnLeft_MouseDown(object sender, MouseEventArgs e)
        {
            _info.CameraControl(_videoID, CCTVInfo.CameraAction.Left, _panSpeed, 0);
        }

        private void btnLeft_MouseUp(object sender, MouseEventArgs e)
        {
            panStop();
        }

        private void btnRight_MouseDown(object sender, MouseEventArgs e)
        {
            _info.CameraControl(_videoID, CCTVInfo.CameraAction.Right, _panSpeed, 0);
        }

        private void btnRight_MouseUp(object sender, MouseEventArgs e)
        {
            panStop();
        }

        private void btnDown_MouseDown(object sender, MouseEventArgs e)
        {
            _info.CameraControl(_videoID, CCTVInfo.CameraAction.Down, _panSpeed, 0);
        }

        private void btnDown_MouseUp(object sender, MouseEventArgs e)
        {
            panStop();
        }

        private void lensStop()
        {
            _info.CameraControl(_videoID, CCTVInfo.CameraAction.LensStop, 0, 0);
        }

        private void btnZoomMinus_MouseDown(object sender, MouseEventArgs e)
        {
            _info.CameraControl(_videoID, CCTVInfo.CameraAction.ZoomTele, 0, 0);
        }

        private void btnZoomMinus_MouseUp(object sender, MouseEventArgs e)
        {
            lensStop();
        }

        private void btnZoomPlus_MouseDown(object sender, MouseEventArgs e)
        {
            _info.CameraControl(_videoID, CCTVInfo.CameraAction.ZoomWide, 0, 0);
        }

        private void btnZoomPlus_MouseUp(object sender, MouseEventArgs e)
        {
            lensStop();
        }

        private void btnFocusMinus_MouseDown(object sender, MouseEventArgs e)
        {
            _info.CameraControl(_videoID, CCTVInfo.CameraAction.FocusNear, 0, 0);
        }

        private void btnFocusMinus_MouseUp(object sender, MouseEventArgs e)
        {
            lensStop();
        }

        private void btnFocusPlus_MouseDown(object sender, MouseEventArgs e)
        {
            _info.CameraControl(_videoID, CCTVInfo.CameraAction.FocusFar, 0, 0);
        }

        private void btnFocusPlus_MouseUp(object sender, MouseEventArgs e)
        {
            lensStop();
        }

        private void tbHeight_TextChanged(object sender, EventArgs e)
        {
            adjustmentChanged();
        }

        private void tbLatency_TextChanged(object sender, EventArgs e)
        {
            adjustmentChanged();
        }

        private void adjustmentChanged()
        {
            timerUpdateAdjustment.Stop();
            timerUpdateAdjustment.Start();
        }

        private void timerUpdateAdjustment_Tick(object sender, EventArgs e)
        {
            timerUpdateAdjustment.Stop();
            int.TryParse(tbHeight.Text, out _adjustment.Height);
            int.TryParse(tbLatency.Text, out _adjustment.Latency);
        }
    }
}
