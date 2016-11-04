using CCTVClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using VTSCore.Layers.Maps;

namespace VTSCore.Layers.Tracks.CCTV
{
    class CCTVCanvas:Canvas
    {
        public const double maxrange = 0.016666;
        public VideoParser.Video Video { get; private set; }
        LocatorAndBorder _locator;
        CCTVTarget target;

        public VideoParser.Camera Camera { set { target.Camera = value; } }
       
        public CCTVCanvas(VideoParser.Video video, VideoParser.Camera camera, LocatorAndBorder locator)
        {
            Video = video;
            _locator = locator;
            target = new CCTVTarget(Video, camera);
            target.Update(Length(), ZoomFactor());
            this.Children.Add(target);
        }

        public void UpdateVideo(VideoParser.Video video)
        {
            target.Update(video);
        }

        public void OnMapRefreshed()
        {
            target.Update(Length(), ZoomFactor());
            System.Windows.Point point = _locator.Locator.MapToScreen(Video.PanTiltUnit.Longitude, Video.PanTiltUnit.Latitude);
            Canvas.SetLeft(target, point.X);
            Canvas.SetTop(target, point.Y);
        }

        public bool NeerPoint(System.Windows.Point point, ref double length)
        {
            if(target.Visibility == System.Windows.Visibility.Visible)
            {
                double x = Canvas.GetLeft(target) - point.X;
                double y = Canvas.GetTop(target) - point.Y;
                length = Math.Sqrt(x * x + y * y);
                if (length < 20)
                    return true;
            }
            return false;
        }

        public bool Selected
        {
            get { return target.Selected; }
            set { target.Selected = value; }
        }

        public string String()
        {
            return string.Format(" Id: {0} Name: {1}, OnLine: {2} Lon: {3} Lat: {4}", Video.Id, Video.Name, Video.Online, Video.PanTiltUnit.Longitude, Video.PanTiltUnit.Latitude);
        }
        
        private int Length()
        {
            System.Windows.Point point = _locator.Locator.MapToScreen(Video.PanTiltUnit.Longitude, Video.PanTiltUnit.Latitude);
            var temp = _locator.Locator.MapToScreen(Video.PanTiltUnit.Longitude, Video.PanTiltUnit.Latitude - maxrange);
            int ll = (int)Math.Sqrt((temp.X - point.X) * (temp.X - point.X) + (temp.Y - point.Y) * (temp.Y - point.Y));
            return ll;
        }

        private float ZoomFactor()
        {
            double scale = _locator.Locator.Scale;
            if (scale > 900000)
                return 0.5f;
            else if (scale <= 20000)
                return 1f;
            return (float)(Math.Sin(Math.PI * (40000 / scale + 1) / 6));
        }
    }
}
