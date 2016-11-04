using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace VTSCore.Layers.Radar
{
    public class ColorTableDataConfig
    {
        public string Heading { get; set; }      //名称
        public int TrailState { get; set; } //余晖级数
        public string FrontStart { get; set; }   //回波
        public string FrontEnd { get; set; }

        public string TrailStart { get; set; }   //余晖
        public string TrailEnd { get; set; }

        public ColorTableDataConfig()
        {
            this.TrailState = 10;
            this.FrontStart = "#FFF7F662";
            this.FrontEnd = "#FFFF0000";
            this.TrailStart = "#FF025E02";
            this.TrailEnd = "#FFB4F6B3";
            Heading = "方案 1";
        }

        public ColorTableData ColorTableData()
        {
            ColorTableData data = new ColorTableData()
            {
                TrailState = this.TrailState,
                FrontStart = (Color)ColorConverter.ConvertFromString(this.FrontStart),
                FrontEnd = (Color)ColorConverter.ConvertFromString(this.FrontEnd),
                TrailStart = (Color)ColorConverter.ConvertFromString(this.TrailStart),
                TrailEnd = (Color)ColorConverter.ConvertFromString(this.TrailEnd)
            };
            return data;
        }

        public static ColorTableDataConfig[] InitColorTableDatas()
        {
            ColorTableDataConfig[] config = new ColorTableDataConfig[1];
            config[0] = new ColorTableDataConfig();
            return config;
        }
    }
}
