using Microsoft.Windows.Sdk;
using System;
using System.Windows;
using System.Windows.Interop;

namespace NotificationFlyout.Wpf.UI.Extensions
{
    public static class WindowExtensions
    {
        [Flags]
        private enum WindowFlag : uint
        {
            SWP_NOSIZE = 0x0001,
            SWP_NOMOVE = 0x0002,
            SWP_NOZORDER = 0x0004,
            SWP_NOACTIVATE = 0x0010,
            WS_EX_NOACTIVATE = 0x08000000,
            SWP_SHOWWINDOW = 0x0040
        }

        public static void SetWindowPosition(this Window window, double top, double left, double height, double width)
        {
            PInvoke.SetWindowPos((HWND)window.GetHandle(), (HWND)IntPtr.Zero, (int)left, (int)top, (int)width, (int)height, (uint)WindowFlag.SWP_NOSIZE | (uint)WindowFlag.SWP_NOZORDER | (uint)WindowFlag.SWP_NOACTIVATE);
        }

        public static void SetTopAll(this Window window)
        {
            PInvoke.SetWindowPos((HWND)window.GetHandle(), (HWND)new IntPtr(-1), 0, 0, 0, 0, (uint)WindowFlag.SWP_NOMOVE | (uint)WindowFlag.SWP_NOSIZE | (uint)WindowFlag.WS_EX_NOACTIVATE);
        }

        public static IntPtr GetHandle(this Window window)
        {
            var helper = new WindowInteropHelper(window);
            return helper.Handle;
        }
    }
}
