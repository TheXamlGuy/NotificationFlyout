using NotificationFlyout.Uwp.UI.Controls;
using NotificationFlyout.Wpf.UI.Extensions;
using NotificationFlyout.Wpf.UI.Helpers;
using System;

namespace NotificationFlyout.Wpf.UI.Controls
{
    internal class NotificationFlyoutContextMenuXamlHost : TransparentXamlHost<NotificationFlyoutContextMenuFlyoutHost>
    {
        public NotificationFlyoutContextMenuXamlHost() => Topmost = true;

        internal void SetOwningFlyout(Uwp.UI.Controls.NotificationFlyout flyout)
        {
            var content = GetHostContent();
            if (content != null)
            {
                content.SetOwningFlyout(flyout);
            }
        }

        internal void ShowContextMenuFlyout()
        {
            var position = CursorHelper.GetPhysicalCursorPos();
            this.SetWindowPosition(position.y, position.x, WindowSize, WindowSize);

            var flyoutHost = GetHostContent();
            if (flyoutHost != null)
            {
                flyoutHost.ShowFlyout();
            }

            Activate();
        }

        protected override void OnDeactivated(EventArgs args)
        {
            var flyoutHost = GetHostContent();
            if (flyoutHost != null)
            {
                flyoutHost.HideFlyout();
            }
        }
    }
}