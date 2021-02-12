using System.Numerics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace NotificationFlyout.Uwp.UI.Controls
{
    internal class NotificationFlyoutHost : Control
    {
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(nameof(Content),
                typeof(UIElement), typeof(NotificationFlyoutHost),
                new PropertyMetadata(null));

        private bool _isLoaded;
        private string _placement;
        private Grid _root;

        public NotificationFlyoutHost() => DefaultStyleKey = typeof(NotificationFlyoutHost);

        internal void SetOwningFlyout(NotificationFlyout flyout)
        {
            BindingOperations.SetBinding(this, ContentProperty,
                new Binding
                {
                    Source = flyout,
                    Path =
                    new PropertyPath(nameof(Content)),
                    Mode = BindingMode.TwoWay
                });

            BindingOperations.SetBinding(this, RequestedThemeProperty,
                new Binding
                {
                    Source = flyout,
                    Path = new PropertyPath(nameof(RequestedTheme)),
                    Mode = BindingMode.TwoWay
                });
        }

        public UIElement Content
        {
            get => (UIElement)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public void HideFlyout()
        {
            if (_root == null) return;
            FlyoutBase flyout = FlyoutBase.GetAttachedFlyout(_root);
            flyout.Hide();
        }

        public void SetFlyoutPlacement(string placement)
        {
            if (!_isLoaded)
            {
                _placement = placement;
            }

            if (string.IsNullOrEmpty(placement)) return;
            VisualStateManager.GoToState(this, placement, true);
        }

        public void ShowFlyout(FlyoutPlacementMode placementMode)
        {
            if (_root == null) return;
            var flyout = FlyoutBase.GetAttachedFlyout(_root);
            flyout.ShowAt(_root, new FlyoutShowOptions
            {
                Placement = placementMode,
                ShowMode = FlyoutShowMode.Standard,
            });
        }

        protected override void OnApplyTemplate()
        {
            _root = GetTemplateChild("Root") as Grid;
            if (GetTemplateChild("ContentRoot") is Grid contentRoot)
            {
                contentRoot.Shadow = new ThemeShadow();

                var currentTranslation = contentRoot.Translation;
                var translation = new Vector3(currentTranslation.X, currentTranslation.Y, 16.0f);
                contentRoot.Translation = translation;
            }

            _isLoaded = true;
            SetFlyoutPlacement(_placement);
        }
    }
}