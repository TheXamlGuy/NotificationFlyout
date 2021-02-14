using System;
using System.Runtime.InteropServices;

namespace NotificationFlyout.Shared.UI.Helpers
{
    public partial class NotificationIconHelper
    {
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

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr DefWindowProcW(IntPtr handle, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("shell32.dll", SetLastError = true)]
        private static extern int Shell_NotifyIcon(NotifyIconCommand notifyCommand, ref NotifyIconData notifyIconData);

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