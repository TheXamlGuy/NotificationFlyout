using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace NotificationFlyout.Uwp.UI.Controls
{
    public class NotificationFlyoutPresenter : ContentControl
    {
        public static readonly DependencyProperty TemplateSettingsProperty =
            DependencyProperty.Register(nameof(TemplateSettings),
                typeof(NotificationFlyoutPresenterTemplateSettings), typeof(NotificationFlyoutPresenter),
                new PropertyMetadata(0d));

        private Grid _root;
        private NotificationFlyoutPresenterTemplateSettings _templateSettings;

        public NotificationFlyoutPresenter()
        {
            DefaultStyleKey = typeof(NotificationFlyoutPresenter);

            _templateSettings = new NotificationFlyoutPresenterTemplateSettings();
            SetValue(TemplateSettingsProperty, _templateSettings);
        }

        public NotificationFlyoutPresenterTemplateSettings TemplateSettings
        {
            get => (NotificationFlyoutPresenterTemplateSettings)GetValue(TemplateSettingsProperty);
            set => SetValue(TemplateSettingsProperty, value);
        }

        public void SetOffset(double verticalOffset, double horizontalOffset)
        {
            if (_templateSettings == null) return;
            _templateSettings.FromVerticalOffset = verticalOffset;
            _templateSettings.FromHorizontalOffset = horizontalOffset;
        }

        public void HideFlyout()
        {
            if (_root == null) return;
            FlyoutBase flyout = FlyoutBase.GetAttachedFlyout(_root);
            flyout.Hide();
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