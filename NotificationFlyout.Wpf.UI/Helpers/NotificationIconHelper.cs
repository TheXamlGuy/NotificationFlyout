using NotificationFlyout.Wpf.UI.Extensions;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace NotificationFlyout.Wpf.UI.Helpers
{
    public class NotificationIconHelper : IDisposable
    {
        private const int CallbackMessage = 0x400;
        private const uint IconVersion = 0x4;

        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_MBUTTONUP = 0x0208;
        private const int WM_RBUTTONUP = 0x0205;
        private readonly object _lock = new();
        private readonly IntPtr _windowHandle;
        private bool _isDisposed;
        private NotifyIconData _notifyIconData;

        private NotificationIconHelper(Window window)
        {
            _windowHandle = window.GetHandle();

            var source = HwndSource.FromHwnd(_windowHandle);
            source.AddHook(new HwndSourceHook(WndProc));

            CreateNotificationIcon();
        }


        ~NotificationIconHelper()
        {
            Dispose(false);
        }

        public event EventHandler<NotificationIconInvokedEventArgs> IconInvoked;

        private enum NotifyIconBalloonType
        {
            None = 0x00,
            Info = 0x01,
            Warning = 0x02,
            Error = 0x03,
            User = 0x04,
            NoSound = 0x10,
            LargeIcon = 0x20,
            RespectQuietTime = 0x80
        }

        private enum NotifyIconCommand : uint
        {
            Add = 0x0,
            Delete = 0x2,
            Modify = 0x1,
            SetVersion = 0x4
        }

        [Flags]
        private enum NotifyIconDataMember : uint
        {
            Message = 0x01,
            Icon = 0x02,
            Tip = 0x04,
            State = 0x08,
            Info = 0x10,
            Realtime = 0x40,
            UseLegacyToolTips = 0x80
        }

        private enum NotifyIconState : uint
        {
            Visible = 0x00,
            Hidden = 0x01
        }

        public static NotificationIconHelper Create(Window window)
        {
            return new NotificationIconHelper(window);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void SetIcon(IntPtr iconHandle)
        {
            lock (_lock)
            {
                _notifyIconData.IconHandle = iconHandle;
                WriteNotifyIconData(NotifyIconCommand.Modify, NotifyIconDataMember.Icon);
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr DefWindowProcW(IntPtr handle, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("shell32.dll", SetLastError = true)]
        private static extern int Shell_NotifyIcon(NotifyIconCommand notifyCommand, ref NotifyIconData notifyIconData);

        private void CreateNotificationIcon()
        {
            lock (_lock)
            {
                _notifyIconData = new NotifyIconData();

                _notifyIconData.cbSize = (uint)Marshal.SizeOf(_notifyIconData);
                _notifyIconData.WindowHandle = _windowHandle;
                _notifyIconData.TaskbarIconId = 0x0;
                _notifyIconData.CallbackMessageId = CallbackMessage;
                _notifyIconData.VersionOrTimeout = IconVersion;

                _notifyIconData.IconHandle = IntPtr.Zero;

                _notifyIconData.IconState = NotifyIconState.Hidden;
                _notifyIconData.StateMask = NotifyIconState.Hidden;

                WriteNotifyIconData(NotifyIconCommand.Add, NotifyIconDataMember.Message | NotifyIconDataMember.Icon | NotifyIconDataMember.Tip);
            }
        }

        private void Dispose(bool disposing)
        {
            if (_isDisposed || !disposing) return;

            lock (_lock)
            {
                _isDisposed = true;
                RemoveNotificationIcon();
            }
        }

        private void RemoveNotificationIcon() => WriteNotifyIconData(NotifyIconCommand.Delete, NotifyIconDataMember.Message);
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == CallbackMessage)
            {
                var mouseButton = MouseButton.Left;
                var isInvoked = false;

                switch ((uint)lParam)
                {
                    case WM_LBUTTONUP:
                        isInvoked = true;
                        mouseButton = MouseButton.Left;
                        break;

                    case WM_MBUTTONUP:
                        isInvoked = true;
                        mouseButton = MouseButton.Middle;
                        break;

                    case WM_RBUTTONUP:
                        isInvoked = true;
                        mouseButton = MouseButton.Right;
                        break;
                }

                if (isInvoked)
                {
                    IconInvoked?.Invoke(this, new NotificationIconInvokedEventArgs { MouseButton = mouseButton });
                }
            }

            return DefWindowProcW(hwnd, (uint)msg, wParam, (lParam));
        }

        private void WriteNotifyIconData(NotifyIconCommand command, NotifyIconDataMember flags)
        {
            _notifyIconData.ValidMembers = flags;
            lock (_lock)
            {
                Shell_NotifyIcon(command, ref _notifyIconData);
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct NotifyIconData
        {
            public uint cbSize;
            public IntPtr WindowHandle;
            public uint TaskbarIconId;
            public NotifyIconDataMember ValidMembers;
            public uint CallbackMessageId;
            public IntPtr IconHandle;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string ToolTipText;

            public NotifyIconState IconState;
            public NotifyIconState StateMask;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string BalloonText;

            public uint VersionOrTimeout;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string BalloonTitle;

            public NotifyIconBalloonType BalloonFlags;
            public Guid TaskbarIconGuid;
            public IntPtr CustomBalloonIconHandle;
        }
    }
}