using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TheXamlGuy.NotificationFlyout.Uwp.UI.Controls
{
    public class NotificationFlyoutPresenter : ContentControl
    {
        public NotificationFlyoutPresenter()
        {
            DefaultStyleKey = typeof(NotificationFlyoutPresenter);
        }

      //  protected override void OnApplyTemplate() => UpdateThemeVisualStates(false);

      //  private void UpdateThemeVisualStates(bool useTransition) => VisualStateManager.GoToState(this, _systemPersonalisationHelper.IsColorPrevalence ? "ColorPrevalenceTheme" : "DefaultTheme", useTransition);
    }
}