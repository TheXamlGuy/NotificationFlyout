using Microsoft.Windows.Sdk;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

namespace NotificationFlyout.Wpf.UI.Helpers
{
    public static class SystemInformationHelper
    {
        private const int SM_CXSCREEN = 0;

        private const int SM_CYSCREEN = 1;

        private const int SPI_GETWORKAREA = 48;

        public static Rect VirtualScreen => GetVirtualScreen();

        public static Rect WorkingArea => GetWorkingArea();

        public static int GetCurrentDpi()
        {
            return (int)typeof(SystemParameters).GetProperty("Dpi", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, null);
        }

        public static double GetCurrentDpiScaleFactor()
        {
            return (double)GetCurrentDpi() / 96;
        }

        private static Rect GetVirtualScreen()
        {
            var size = new Size(PInvoke.GetSystemMetrics(SM_CXSCREEN), PInvoke.GetSystemMetrics(SM_CYSCREEN));
            return new Rect(0, 0, size.Width, size.Height);
        }

        private static Rect GetWorkingArea()
        {
            var rect = new RECT();

            SystemParametersInfo(SPI_GETWORKAREA, 0, ref rect, 0);
            return new Rect(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool SystemParametersInfo(int nAction, int nParam, ref RECT rc, int nUpdate);
    }
}
