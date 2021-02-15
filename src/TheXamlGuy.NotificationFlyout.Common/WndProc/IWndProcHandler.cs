using System;

namespace TheXamlGuy.NotificationFlyout.Common.Helpers
{
    public interface IWndProcHandler
    {
        void Handle(uint message, IntPtr wParam, IntPtr lParam);
    }
}