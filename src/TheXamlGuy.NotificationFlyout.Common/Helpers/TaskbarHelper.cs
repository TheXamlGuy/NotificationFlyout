using System;
using System.Runtime.InteropServices;
using TheXamlGuy.NotificationFlyout.Common.Extensions;

namespace TheXamlGuy.NotificationFlyout.Common.Helpers
{
    public partial class TaskbarHelper : IWndProcHandler
    {
        private TaskbarHelper() => WndProcHandlerSubscriber.Current.Subscribe(this);

        public event EventHandler TaskbarChanged;

        public static TaskbarHelper Create() => new();

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

        public void Handle(uint message, IntPtr wParam, IntPtr lParam)
        {
            if (message == WM_TASKBARCREATED || message == (int)WndProcMessages.WM_SETTINGCHANGE && (int)wParam == SPI_SETWORKAREA)
            {
                TaskbarChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private AppBarData GetAppBarData(IntPtr handle)
        {
            return new AppBarData
            {
                cbSize = (uint)Marshal.SizeOf(typeof(AppBarData)),
                hWnd = handle
            };
        }

        private void GetAppBarPosition(ref AppBarData appBarData) => SHAppBarMessage(AppBarMessage.GetTaskbarPos, ref appBarData);
    }
}