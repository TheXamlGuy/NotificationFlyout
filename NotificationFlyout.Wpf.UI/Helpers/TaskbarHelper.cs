using Microsoft.Windows.Sdk;
using NotificationFlyout.Wpf.UI.Extensions;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace NotificationFlyout.Wpf.UI.Helpers
{
    public class TaskbarHelper
    {
        private const string ShellTrayHandleName = "Shell_TrayWnd";

        private const int SPI_SETWORKAREA = 0x002F;

        private const int WSETTINGCHANGE = 0x001A;

        private static readonly uint WTASKBARCREATED = PInvoke.RegisterWindowMessage("TaskbarCreated");

        private TaskbarHelper(Window window)
        {
            var handle = window.GetHandle();

            var source = HwndSource.FromHwnd(handle);
            source.AddHook(new HwndSourceHook(WndProc));
        }

        public event EventHandler TaskbarChanged;

        private enum AppBarEdge : uint
        {
            Left = 0,
            Top = 1,
            Right = 2,
            Bottom = 3
        }

        private enum AppBarMessage : uint
        {
            New = 0x00000000,
            Remove = 0x00000001,
            QueryPos = 0x00000002,
            SetPos = 0x00000003,
            GetState = 0x00000004,
            GetTaskbarPos = 0x00000005,
            Activate = 0x00000006,
            GetAutoHideBar = 0x00000007,
            SetAutoHideBar = 0x00000008,
            WindowPosChanged = 0x00000009,
            SetState = 0x0000000A,
        }

        public static TaskbarHelper Create(Window window)
        {
            return new TaskbarHelper(window);
        }

        public TaskbarState GetCurrentState()
        {
            var handle = GetSystemTrayHandle();
            var state = new TaskbarState
            {
                Screen = Screen.FromHandle(handle)
            };

            var appBarData = GetAppBarData(handle);
            GetAppBarPosition(ref appBarData);

            state.Rect = appBarData.rect.ToRect();
            state.Position = (TaskbarPosition)appBarData.uEdge;

            return state;
        }

        private static IntPtr GetSystemTrayHandle()
        {
            return WindowHelper.GetHandle(ShellTrayHandleName);
        }

        private AppBarData GetAppBarData(IntPtr handle)
        {
            return new AppBarData
            {
                cbSize = (uint)Marshal.SizeOf(typeof(AppBarData)),
                hWnd = handle
            };
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct AppBarData
        {
            public uint cbSize;
            public IntPtr hWnd;
            public uint uCallbackMessage;
            public AppBarEdge uEdge;
            public RECT rect;
            public int lParam;
        }

        [DllImport("shell32.dll", SetLastError = true)]
        private static extern IntPtr SHAppBarMessage(AppBarMessage dwMessage, ref AppBarData pData);

        private void GetAppBarPosition(ref AppBarData appBarData)
        {
            SHAppBarMessage(AppBarMessage.GetTaskbarPos, ref appBarData);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WTASKBARCREATED || msg == WSETTINGCHANGE && (int)wParam == SPI_SETWORKAREA)
            {
                TaskbarChanged?.Invoke(this, EventArgs.Empty);
            }

            return (IntPtr)(int)PInvoke.DefWindowProc((HWND)hwnd, (uint)msg, (WPARAM)(UIntPtr)(uint)wParam, (LPARAM)lParam);
        }
    }
}
