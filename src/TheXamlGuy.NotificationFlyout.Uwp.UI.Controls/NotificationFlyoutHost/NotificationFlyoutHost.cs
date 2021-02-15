﻿using System.Numerics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace TheXamlGuy.NotificationFlyout.Uwp.UI.Controls
{
    internal class NotificationFlyoutHost : Control
    {
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(nameof(Content),
                typeof(UIElement), typeof(NotificationFlyoutHost),
                new PropertyMetadata(null));

        public static readonly DependencyProperty FlyoutPresenterStyleProperty =
            DependencyProperty.Register(nameof(FlyoutPresenterStyle),
                typeof(Style), typeof(NotificationFlyoutHost),
                new PropertyMetadata(null));

        private Flyout _flyout;
        private bool _isColorPrevalence;
        private bool _isLoaded;
        private NotificationFlyout _notificationFlyout;
        private NotificationFlyoutPresenter _notificationFlyoutPresenter;
        private string _placement;
        private Grid _root;
        public NotificationFlyoutHost() => DefaultStyleKey = typeof(NotificationFlyoutHost);

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

        public void SetFlyoutPlacement(string placement)
        {
            _placement = placement;
            UpdateFlyoutVisualState();
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
            _isColorPrevalence = isColorPrevalence;
            if (_notificationFlyoutPresenter == null) return;
            _notificationFlyoutPresenter.UpdateThemeVisualState(isColorPrevalence);
        }

        protected override void OnApplyTemplate()
        {
            _notificationFlyoutPresenter = GetTemplateChild("NotificationFlyoutPresenter") as NotificationFlyoutPresenter;
            _notificationFlyoutPresenter.ApplyTemplate();

            _notificationFlyoutPresenter.UpdateThemeVisualState(_isColorPrevalence);

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
            if (GetTemplateChild("ContentRoot") is Grid contentRoot)
            {
                contentRoot.Shadow = new ThemeShadow();

                var currentTranslation = contentRoot.Translation;
                var translation = new Vector3(currentTranslation.X, currentTranslation.Y, 16.0f);
                contentRoot.Translation = translation;
            }

            _isLoaded = true;
            UpdateFlyoutVisualState();
        }

        private void OnFlyoutClosed(object sender, object args) => _notificationFlyout?.InvokeClosedEvent(args);

        private void OnFlyoutClosing(FlyoutBase sender, FlyoutBaseClosingEventArgs args) => _notificationFlyout?.InvokeClosingEvent(new NotificationFlyoutClosingEventArgs());

        private void OnFlyoutOpened(object sender, object args) => _notificationFlyout?.InvokeOpenedEvent(args);

        private void OnFlyoutOpening(object sender, object args) => _notificationFlyout?.InvokeOpeningEvent(args);

        private void UpdateFlyoutVisualState()
        {
            if (!_isLoaded) return;

            if (string.IsNullOrEmpty(_placement)) return;
            VisualStateManager.GoToState(this, _placement, true);
        }
    }
}