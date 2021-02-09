using Microsoft.Windows.Sdk;
using System;

namespace NotificationFlyout.Wpf.UI.Helpers
{
    public class WindowHelper
    {
        public static IntPtr GetHandle(string windowName)
        {
            return PInvoke.FindWindow(windowName, null);
        }

        public static uint GetDpi(IntPtr handle)
        {
            return PInvoke.GetDpiForWindow((HWND)handle);
        }
    }
}
