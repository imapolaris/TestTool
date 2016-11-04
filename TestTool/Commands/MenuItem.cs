using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VTSCore.Commands
{
    public static class MenuItem
    {
        public static RoutedUICommand AddSeaChart { get; private set; }
        public static RoutedUICommand ExitClick { get; private set; }
        public static RoutedUICommand SetupPlottingArea { get; private set; }
        public static RoutedUICommand AddAISInfo { get; private set; }
        /// <summary>
        /// 雷达参数修改窗口
        /// </summary>
        public static RoutedUICommand RadarMenuInfo { get; private set; }
        public static RoutedUICommand AddRadarInfo { get; private set; }


        static MenuItem()
        {
            Util.CreateAllCommands(typeof(MenuItem));
        }
    }
}
