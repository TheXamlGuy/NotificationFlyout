using System.Numerics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace TheXamlGuy.NotificationFlyout.Uwp.UI.Controls
{
    public class NotificationFlyoutPresenter : ContentControl
    {
        public NotificationFlyoutPresenter() => DefaultStyleKey = typeof(NotificationFlyoutPresenter);

        protected override void OnApplyTemplate()
        {
            if (GetTemplateChild("Root") is Grid contentRoot)
            {
                contentRoot.Shadow = new ThemeShadow();

                var currentTranslation = contentRoot.Translation;
                var translation = new Vector3(currentTranslation.X, currentTranslation.Y, 16.0f);
                contentRoot.Translation = translation;
            }
        }

        internal void UpdateThemeVisualState(bool isColorPrevalence) => VisualStateManager.GoToState(this, isColorPrevalence ? "ColorPrevalenceTheme" : "DefaultTheme", true);
    }
}