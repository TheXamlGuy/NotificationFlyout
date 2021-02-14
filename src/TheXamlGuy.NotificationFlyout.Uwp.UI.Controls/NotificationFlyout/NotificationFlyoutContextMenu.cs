using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace TheXamlGuy.NotificationFlyout.Uwp.UI.Controls
{
    [ContentProperty(Name = "MenuItems")]
    public class NotificationFlyoutContextMenu : DependencyObject
    {
        public static DependencyProperty MenuItemsProperty =
            DependencyProperty.Register(nameof(MenuItems),
                typeof(IList<MenuFlyoutItemBase>), typeof(NotificationFlyout),
                new PropertyMetadata(null));

        public NotificationFlyoutContextMenu() => MenuItems = new ObservableCollection<MenuFlyoutItemBase>();

        public IList<MenuFlyoutItemBase> MenuItems
        {
            get => (IList<MenuFlyoutItemBase>)GetValue(MenuItemsProperty);
            set => SetValue(MenuItemsProperty, value);
        }
    }
}
