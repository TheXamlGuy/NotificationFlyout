using System;

namespace TheXamlGuy.NotificationFlyout.Shared.UI.Helpers
{
    public class NotificationIconInvokedEventArgs : EventArgs
    {
        internal NotificationIconInvokedEventArgs(PointerButton pointerButton) => PointerButton = pointerButton;

        public PointerButton PointerButton { get; private set; }
    }
}