using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VTSCore.Layers.Radar
{
    /// <summary>
    /// RadarColorSchemeEditorClient.xaml 的交互逻辑
    /// </summary>
    public partial class RadarColorSchemeEditorClient : Window
    {
        RadarColorSchemeEvent _scheme;

        public ColorTableDataConfig Config { get { return _scheme.Config(); } }
        public RadarColorSchemeEditorClient(ColorTableDataConfig config)
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            _scheme = new RadarColorSchemeEvent(config);
            this.DataContext = _scheme;
        }


        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btQuit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btFrontStart_Click(object sender, RoutedEventArgs e)
        {
            var color = _scheme.FrontStart;
            if (updateColor(ref color))
                _scheme.FrontStart = color;
        }

        private void tbFrontEnd_Click(object sender, RoutedEventArgs e)
        {
            var color = _scheme.FrontEnd;
            if (updateColor(ref color))
                _scheme.FrontEnd = color;
        }

        private void tbTrailStart_Click(object sender, RoutedEventArgs e)
        {
            var color = _scheme.TrailStart;
            if (updateColor(ref color))
                _scheme.TrailStart = color;
        }

        private void tbTrailEnd_Click(object sender, RoutedEventArgs e)
        {
            var color = _scheme.TrailEnd;
            if (updateColor(ref color))
                _scheme.TrailEnd = color;
        }

        bool updateColor(ref Color color)
        {
            Microsoft.Samples.CustomControls.ColorPickerDialog cPicker
               = new Microsoft.Samples.CustomControls.ColorPickerDialog();
            cPicker.StartingColor = color;
            cPicker.Owner = this;

            bool? dialogResult = cPicker.ShowDialog();
            if (dialogResult != null && (bool)dialogResult == true)
            {
                color = cPicker.SelectedColor;
                return true;
            }
            return false;
        }
    }

    public class RadarColorSchemeEvent : INotifyPropertyChanged
    {

        public RadarColorSchemeEvent(ColorTableDataConfig config)
        {
            if (config == null)
                config = new ColorTableDataConfig();
            Heading = config.Heading;
            TrailState = config.TrailState;
            var color = config.ColorTableData();
            TrailStart = color.TrailStart;
            TrailEnd = color.TrailEnd;
            FrontStart = color.FrontStart;
            FrontEnd = color.FrontEnd;
        }

        public ColorTableDataConfig Config()
        {
            ColorTableDataConfig config = new ColorTableDataConfig()
            {
                Heading = this.Heading,
                TrailState = this.TrailState,
                TrailStart = this.TrailStart.ToString(),
                TrailEnd = this.TrailEnd.ToString(),
                FrontStart = this.FrontStart.ToString(),
                FrontEnd = this.FrontEnd.ToString()
            };
            return config;
        }

        string heading;
        public string Heading
        {
            get { return heading; }
            set
            {
                if (Heading != value)
                {
                    heading = value;
                    FirePropertyChanged("Heading");
                }
            }
        }
        int trailState = 10;
        public int TrailState
        {
            get { return trailState; }
            set
            {
                if (TrailState != value)
                {
                    trailState = value;
                    FirePropertyChanged("TrailState");
                }
            }
        }

        Color frontStart;
        public Color FrontStart
        {
            get { return frontStart; }
            set
            {
                if (FrontStart != value)
                {
                    frontStart = value;
                    FirePropertyChanged("FrontStart");
                }
            }
        }

        Color frontEnd;
        public Color FrontEnd
        {
            get { return frontEnd; }
            set
            {
                if (FrontEnd != value)
                {
                    frontEnd = value;
                    FirePropertyChanged("FrontEnd");
                }
            }
        }
        Color trailStart;
        public Color TrailStart
        {
            get { return trailStart; }
            set
            {
                if (TrailStart != value)
                {
                    trailStart = value;
                    FirePropertyChanged("TrailStart");
                }
            }
        }
        Color trailEnd;
        public Color TrailEnd
        {
            get { return trailEnd; }
            set
            {
                if (TrailEnd != value)
                {
                    trailEnd = value;
                    FirePropertyChanged("TrailEnd");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void FirePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
