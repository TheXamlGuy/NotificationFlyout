using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace NotificationFlyout.Uwp.UI.Controls
{
    [ContentProperty(Name = "Content")]
    public class NotificationFlyout : DependencyObject
    {
        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.Register(nameof(IconSource),
                typeof(ImageSource), typeof(NotificationFlyout),
                new PropertyMetadata(null, OnIconPropertyChanged));

        public static readonly DependencyProperty LightIconSourceProperty =
          DependencyProperty.Register(nameof(LightIconSource),
              typeof(ImageSource), typeof(NotificationFlyout),
                new PropertyMetadata(null, OnIconPropertyChanged));

        public static readonly DependencyProperty RequestedThemeProperty =
            DependencyProperty.Register(nameof(RequestedTheme),
                typeof(ElementTheme), typeof(NotificationFlyout),
                new PropertyMetadata(ElementTheme.Default, OnRequestedThemePropertyChanged));

        public static INotificationFlyoutApplication _applicationInstance;

        public static DependencyProperty ContentProperty =
                    DependencyProperty.Register(nameof(Content),
                 typeof(UIElement), typeof(NotificationFlyout),
                 new PropertyMetadata(null, OnContentPropertyChanged));

        public static DependencyProperty ContextMenuItemsProperty =
            DependencyProperty.Register(nameof(ContextMenuItems),
                 typeof(IList<MenuFlyoutItemBase>), typeof(NotificationFlyout),
                 new PropertyMetadata(null));
        public NotificationFlyout()
        {
            ContextMenuItems = new ObservableCollection<MenuFlyoutItemBase>();
            (ContextMenuItems as INotifyCollectionChanged).CollectionChanged += OnContextMenuItemsChanged;
        }

        internal event EventHandler ContentChanged;

        internal event EventHandler IconSourceChanged;

        internal event EventHandler<NotificationFlyoutMenuItemsChangedEventArgs> MenuItemsChanged;

        internal event EventHandler RequestedThemeChanged;

        public UIElement Content
        {
            get => (UIElement)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public IList<MenuFlyoutItemBase> ContextMenuItems
        {
            get => (IList<MenuFlyoutItemBase>)GetValue(ContextMenuItemsProperty);
            set => SetValue(ContextMenuItemsProperty, value);
        }

        public ImageSource IconSource
        {
            get => (ImageSource)GetValue(IconSourceProperty);
            set => SetValue(IconSourceProperty, value);
        }

        public ImageSource LightIconSource
        {
            get => (ImageSource)GetValue(LightIconSourceProperty);
            set => SetValue(LightIconSourceProperty, value);
        }

        public ElementTheme RequestedTheme
        {
            get => (ElementTheme)GetValue(RequestedThemeProperty);
            set => SetValue(RequestedThemeProperty, value);
        }

        public static INotificationFlyoutApplication GetApplication()
        {
            return _applicationInstance;
        }

        internal static void SetApplication(INotificationFlyoutApplication application)
        {
            _applicationInstance = application;
        }

        private static void OnContentPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var sender = dependencyObject as NotificationFlyout;
            sender?.OnContentPropertyChanged();
        }

        private static void OnIconPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var sender = dependencyObject as NotificationFlyout;
            sender?.OnIconPropertyChanged();
        }

        private static void OnRequestedThemePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var sender = dependencyObject as NotificationFlyout;
            sender?.OnRequestedThemePropertyChanged();
        }

        private void OnContentPropertyChanged()
        {
            ContentChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnContextMenuItemsChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            var addedItems = args.NewItems.Cast<MenuFlyoutItemBase>().ToList();
            var removedItems = args.NewItems.Cast<MenuFlyoutItemBase>().ToList();

            MenuItemsChanged?.Invoke(this, new NotificationFlyoutMenuItemsChangedEventArgs(addedItems, removedItems));
        }

        private void OnIconPropertyChanged()
        {
            IconSourceChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnRequestedThemePropertyChanged()
        {
            RequestedThemeChanged?.Invoke(this, EventArgs.Empty);
        }
    }

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
