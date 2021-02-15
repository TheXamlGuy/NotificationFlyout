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

        internal void UpdateThemeVisualState(bool isColorPrevalence) => VisualStateManager.GoToState(this, isColorPrevalence ? "ColorPrevalenceTheme" : "DefaultTheme", true);
    }
}