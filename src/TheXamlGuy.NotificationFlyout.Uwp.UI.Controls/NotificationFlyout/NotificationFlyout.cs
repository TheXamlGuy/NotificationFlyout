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

        public static readonly DependencyProperty IsLightDismissEnabledProperty =
            DependencyProperty.Register(nameof(IsLightDismissEnabled),
                typeof(bool), typeof(NotificationFlyout),
                new PropertyMetadata(true));

        public static readonly DependencyProperty LightIconSourceProperty =
            DependencyProperty.Register(nameof(LightIconSource),
              typeof(ImageSource), typeof(NotificationFlyout),
                new PropertyMetadata(null));

        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.Register(nameof(Placement),
                typeof(NotificationFlyoutPlacement), typeof(NotificationFlyout),
                new PropertyMetadata(NotificationFlyoutPlacement.Auto, OnPlacementPropertyChanged));

        public static readonly DependencyProperty TemplateSettingsProperty =
            DependencyProperty.Register(nameof(TemplateSettings),
                typeof(NotificationFlyoutTemplateSettings), typeof(NotificationFlyout),
                new PropertyMetadata(null));

        private const double OffsetValue = 1;

        private static INotificationFlyoutApplication _applicationInstance;

        private Border _backgroundElement;

        private UIElement _child;

        private Border _container;

        private Popup _popup;

        public NotificationFlyout()
        {
            DefaultStyleKey = typeof(NotificationFlyout);
            TemplateSettings = new NotificationFlyoutTemplateSettings();
        }

        public event EventHandler<object> Closed;

        public event EventHandler<object> Opened;

        internal event EventHandler IconSourcePropertyChanged;

        internal event EventHandler InteractedWith;

        internal event EventHandler PlacementPropertyChanged;


        public ImageSource IconSource
        {
            get => (ImageSource)GetValue(IconSourceProperty);
            set => SetValue(IconSourceProperty, value);
        }

        public bool IsLightDismissEnabled
        {
            get => (bool)GetValue(IsLightDismissEnabledProperty);
            set => SetValue(IsLightDismissEnabledProperty, value);
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

        public NotificationFlyoutTemplateSettings TemplateSettings
        {
            get => (NotificationFlyoutTemplateSettings)GetValue(TemplateSettingsProperty);
            set => SetValue(TemplateSettingsProperty, value);
        }

        public static INotificationFlyoutApplication GetApplication() => _applicationInstance;

        public void Close()
        {
            if (_popup == null)
            {
                PreparePopup();
            }

            _popup.IsOpen = false;
        }

        public void Open()
        {
            if (_popup == null)
            {
                PreparePopup();
            }

            if (!_popup.IsOpen)
            {
                _popup.Child = _child;
                _popup.IsOpen = true;
            }
        }

        internal static void SetApplication(INotificationFlyoutApplication application) => _applicationInstance = application;

        internal void Close(bool shouldRespectIsLightDismissEnbabled)
        {
            if (!shouldRespectIsLightDismissEnbabled || IsLightDismissEnabled)
            {
                Close();
            }
        }

        internal void SetPlacement(double horizontalOffset, double verticalOffset, double workingAreaHeight, double workingAreaWidth, NotificationFlyoutTaskbarPlacement flyoutTaskbarPlacement)
        {
            if (_popup == null)
            {
                PreparePopup();
            }

            VisualStateManager.GoToState(this, "DefaultPlacement", true);

            _child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            var width = _child.DesiredSize.Width;
            var height = Placement == NotificationFlyoutPlacement.Auto ? _child.DesiredSize.Height : workingAreaHeight;

            var desiredHorizontalOffset = horizontalOffset;
            var desiredVerticalOffset = verticalOffset;

            var visualState = "";
            switch (Placement)
            {
                case NotificationFlyoutPlacement.Auto:
                    visualState = $"{flyoutTaskbarPlacement}";
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

                    break;
                case NotificationFlyoutPlacement.FullRight:
                    visualState = $"{Placement}";
                    switch (flyoutTaskbarPlacement)
                    {
                        case NotificationFlyoutTaskbarPlacement.Left:
                            desiredHorizontalOffset += workingAreaWidth - width;
                            desiredVerticalOffset = 0;
                            break;
                        case NotificationFlyoutTaskbarPlacement.Top:
                            desiredHorizontalOffset = workingAreaWidth - width;
                            break;
                        case NotificationFlyoutTaskbarPlacement.Right:
                            desiredHorizontalOffset -= width;
                            desiredVerticalOffset = 0;
                            break;
                        case NotificationFlyoutTaskbarPlacement.Bottom:
                            desiredHorizontalOffset = workingAreaWidth - width;
                            desiredVerticalOffset = 0;
                            break;
                    }
                    break;
            }


            TemplateSettings.SetValue(NotificationFlyoutTemplateSettings.HeightProperty, height);
            TemplateSettings.SetValue(NotificationFlyoutTemplateSettings.WidthProperty, width);

            TemplateSettings.SetValue(NotificationFlyoutTemplateSettings.NegativeHeightProperty, -height);
            TemplateSettings.SetValue(NotificationFlyoutTemplateSettings.NegativeWidthProperty, -width);

            _popup.SetValue(Popup.HorizontalOffsetProperty, desiredHorizontalOffset);
            _popup.SetValue(Popup.VerticalOffsetProperty, desiredVerticalOffset);

            VisualStateManager.GoToState(this, $"{visualState}Placement", true);
        }

        internal void ShowContextMenuAt(double x, double y)
        {
            if (ContextFlyout == null) return;
            ContextFlyout.ShouldConstrainToRootBounds = false;
            ContextFlyout.XamlRoot = XamlRoot;

            ContextFlyout.ShowAt(_container);
            ContextFlyout.ShowAt(_container, new FlyoutShowOptions
            {
                Position = new Point(x, y),
                ShowMode = FlyoutShowMode.Standard 
            });
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

            _backgroundElement = GetTemplateChild("BackgroundElement") as Border;
            if (_backgroundElement != null)
            {
                _backgroundElement.Shadow = new ThemeShadow();

                var currentTranslation = _backgroundElement.Translation;
                var translation = new Vector3(currentTranslation.X, currentTranslation.Y, 16.0f);
                _backgroundElement.Translation = translation;
            }
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

        private void OnGotFocus(object sender, RoutedEventArgs args) => InteractedWith?.Invoke(this, EventArgs.Empty);

        private void OnIconPropertyChanged() => IconSourcePropertyChanged?.Invoke(this, EventArgs.Empty);

        private void OnPlacementPropertyChanged() => PlacementPropertyChanged?.Invoke(this, EventArgs.Empty);

        private void OnPointerPressed(object sender, PointerRoutedEventArgs args) => InteractedWith?.Invoke(this, EventArgs.Empty);

        private void OnPopupClosed(object sender, object args) => Closed?.Invoke(this, args);

        private void OnPopupOpened(object sender, object args) => Opened?.Invoke(this, args);

        private void PreparePopup()
        {
            if (_popup != null)
            {
                _popup.Opened -= OnPopupOpened;
                _popup.Closed -= OnPopupClosed;
            }

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