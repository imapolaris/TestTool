using CCTVClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace VTSCore.Layers.Tracks.CCTV
{
    class CCTVTarget: Canvas
    {
        VideoParser.Camera _camera;
        int _length = 0;
        float _zoomFactor = 1;
        private VideoParser.Video Video;
        Polygon polygon;
        Path pathLimit;
        Path pathView;
        Line lineMid;
        Line lineLeft;
        Line lineRight;
        Ellipse ellipse;
        bool selectedStatus;
        public CCTVTarget(VideoParser.Video video, VideoParser.Camera camera)
        {
            Video = video;
            pathLimit = newPath(System.Windows.Media.Brushes.LightYellow);
            pathView = newPath(System.Windows.Media.Brushes.LightGreen);
            this.Children.Add(pathLimit);
            this.Children.Add(pathView);
            newPolygon();
            fillColorByOnline();
            AddLine(Video.PanTiltUnit.WideView);
            Camera = camera;
        }

        public void Update(CCTVClient.VideoParser.Video video)
        {
            //Console.WriteLine(string.Format("Video: Id: {0} {1} -> {2}", Video.Id, Video.PanTiltUnit.WideView, video.PanTiltUnit.WideView));
            if (Video.Online != video.Online)
            {
                Video.Online = video.Online;
                fillColorByOnline();
            }
            if(Video.PanTiltUnit.WideView != video.PanTiltUnit.WideView)
            {
                Video.PanTiltUnit.WideView = video.PanTiltUnit.WideView;
                updateWideView();
                updateViewport();
            }
            if(Video.PanTiltUnit.LeftLimit != video.PanTiltUnit.LeftLimit || Video.PanTiltUnit.RightLimit != video.PanTiltUnit.RightLimit)
            {
                Video.PanTiltUnit.LeftLimit = video.PanTiltUnit.LeftLimit;
                Video.PanTiltUnit.RightLimit = video.PanTiltUnit.RightLimit;
                updateViewArea(pathLimit, _length, Video.PanTiltUnit.LeftLimit, Video.PanTiltUnit.RightLimit);
            }
        }

        private void updateViewport()
        {
            double viewport = WideView();
            updateViewArea(pathView, _length * 4 / 5, -viewport / 2, viewport / 2);
        }

        public void Update(int length, float zoomFactor)
        {
            if (_length != length)
                updateArc(length);
            if (_zoomFactor != zoomFactor)
                updateCameraPolygon(zoomFactor);
        }

        public VideoParser.Camera Camera
        {
            get { return _camera; }
            set
            {
                if (_camera != value)
                {
                    _camera = value;
                    renderTransform();
                }
            }
        }

        public bool Selected
        {
            get { return selectedStatus; }
            set
            {
                if (selectedStatus != value)
                {
                    selectedStatus = value;
                    if (Selected)
                    {
                        ellipse = NewEllipse(20);
                        this.Children.Add(ellipse);
                    }
                    else
                    {
                        this.Children.Remove(ellipse);
                        ellipse = null;
                    }
                }
            }
        }

        private void updateCameraPolygon(float zoomFactor)
        {
            _zoomFactor = zoomFactor;
            int l1 = (int)(10 * _zoomFactor);
            int l2 = (int)(15 * _zoomFactor);
            int l3 = (int)(6 * _zoomFactor);
            int l4 = (int)(2 * _zoomFactor);
            int l5 = (int)(5 * _zoomFactor);
            polygon.Points[0] = new System.Windows.Point(-l3, l1);
            polygon.Points[1] = new System.Windows.Point(-l3, -l1);
            polygon.Points[2] = new System.Windows.Point(-l4, -l1);
            polygon.Points[3] = new System.Windows.Point(-l5, -l2);
            polygon.Points[4] = new System.Windows.Point(l5, -l2);
            polygon.Points[5] = new System.Windows.Point(l4, -l1);
            polygon.Points[6] = new System.Windows.Point(l3, -l1);
            polygon.Points[7] = new System.Windows.Point(l3, l1);
        }

        private void AddLine(double angle)
        {
            lineLeft = NewLine();
            this.Children.Add(lineLeft);
            lineMid = NewLine();
            this.Children.Add(lineMid);
            lineRight = NewLine();
            this.Children.Add(lineRight);
        }

        private void updateArc(int length)
        {
            _length = length;
            lineMid.Y2 = -length;
            lineLeft.Y2 = -length;
            lineRight.Y2 = -length;
            updateViewArea(pathLimit, _length, Video.PanTiltUnit.LeftLimit, Video.PanTiltUnit.RightLimit);
            updateViewport();
        }

        void updateViewArea(Path path, int length, double left, double right)
        {
            var valueView = GetArcData(length, left, right);
            path.SetBinding(Path.DataProperty, new System.Windows.Data.Binding() { Source = valueView });
        }

        void renderTransform()
        {
            var transform = new System.Windows.Media.RotateTransform(Angle());
            polygon.RenderTransform = transform;
            pathView.RenderTransform = transform;
            lineMid.RenderTransform = transform;
            updateWideView();
        }

        private double Angle()
        {
            double angle = 0;
            if (_camera != null)
                angle = _camera.Pointing.Pan;
            return angle;
        }

        private double WideView()
        {
            double camZoom = 1;
            if (Camera != null)
                camZoom = Camera.Zoom;
            double viewport = (1 - camZoom) * (Video.PanTiltUnit.WideView - Video.PanTiltUnit.TeleView) + Video.PanTiltUnit.TeleView;
            return viewport;
        }

        private void updateWideView()
        {
            double angle = Angle();
            double viewport = WideView();
            var transformLeft = new System.Windows.Media.RotateTransform(angle - viewport / 2);
            var transformRight = new System.Windows.Media.RotateTransform(angle + viewport / 2);
            lineLeft.RenderTransform = transformLeft;
            lineRight.RenderTransform = transformRight;
            updateViewport();
        }

        private string GetArcData(int length, double left, double right)
        {
            double angle = getRotationAngle(left, right);
            int isLargeArcFlag = LargeArcFlag(angle);
            double angleLeft = left * Math.PI / 180;
            double angleRight = right * Math.PI / 180;
            System.Windows.Point pointLeft = new System.Windows.Point(length * Math.Sin(angleLeft), -length * Math.Cos(angleLeft));
            System.Windows.Point pointRight = new System.Windows.Point(length * Math.Sin(angleRight), -length * Math.Cos(angleRight));
            string value = string.Format("M 0,0 L {0},{1} A {2},{2} {3} {4} 1 {5},{6} M 0,0", pointLeft.X, pointLeft.Y, length, angle, isLargeArcFlag, pointRight.X, pointRight.Y);
            return value;
        }

        private double getRotationAngle(double left, double right)
        {
            double angle = right - left;
            if (angle < 0)
                angle += 360;
            return angle;
        }

        int LargeArcFlag(double angle)
        {
            int largeArcFlag = 0;
            if (angle <= 180)
                largeArcFlag = 0;
            else
                largeArcFlag = 1;
            return largeArcFlag;
        }

        private void newPolygon()
        {
            polygon = new Polygon();
            for (int i = 0; i < 8; i++)
                polygon.Points.Add(new System.Windows.Point());
            polygon.Stroke = System.Windows.Media.Brushes.Black;
            this.Children.Add(polygon);
        }

        private void fillColorByOnline()
        {
            polygon.Fill = Video.Online ? System.Windows.Media.Brushes.Blue : System.Windows.Media.Brushes.Gray;
        }

        private Path newPath(System.Windows.Media.SolidColorBrush brush)
        {
            Path path = new Path();
            path.Fill = brush;
            path.Opacity = 0.25;
            return path;
        }

        private Line NewLine()
        {
            Line line = new Line();
            line.Stroke = System.Windows.Media.Brushes.Black;
            return line;
        }

        Ellipse NewEllipse(double Radius)
        {
            Ellipse e = new Ellipse();
            e.Stroke = System.Windows.Media.Brushes.DarkGreen;
            e.Height = Radius * 2 + 1;
            e.Width = Radius * 2 + 1;
            Canvas.SetLeft(e, -Radius);
            Canvas.SetTop(e, -Radius);
            return e;
        }
    }
}