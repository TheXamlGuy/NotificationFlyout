using System;

namespace TheXamlGuy.NotificationFlyout.Uwp.UI.Controls
{
    public sealed class NotificationFlyoutClosingEventArgs : EventArgs
    {
        public bool Cancel { get; set; }
    }
}