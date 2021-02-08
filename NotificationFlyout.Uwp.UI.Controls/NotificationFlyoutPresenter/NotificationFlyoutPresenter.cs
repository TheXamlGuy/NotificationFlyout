using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NotificationFlyout.Uwp.UI.Controls
{
    public class NotificationFlyoutPresenter : ContentControl
    {
        private NotificationFlyoutContentPresenter _contentPresenter;

        public NotificationFlyoutPresenter()
        {
            DefaultStyleKey = typeof(NotificationFlyoutPresenter);
            ActualThemeChanged += OnActualThemeChanged;
        }

        protected override void OnApplyTemplate()
        {
            _contentPresenter = GetTemplateChild("ContentPresenter") as NotificationFlyoutContentPresenter;
        }

        public void SetBackground(string theme)
        {
            if (_contentPresenter == null) return;
            if (RequestedTheme == ElementTheme.Default)
            {
                ActualThemeChanged -= OnActualThemeChanged;

                switch (theme)
                {
                    case "Dark":
                        _contentPresenter.SetValue(RequestedThemeProperty, ElementTheme.Dark);
                        break;
                    case "Light":
                        _contentPresenter.SetValue(RequestedThemeProperty, ElementTheme.Light);
                        break;
                }

                ActualThemeChanged += OnActualThemeChanged;
            }
        }

        private void OnActualThemeChanged(FrameworkElement sender, object args)
        {
            _contentPresenter.SetValue(RequestedThemeProperty, RequestedTheme);
        }
    }
}