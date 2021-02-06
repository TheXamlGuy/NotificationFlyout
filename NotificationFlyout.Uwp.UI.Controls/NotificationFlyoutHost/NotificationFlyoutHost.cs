using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace NotificationFlyout.Uwp.UI.Controls
{
    public class NotificationFlyoutHost : Control
    {
        public static readonly DependencyProperty FlyoutPresenterProperty =
            DependencyProperty.Register(nameof(FlyoutPresenter),
                typeof(NotificationFlyoutPresenter), typeof(NotificationFlyoutHost),
                new PropertyMetadata(null));

        private bool _isLoaded;
        private string _placement;
        private Grid _root;

        public NotificationFlyoutHost()
        {
            DefaultStyleKey = typeof(NotificationFlyoutHost);
        }

        public NotificationFlyoutPresenter FlyoutPresenter
        {
            get => (NotificationFlyoutPresenter)GetValue(FlyoutPresenterProperty);
            set => SetValue(FlyoutPresenterProperty, value);
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
            _isLoaded = true;
            SetFlyoutPlacement(_placement);
        }
    }
}