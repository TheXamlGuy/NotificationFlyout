using System;
using System.Windows.Input;

namespace NotificationFlyout.Wpf.UI.Helpers
{
    public class NotificationIconInvokedEventArgs : EventArgs
    {
        public MouseButton MouseButton { get; internal set; }
    }
}