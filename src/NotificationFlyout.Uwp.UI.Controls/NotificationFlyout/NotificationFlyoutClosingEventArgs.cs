using System;

namespace NotificationFlyout.Uwp.UI.Controls
{
    public sealed class NotificationFlyoutClosingEventArgs : EventArgs
    {
        public bool Cancel { get; set; }
    }
}