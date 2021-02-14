using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace NotificationFlyout.Uwp.UI.Controls
{
    internal class NotificationFlyoutMenuItemsChangedEventArgs : EventArgs
    {
        public NotificationFlyoutMenuItemsChangedEventArgs(IList<MenuFlyoutItemBase> addedItems, IList<MenuFlyoutItemBase> removedItems)
        {
            AddedItems = addedItems;
            RemovedItems = removedItems;
        }

        public IList<MenuFlyoutItemBase> AddedItems { get; private set; }

        public IList<MenuFlyoutItemBase> RemovedItems { get; private set; }
    }
}