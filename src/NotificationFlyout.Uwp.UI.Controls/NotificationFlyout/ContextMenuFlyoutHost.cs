using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace NotificationFlyout.Uwp.UI.Controls
{
    internal class ContextMenuFlyoutHost : Control
    {
        private MenuFlyout _flyout;
        private Grid _root;

        public ContextMenuFlyoutHost()
        {
            DefaultStyleKey = typeof(ContextMenuFlyoutHost);
        }

        public void HideFlyout()
        {
            if (_flyout == null) return;
            _flyout.Hide();
        }

        public void ShowFlyout()
        {
            if (_root == null) return;
            if (_flyout == null) return;

            _flyout.ShowAt(_root, new FlyoutShowOptions { Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft, ShowMode = FlyoutShowMode.TransientWithDismissOnPointerMoveAway });
        }

        protected override void OnApplyTemplate()
        {
            _root = GetTemplateChild("Root") as Grid;
            _flyout = GetTemplateChild("Flyout") as MenuFlyout;
        }

        internal void SetMenuItems(IList<MenuFlyoutItemBase> addedItems, IList<MenuFlyoutItemBase> removedItems = null)
        {
            if (_flyout == null) return;

            if (removedItems != null)
            {
                foreach (var item in removedItems)
                {
                    _flyout.Items.Remove(item);
                }
            }

            foreach (var item in addedItems)
            {
                _flyout.Items.Add(item);
            }
        }
    }
}