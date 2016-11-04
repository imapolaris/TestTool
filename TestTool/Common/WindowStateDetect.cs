using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VTSCore.Common
{
    public static class WindowStateDetect
    {
        public static bool ShowWindow(System.Windows.Window win)
        {
            if (win != null)
            {
                foreach (System.Windows.Window openWindow in Application.Current.Windows)
                {
                    if (openWindow.ToString() == win.ToString() && openWindow.IsVisible)
                    {
                        if (win.WindowState == WindowState.Minimized)
                            win.WindowState = WindowState.Normal;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
