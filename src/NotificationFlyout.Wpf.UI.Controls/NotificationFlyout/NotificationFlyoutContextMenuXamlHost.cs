using NotificationFlyout.Uwp.UI.Controls;
using NotificationFlyout.Wpf.UI.Extensions;
using NotificationFlyout.Wpf.UI.Helpers;
using System;

namespace NotificationFlyout.Wpf.UI.Controls
{
    internal class ContextMenuXamlHost : XamlHostWindow<ContextMenuFlyoutHost>
    {
        public ContextMenuXamlHost()
        {
            Topmost = true;
        }

        protected override void OnDeactivated(EventArgs args)
        {
            var flyoutHost = GetHostContent();
            if (flyoutHost != null)
            {
                flyoutHost.HideFlyout();
            }
        }

        public void ShowContextMenuFlyout()
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
    }
}