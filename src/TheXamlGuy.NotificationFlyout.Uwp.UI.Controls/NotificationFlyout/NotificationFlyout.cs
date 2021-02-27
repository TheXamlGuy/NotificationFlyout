using System;
using System.Numerics;
using TheXamlGuy.NotificationFlyout.Shared.UI;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
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

        private const double OffsetValue = 1;
        private static INotificationFlyoutApplication _applicationInstance;

        private UIElement _child;
        private Border _container;
        private Popup _popup;

        public NotificationFlyout() => DefaultStyleKey = typeof(NotificationFlyout);

        public event EventHandler<object> Closed;
        public event EventHandler<object> Opened;

        internal event EventHandler IconSourceChanged;
        internal event EventHandler InteractedWith;

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

        public static INotificationFlyoutApplication GetApplication() => _applicationInstance;

        internal static void SetApplication(INotificationFlyoutApplication application) => _applicationInstance = application;

        internal void SetPlacement(double horizontalOffset, double verticalOffset, NotificationFlyoutTaskbarPlacement flyoutTaskbarPlacement)
        {
            if (_popup == null)
            {
                PreparePopup();
            }

            _child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            var width = _child.DesiredSize.Width;
            var height = _child.DesiredSize.Height;

            var desiredHorizontalOffset = horizontalOffset;
            var desiredVerticalOffset = verticalOffset;

            switch (flyoutTaskbarPlacement)
            {
                case NotificationFlyoutTaskbarPlacement.Left:
                    desiredHorizontalOffset -= OffsetValue;
                    desiredVerticalOffset -= height;
                    break;
                case NotificationFlyoutTaskbarPlacement.Top:
                    desiredHorizontalOffset -= width;
                    desiredVerticalOffset -= OffsetValue;
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

            VisualStateManager.GoToState(this, flyoutTaskbarPlacement.ToString(), true);
        }

        internal void Show()
        {
            if (_popup == null)
            {
                PreparePopup();
            }

            _popup.Child = _child;
            _popup.IsOpen = true;
        }

        internal void ShowContextMenuAt(double x, double y)
        {
            if (ContextFlyout == null) return;
            ContextFlyout.ShouldConstrainToRootBounds = false;
            ContextFlyout.XamlRoot = XamlRoot;

            ContextFlyout.ShowAt(_container);
            ContextFlyout.ShowAt(_container, new FlyoutShowOptions { Position = new Point(x, y), ShowMode = FlyoutShowMode.Standard });
        }

        internal void UpdateTheme(bool isColorPrevalence) => VisualStateManager.GoToState(this, isColorPrevalence ? "ColorPrevalenceTheme" : "DefaultTheme", true);

        protected override void OnApplyTemplate()
        {
            _container = GetTemplateChild("Container") as Border;
            if (_container != null)
            {
                if (_child != null)
                {
                    _child.PointerPressed += OnPointerPressed;
                    _child.GotFocus -= OnGotFocus;
                }

                _child = _container.Child;
                _child.PointerPressed += OnPointerPressed;
                _child.GotFocus += OnGotFocus;

                _container.Child = null;
            }

            if (GetTemplateChild("BackgroundElement") is Border backgroundElement)
            {
                backgroundElement.Shadow = new ThemeShadow();

                var currentTranslation = backgroundElement.Translation;
                var translation = new Vector3(currentTranslation.X, currentTranslation.Y, 16.0f);
                backgroundElement.Translation = translation;
            }
        }

        private static void OnIconPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var sender = dependencyObject as NotificationFlyout;
            sender?.OnIconPropertyChanged();
        }

        private void OnGotFocus(object sender, RoutedEventArgs args) => InteractedWith?.Invoke(this, EventArgs.Empty);

        private void OnIconPropertyChanged() => IconSourceChanged?.Invoke(this, EventArgs.Empty);

        private void OnPointerPressed(object sender, PointerRoutedEventArgs args) => InteractedWith?.Invoke(this, EventArgs.Empty);
        private void OnPopupClosed(object sender, object args) => Opened?.Invoke(this, args);

        private void OnPopupOpened(object sender, object args) => Closed?.Invoke(this, args);

        private void PreparePopup()
        {
            _popup = new Popup
            {
                XamlRoot = XamlRoot,
                ShouldConstrainToRootBounds = false,
                HorizontalOffset = -OffsetValue,
                VerticalOffset = -OffsetValue
            };

            _popup.Opened += OnPopupOpened;
            _popup.Closed += OnPopupClosed;
        }
    }
}