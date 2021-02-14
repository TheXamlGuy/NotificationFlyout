using TheXamlGuy.NotificationFlyout.Shared.UI.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TheXamlGuy.NotificationFlyout.Uwp.UI.Controls
{
    public class NotificationFlyoutPresenter : ContentControl
    {
        private SystemPersonalisationHelper _systemPersonalisationHelper;

        public NotificationFlyoutPresenter()
        {
            DefaultStyleKey = typeof(NotificationFlyoutPresenter);

            _systemPersonalisationHelper = SystemPersonalisationHelper.Current;
            _systemPersonalisationHelper.ThemeChanged += OnThemeChanged;
        }

        protected override void OnApplyTemplate() => UpdateThemeVisualStates(false);

        private void OnThemeChanged(object sender, SystemPersonalisationChangedEventArgs args) => UpdateThemeVisualStates(true);

        private void UpdateThemeVisualStates(bool useTransition) => VisualStateManager.GoToState(this, _systemPersonalisationHelper.IsColorPrevalence ? "ColorPrevalenceTheme" : "DefaultTheme", useTransition);
    }
}