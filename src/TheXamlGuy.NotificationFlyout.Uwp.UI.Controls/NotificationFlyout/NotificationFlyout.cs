using System;
using System.Numerics;
using TheXamlGuy.NotificationFlyout.Shared.UI;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
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

        private static INotificationFlyoutApplication _applicationInstance;

        private UIElement _child;
        private Border _container;
        private Popup _popup;

        public NotificationFlyout() => DefaultStyleKey = typeof(NotificationFlyout);

        public event EventHandler<object> Closed;
        public event TypedEventHandler<NotificationFlyout, NotificationFlyoutClosingEventArgs> Closing;
        public event EventHandler<object> Opened;
        public event EventHandler<object> Opening;

        internal event EventHandler IconSourceChanged;

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
                    desiredVerticalOffset -= height;
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
                _child = _container.Child;
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

        private void InvokeClosedEvent(object obj) => Closed?.Invoke(this, obj);

        private void InvokeClosingEvent(NotificationFlyoutClosingEventArgs eventArgs) => Closing?.Invoke(this, eventArgs);

        private void InvokeOpenedEvent(object obj) => Opened?.Invoke(this, obj);

        private void InvokeOpeningEvent(object obj) => Opening?.Invoke(this, obj);

        private void OnIconPropertyChanged() => IconSourceChanged?.Invoke(this, EventArgs.Empty);

        private void PreparePopup()
        {
            _popup = new Popup
            {
                XamlRoot = XamlRoot,
                ShouldConstrainToRootBounds = false,
                IsLightDismissEnabled = true,
                HorizontalOffset = -1,
                VerticalOffset = -1
            };
        }
    }
}