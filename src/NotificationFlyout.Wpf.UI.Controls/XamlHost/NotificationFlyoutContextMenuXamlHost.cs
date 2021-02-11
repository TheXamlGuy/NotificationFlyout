using NotificationFlyout.Uwp.UI.Controls;
using NotificationFlyout.Wpf.UI.Extensions;
using NotificationFlyout.Wpf.UI.Helpers;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace NotificationFlyout.Wpf.UI.Controls
{
    internal class ContextMenuXamlHost : TransparentXamlHost<ContextMenuFlyoutHost>
    {
        private Uwp.UI.Controls.NotificationFlyout _flyout;

        public ContextMenuXamlHost()
        {
            Topmost = true;
        }

        public void SetFlyout(Uwp.UI.Controls.NotificationFlyout flyout)
        {
            if (_flyout != null)
            {
                _flyout.MenuItemsChanged -= OnContextMenuItemsChanged;
            }

            _flyout = flyout;
            UpdateMenuItems();
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

        protected override void OnContentLoaded()
        {
            UpdateMenuItems();
        }

        protected override void OnDeactivated(EventArgs args)
        {
            var flyoutHost = GetHostContent();
            if (flyoutHost != null)
            {
                flyoutHost.HideFlyout();
            }
        }

        private void OnContextMenuItemsChanged(object sender, NotificationFlyoutMenuItemsChangedEventArgs args)
        {
            UpdateMenuItems(args.AddedItems, args.RemovedItems);
        }

        private void UpdateMenuItems()
        {
            if (_flyout == null) return;
            UpdateMenuItems(_flyout.ContextMenuItems);
        }

        private void UpdateMenuItems(IList<MenuFlyoutItemBase> addedItems, IList<MenuFlyoutItemBase> removedItems = default)
        {
            var flyoutHost = GetHostContent();
            if (flyoutHost != null)
            {
                flyoutHost.SetMenuItems(addedItems, removedItems);
            }
        }
    }
}