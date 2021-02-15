using Microsoft.Windows.Sdk;
using System;

namespace TheXamlGuy.NotificationFlyout.Common.Helpers
{
    public class WindowHelper
    {
        public static IntPtr GetHandle(string windowName) => PInvoke.FindWindow(windowName, null);

        public static uint GetDpi(IntPtr handle) => PInvoke.GetDpiForWindow((HWND)handle);
    }
}