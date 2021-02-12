using System.Collections.Specialized;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace NotificationFlyout.Uwp.UI.Controls
{
    internal class NotificationFlyoutContextMenuFlyoutHost : Control
    {
        private NotificationFlyout _flyout;
        private MenuFlyout _menuFlyout;
        private Grid _root;

        public NotificationFlyoutContextMenuFlyoutHost() => DefaultStyleKey = typeof(NotificationFlyoutContextMenuFlyoutHost);

        public void HideFlyout()
        {
            if (_menuFlyout == null) return;
            _menuFlyout.Hide();
        }

        public void ShowFlyout()
        {
            if (_root == null) return;
            if (_menuFlyout == null) return;

            _menuFlyout.ShowAt(_root, new FlyoutShowOptions
            { 
                Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft,
                ShowMode = FlyoutShowMode.TransientWithDismissOnPointerMoveAway
            });
        }

        internal void SetOwningFlyout(NotificationFlyout flyout)
        {
            if (_flyout != null)
            {
                (_flyout.ContextMenu.MenuItems as INotifyCollectionChanged).CollectionChanged -= OnContextMenuItemsChanged;
            }

            _flyout = flyout;
            
            var contextMenu = _flyout.ContextMenu;
            if (contextMenu == null) return;

            (contextMenu.MenuItems as INotifyCollectionChanged).CollectionChanged += OnContextMenuItemsChanged;

            PrepareMenuItems();
        }

        protected override void OnApplyTemplate()
        {
            _root = GetTemplateChild("Root") as Grid;
            _menuFlyout = GetTemplateChild("Flyout") as MenuFlyout;

            PrepareMenuItems();
        }

        private void OnContextMenuItemsChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (_flyout == null) return;

            var contextMenu = _flyout.ContextMenu;
            if (contextMenu == null) return;

            var addedItems = args.NewItems.Cast<MenuFlyoutItemBase>().ToList();
            var removedItems = args.NewItems.Cast<MenuFlyoutItemBase>().ToList();

            if (removedItems != null)
            {
                foreach (var item in removedItems)
                {
                    _menuFlyout.Items.Remove(item);
                }
            }

            foreach (var item in addedItems)
            {
                _menuFlyout.Items.Add(item);
            }
        }

        private void PrepareMenuItems()
        {
            if (_menuFlyout == null) return;
            if (_flyout == null) return;

            var contextMenu = _flyout.ContextMenu;
            if (contextMenu == null) return;

            _menuFlyout.Items.Clear();

            var items = contextMenu.MenuItems;
            foreach (var item in items)
            {
                _menuFlyout.Items.Add(item);
            }
        }
    }
}