using System;

namespace TheXamlGuy.NotificationFlyout.Common.Helpers
{
    internal class WndProcHelperMessageEventArgs : EventArgs
    {
        public WndProcHelperMessageEventArgs(uint message, IntPtr wParam, IntPtr lParam)
        {
            Message = message;
            WParam = wParam;
            LParam = lParam;
        }

        public IntPtr LParam { get; private set; }
        public uint Message { get; private set; }
        public IntPtr WParam { get; private set; }
    }
}