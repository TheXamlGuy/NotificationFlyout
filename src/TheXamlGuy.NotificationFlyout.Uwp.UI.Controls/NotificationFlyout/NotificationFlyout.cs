using System;
using TheXamlGuy.NotificationFlyout.Shared.UI;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace TheXamlGuy.NotificationFlyout.Uwp.UI.Controls
{
    public class NotificationFlyout : ContentControl
    {
        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.Register(nameof(IconSource),
                typeof(ImageSource), typeof(NotificationFlyout),
                new PropertyMetadata(null, OnIconPropertyChanged));

        public static readonly DependencyProperty LightIconSourceProperty =
          DependencyProperty.Register(nameof(LightIconSource),
              typeof(ImageSource), typeof(NotificationFlyout),
                new PropertyMetadata(null));

        public static DependencyProperty ContextMenuProperty =
            DependencyProperty.Register(nameof(ContextMenu),
                 typeof(NotificationFlyoutContextMenu), typeof(NotificationFlyout),
                 new PropertyMetadata(null, OnContextMenuPropertyChanged));

        public static DependencyProperty PlacementProperty =
            DependencyProperty.Register(nameof(Placement),
                typeof(NotificationFlyoutContextMenu), typeof(NotificationFlyout),
                new PropertyMetadata(NotificationFlyoutPlacement.Auto, OnPlacementPropertyChanged));

        private static INotificationFlyoutApplication _applicationInstance;

        private Popup _popup;

        public event EventHandler<object> Closed;

        public event TypedEventHandler<NotificationFlyout, NotificationFlyoutClosingEventArgs> Closing;

        public event EventHandler<object> Opened;

        public event EventHandler<object> Opening;

        internal event EventHandler ContextMenuChanged;

        internal event EventHandler IconSourceChanged;

        internal event EventHandler PlacementChanged;

        public NotificationFlyoutContextMenu ContextMenu
        {
            get => (NotificationFlyoutContextMenu)GetValue(ContextMenuProperty);
            set => SetValue(ContextMenuProperty, value);
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

        public static INotificationFlyoutApplication GetApplication() => _applicationInstance;

        internal static void SetApplication(INotificationFlyoutApplication application) => _applicationInstance = application;

        internal void InvokeClosedEvent(object obj) => Closed?.Invoke(this, obj);

        internal void InvokeClosingEvent(NotificationFlyoutClosingEventArgs eventArgs) => Closing?.Invoke(this, eventArgs);

        internal void InvokeOpenedEvent(object obj) => Opened?.Invoke(this, obj);

        internal void InvokeOpeningEvent(object obj) => Opening?.Invoke(this, obj);

        internal void SetPlacement(double horizontalOffset, double verticalOffset, NotificationFlyoutTaskbarPlacement flyoutTaskbarPlacement)
        {
            if (_popup == null)
            {
                PreparePopup();
            }

            var width = 100;
            var height = 100;

            var desiredHorizontalOffset = horizontalOffset;
            var desiredVerticalOffset = verticalOffset;

            switch (flyoutTaskbarPlacement)
            {
                case NotificationFlyoutTaskbarPlacement.Left:
                    desiredVerticalOffset = 0;
                    break;
                case NotificationFlyoutTaskbarPlacement.Top:
                    desiredHorizontalOffset -= width;
                    break;
                case NotificationFlyoutTaskbarPlacement.Right:
                    desiredHorizontalOffset -= width;
                    desiredVerticalOffset -= height;
                    break;
                case NotificationFlyoutTaskbarPlacement.Bottom:
                    desiredHorizontalOffset -= width;
                    desiredVerticalOffset -= height;
                    break;
            }

            _popup.HorizontalOffset = desiredHorizontalOffset;
            _popup.VerticalOffset = desiredVerticalOffset;
        }

        internal void Show()
        {
            if (_popup == null)
            {
                PreparePopup();
            }

            _popup.IsOpen = true;
        }

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

        private static void OnPlacementPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var sender = dependencyObject as NotificationFlyout;
            sender?.OnPlacementPropertyChanged();
        }

        private void OnContextMenuPropertyChanged() => ContextMenuChanged?.Invoke(this, EventArgs.Empty);

        private void OnIconPropertyChanged() => IconSourceChanged?.Invoke(this, EventArgs.Empty);

        private void OnPlacementPropertyChanged() => PlacementChanged?.Invoke(this, EventArgs.Empty);

        private void PreparePopup()
        {
            var f = new Grid { Background = new SolidColorBrush(Colors.Blue), Height = 100, Width = 100 };
            f.Children.Add(new Button { Content = "hefrefsef2" });

            _popup = new Popup
            {
                XamlRoot = XamlRoot,
                ShouldConstrainToRootBounds = false,
                HorizontalOffset = -1,
                VerticalOffset = -1,
                Child = f
            };
        }
    }
}