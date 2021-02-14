using System;
using System.Runtime.InteropServices;

namespace TheXamlGuy.NotificationFlyout.Shared.UI.Helpers
{
    public partial class NotificationIconHelper : IDisposable, IWndProcHandler
    {
        private const int CallbackMessage = 0x400;
        private const uint IconVersion = 0x4;

        private readonly object _lock = new();
        private bool _isDisposed;
        private NotifyIconData _notifyIconData;

        private NotificationIconHelper()
        {
            WndProcHandlerSubscriber.Current.Subscribe(this);
            CreateNotificationIcon();
        }

        ~NotificationIconHelper()
        {
            Dispose(false);
        }

        public event EventHandler<NotificationIconInvokedEventArgs> IconInvoked;

        public static NotificationIconHelper Create() => new();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Handle(uint message, IntPtr wParam, IntPtr lParam)
        {
            if (message == CallbackMessage)
            {
                switch ((uint)lParam)
                {
                    case (uint)WndProcMessages.WM_LBUTTONUP:
                        InvokeIconInvoked(PointerButton.Left);
                        break;

                    case (uint)WndProcMessages.WM_MBUTTONUP:
                        InvokeIconInvoked(PointerButton.Middle);
                        break;

                    case (uint)WndProcMessages.WM_RBUTTONUP:
                        InvokeIconInvoked(PointerButton.Right);
                        break;
                }
            }
        }

        public void SetIcon(IntPtr iconHandle)
        {
            lock (_lock)
            {
                _notifyIconData.IconHandle = iconHandle;
                WriteNotifyIconData(NotifyIconCommand.Modify, NotifyIconDataMember.Icon);
            }
        }

        private void CreateNotificationIcon()
        {
            lock (_lock)
            {
                _notifyIconData = new NotifyIconData();

                _notifyIconData.cbSize = (uint)Marshal.SizeOf(_notifyIconData);
                _notifyIconData.WindowHandle = WndProcListener.Current.Handle;
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

        private void InvokeIconInvoked(PointerButton pointerButton) => IconInvoked?.Invoke(this, new NotificationIconInvokedEventArgs(pointerButton));

        private void RemoveNotificationIcon() => WriteNotifyIconData(NotifyIconCommand.Delete, NotifyIconDataMember.Message);

        private void WriteNotifyIconData(NotifyIconCommand command, NotifyIconDataMember flags)
        {
            _notifyIconData.ValidMembers = flags;
            lock (_lock)
            {
                Shell_NotifyIcon(command, ref _notifyIconData);
            }
        }
    }
}