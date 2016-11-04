using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;


namespace VTSCore.Layers.Plotting
{
    public class ConfigRadarAreaColor
    {
        public string Heading;
        public double Opacity;
        public string FillColor;
        public string StrokeColor;
        public bool IsVisible;
        public ConfigRadarAreaColor()
        {
            Opacity = 0.1;
            FillColor = "#FFFF0000"; //(Color)ColorConverter.ConvertFromString("#FFFF0000");
            StrokeColor = "#FF00FF00";// (Color)ColorConverter.ConvertFromString("#FF00FF00");
            IsVisible = true;
        }
    }

    public class RadarAreaMaskColor
    {
        public string Heading;
        public double Opacity;
        public Color FillColor;
        public Color StrokeColor;
        public bool IsVisible;
        ILog LogService { get { return LogManager.GetLogger(GetType()); } }
        public RadarAreaMaskColor()
        {
            Opacity = 0.1;
            FillColor = (Color)ColorConverter.ConvertFromString("#FFFF0000");
            StrokeColor = (Color)ColorConverter.ConvertFromString("#FF00FF00");
            IsVisible = true;
        }

        public RadarAreaMaskColor (ConfigRadarAreaColor config)
        {
            try
            {
                Heading = config.Heading;
                Opacity = config.Opacity;
                FillColor = (Color)ColorConverter.ConvertFromString(config.FillColor);
                StrokeColor = (Color)ColorConverter.ConvertFromString(config.StrokeColor);
                IsVisible = config.IsVisible;
            }
            catch(Exception ex)
            {
                LogService.Error(ex.ToString());
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
    }
}
