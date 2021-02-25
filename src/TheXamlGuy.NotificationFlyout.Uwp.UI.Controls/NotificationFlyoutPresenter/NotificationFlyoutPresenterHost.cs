using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;

namespace TheXamlGuy.NotificationFlyout.Uwp.UI.Controls
{
    internal class NotificationFlyoutPresenterHost : Control
    {
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(nameof(Content),
                typeof(UIElement), typeof(NotificationFlyoutPresenterHost),
                new PropertyMetadata(null));

        public static readonly DependencyProperty FlyoutPresenterStyleProperty =
            DependencyProperty.Register(nameof(FlyoutPresenterStyle),
                typeof(Style), typeof(NotificationFlyoutPresenterHost),
                new PropertyMetadata(null));

        private Flyout _flyout;
        private NotificationFlyout _notificationFlyout;
        private NotificationFlyoutPresenter _notificationFlyoutPresenter;
        private Grid _root;

        public NotificationFlyoutPresenterHost() => DefaultStyleKey = typeof(NotificationFlyoutPresenterHost);

        public UIElement Content
        {
            get => (UIElement)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public Style FlyoutPresenterStyle
        {
            get => (Style)GetValue(FlyoutPresenterStyleProperty);
            set => SetValue(FlyoutPresenterStyleProperty, value);
        }

        public void HideFlyout()
        {
            if (_root == null) return;
            FlyoutBase flyout = FlyoutBase.GetAttachedFlyout(_root);
            flyout.Hide();
        }

        public void UpdatePlacement(NotificationFlyoutPresenterPlacement placement)
        {
            var state = placement.ToString();

            VisualStateManager.GoToState(this, state, true);
            VisualStateManager.GoToState(_notificationFlyoutPresenter, state, true);
        }

        public void SetOwningFlyout(NotificationFlyout flyout)
        {
            _notificationFlyout = flyout;

            BindingOperations.SetBinding(this, ContentProperty,
                new Binding
                {
                    Source = _notificationFlyout,
                    Path =
                    new PropertyPath(nameof(Content)),
                    Mode = BindingMode.TwoWay
                });

            BindingOperations.SetBinding(this, RequestedThemeProperty,
                new Binding
                {
                    Source = _notificationFlyout,
                    Path = new PropertyPath(nameof(RequestedTheme)),
                    Mode = BindingMode.TwoWay
                });

            BindingOperations.SetBinding(this, FlyoutPresenterStyleProperty,
                new Binding
                {
                    Source = _notificationFlyout,
                    Path = new PropertyPath(nameof(FlyoutPresenterStyle)),
                    Mode = BindingMode.TwoWay
                });
        }

        public void ShowFlyout(FlyoutPlacementMode placementMode)
        {
            if (_root == null) return;
            var flyout = FlyoutBase.GetAttachedFlyout(_root);
            flyout.ShowAt(_root, new FlyoutShowOptions
            {
                Placement = placementMode,
                ShowMode = FlyoutShowMode.Transient,
            });
        }

        public void UpdateTheme(bool isColorPrevalence)
        {
            if (_notificationFlyoutPresenter == null) return;
            _notificationFlyoutPresenter.UpdateThemeVisualState(isColorPrevalence);
        }

        protected override void OnApplyTemplate()
        {
            _notificationFlyoutPresenter = GetTemplateChild("NotificationFlyoutPresenter") as NotificationFlyoutPresenter;

            if (_notificationFlyoutPresenter != null)
            {
                _notificationFlyoutPresenter.ApplyTemplate();
            }

            _flyout = GetTemplateChild("Flyout") as Flyout;
            if (_flyout != null)
            {
                _flyout.Closing -= OnFlyoutClosing;
                _flyout.Closed -= OnFlyoutClosed;
                _flyout.Opening -= OnFlyoutOpening;
                _flyout.Opened -= OnFlyoutOpened;

                _flyout.Closing += OnFlyoutClosing;
                _flyout.Closed += OnFlyoutClosed;
                _flyout.Opening += OnFlyoutOpening;
                _flyout.Opened += OnFlyoutOpened;
            }

            _root = GetTemplateChild("Root") as Grid;
        }

        private void OnFlyoutClosed(object sender, object args) => _notificationFlyout?.InvokeClosedEvent(args);

        private void OnFlyoutClosing(FlyoutBase sender, FlyoutBaseClosingEventArgs args) => _notificationFlyout?.InvokeClosingEvent(new NotificationFlyoutClosingEventArgs());

        private void OnFlyoutOpened(object sender, object args) => _notificationFlyout?.InvokeOpenedEvent(args);

        private void OnFlyoutOpening(object sender, object args) => _notificationFlyout?.InvokeOpeningEvent(args);
    }
}