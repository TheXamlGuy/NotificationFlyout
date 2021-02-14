using System;

namespace TheXamlGuy.NotificationFlyout.Shared.UI.Helpers
{
    internal partial class WndProcHelper : IDisposable
    {
        private WndProcHandler _handler;

        private WndProcHelper() => CreateWndProcWindow();

        private delegate IntPtr WndProcHandler(IntPtr hwnd, uint uMsg, IntPtr wparam, IntPtr lparam);

        public event EventHandler<WndProcHelperMessageEventArgs> WndProcMessage;

        public IntPtr Handle { get; private set; }

        public static WndProcHelper Create() => new();

        public void Dispose() => DestroyWindow(Handle);

        private void CreateWndProcWindow()
        {
            var windowName = Guid.NewGuid().ToString();
            _handler = WndProc;

            WNDCLASSW wndProcWindow;

            wndProcWindow.style = 0;
            wndProcWindow.lpfnWndProc = _handler;
            wndProcWindow.cbClsExtra = 0;
            wndProcWindow.cbWndExtra = 0;
            wndProcWindow.hInstance = IntPtr.Zero;
            wndProcWindow.hIcon = IntPtr.Zero;
            wndProcWindow.hCursor = IntPtr.Zero;
            wndProcWindow.hbrBackground = IntPtr.Zero;
            wndProcWindow.lpszMenuName = "";
            wndProcWindow.lpszClassName = windowName;

            RegisterClassW(ref wndProcWindow);

            Handle = CreateWindowExW(0, windowName, "", 0, 0, 0, 1, 1, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        }

        private IntPtr WndProc(IntPtr handle, uint message, IntPtr wParam, IntPtr lParam)
        {
            WndProcMessage?.Invoke(this, new WndProcHelperMessageEventArgs(message, wParam, lParam));
            return DefWindowProcW(handle, message, wParam, lParam);
        }
    }
}