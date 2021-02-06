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

        public static readonly DependencyProperty TemplateSettingsProperty =
            DependencyProperty.Register(nameof(TemplateSettings),
                typeof(NotificationFlyoutHostTemplateSettings), typeof(NotificationFlyoutHost),
                new PropertyMetadata(null));

        private readonly NotificationFlyoutHostTemplateSettings _templateSettings;
        private Grid _root;

        public NotificationFlyoutHost()
        {
            DefaultStyleKey = typeof(NotificationFlyoutHost);

            _templateSettings = new NotificationFlyoutHostTemplateSettings();
            SetValue(TemplateSettingsProperty, _templateSettings);
        }

        public NotificationFlyoutPresenter FlyoutPresenter
        {
            get => (NotificationFlyoutPresenter)GetValue(FlyoutPresenterProperty);
            set => SetValue(FlyoutPresenterProperty, value);
        }

        public NotificationFlyoutHostTemplateSettings TemplateSettings
        {
            get => (NotificationFlyoutHostTemplateSettings)GetValue(TemplateSettingsProperty);
            set => SetValue(TemplateSettingsProperty, value);
        }

        public void HideFlyout()
        {
            if (_root == null) return;
            FlyoutBase flyout = FlyoutBase.GetAttachedFlyout(_root);
            flyout.Hide();
        }

        public void SetOffset(double verticalOffset, double horizontalOffset)
        {
            if (_templateSettings == null) return;
            _templateSettings.FromVerticalOffset = verticalOffset;
            _templateSettings.FromHorizontalOffset = horizontalOffset;
        }

        public void ShowFlyout(FlyoutPlacementMode placementMode)
        {
            if (_root == null) return;
            var flyout = FlyoutBase.GetAttachedFlyout(_root);
            flyout.ShowAt(_root, new FlyoutShowOptions
            {
                Placement = placementMode,
                ShowMode = FlyoutShowMode.Standard
            });
        }

        protected override void OnApplyTemplate()
        {
            _root = GetTemplateChild("Root") as Grid;
        }
    }
}