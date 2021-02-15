using System;

namespace TheXamlGuy.NotificationFlyout.Common.Helpers
{
    public class NotificationIconInvokedEventArgs : EventArgs
    {
        internal NotificationIconInvokedEventArgs(PointerButton pointerButton) => PointerButton = pointerButton;

        public PointerButton PointerButton { get; private set; }
    }
}