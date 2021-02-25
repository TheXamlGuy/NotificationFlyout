using System;
using TheXamlGuy.NotificationFlyout.Shared.UI;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace TheXamlGuy.NotificationFlyout.Uwp.UI.Controls
{
    [ContentProperty(Name = "Content")]
    public class NotificationFlyout : DependencyObject
    {
        public static readonly DependencyProperty FlyoutPresenterStyleProperty =
            DependencyProperty.Register(nameof(FlyoutPresenterStyle),
                typeof(Style), typeof(NotificationFlyout),
                new PropertyMetadata(null));

        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.Register(nameof(IconSource),
                typeof(ImageSource), typeof(NotificationFlyout),
                new PropertyMetadata(null, OnIconPropertyChanged));

        public static readonly DependencyProperty LightIconSourceProperty =
          DependencyProperty.Register(nameof(LightIconSource),
              typeof(ImageSource), typeof(NotificationFlyout),
                new PropertyMetadata(null));

        public static readonly DependencyProperty RequestedThemeProperty =
            DependencyProperty.Register(nameof(RequestedTheme),
                typeof(ElementTheme), typeof(NotificationFlyout),
                new PropertyMetadata(ElementTheme.Default));

        public static DependencyProperty ContentProperty =
            DependencyProperty.Register(nameof(Content),
                 typeof(UIElement), typeof(NotificationFlyout),
                 new PropertyMetadata(null));

        public static DependencyProperty ContextMenuProperty =
            DependencyProperty.Register(nameof(ContextMenu),
                 typeof(NotificationFlyoutContextMenu), typeof(NotificationFlyout),
                 new PropertyMetadata(null, OnContextMenuPropertyChanged));

        public static DependencyProperty PlacementProperty =
            DependencyProperty.Register(nameof(Placement),
                typeof(NotificationFlyoutContextMenu), typeof(NotificationFlyout),
                new PropertyMetadata(NotificationFlyoutPlacement.Auto));

        private static INotificationFlyoutApplication _applicationInstance;

        public event EventHandler<object> Closed;
        public event TypedEventHandler<NotificationFlyout, NotificationFlyoutClosingEventArgs> Closing;
        public event EventHandler<object> Opened;
        public event EventHandler<object> Opening;

        internal event EventHandler ContextMenuChanged;
        internal event EventHandler IconSourceChanged;

        public UIElement Content
        {
            get => (UIElement)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public NotificationFlyoutContextMenu ContextMenu
        {
            get => (NotificationFlyoutContextMenu)GetValue(ContextMenuProperty);
            set => SetValue(ContextMenuProperty, value);
        }

        public Style FlyoutPresenterStyle
        {
            get => (Style)GetValue(FlyoutPresenterStyleProperty);
            set => SetValue(FlyoutPresenterStyleProperty, value);
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

        public NotificationFlyoutPlacement Placement
        {
            get => (NotificationFlyoutPlacement)GetValue(PlacementProperty);
            set => SetValue(PlacementProperty, value);
        }
        public ElementTheme RequestedTheme
        {
            get => (ElementTheme)GetValue(RequestedThemeProperty);
            set => SetValue(RequestedThemeProperty, value);
        }

        public static INotificationFlyoutApplication GetApplication() => _applicationInstance;

        internal static void SetApplication(INotificationFlyoutApplication application) => _applicationInstance = application;

        internal void InvokeClosedEvent(object obj) => Closed?.Invoke(this, obj);

        internal void InvokeClosingEvent(NotificationFlyoutClosingEventArgs eventArgs) => Closing?.Invoke(this, eventArgs);

        internal void InvokeOpenedEvent(object obj) => Opened?.Invoke(this, obj);

        internal void InvokeOpeningEvent(object obj) => Opening?.Invoke(this, obj);
       
        private static void OnContextMenuPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var sender = dependencyObject as NotificationFlyout;
            sender?.OnContextMenuPropertyChanged();
        }

        private static void OnIconPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var sender = dependencyObject as NotificationFlyout;
            sender?.OnIconPropertyChanged();
        }

        private void OnContextMenuPropertyChanged() => ContextMenuChanged?.Invoke(this, EventArgs.Empty);

        private void OnIconPropertyChanged() => IconSourceChanged?.Invoke(this, EventArgs.Empty);
    }
}