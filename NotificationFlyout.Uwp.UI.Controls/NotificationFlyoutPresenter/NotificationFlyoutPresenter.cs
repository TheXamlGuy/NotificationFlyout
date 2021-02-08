using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NotificationFlyout.Uwp.UI.Controls
{
    public class NotificationFlyoutPresenter : ContentControl
    {
        private NotificationFlyoutContentPresenter _contentPresenter;

        private ElementTheme _systemTheme;

        public NotificationFlyoutPresenter()
        {
            DefaultStyleKey = typeof(NotificationFlyoutPresenter);       
            RegisterPropertyChangedCallback(RequestedThemeProperty, RequestedThemePropertyChanged);
        }

        public void SetBackground(string theme)
        {
            if (_contentPresenter == null) return;

            switch (theme)
            {
                case "Dark":
                    _systemTheme = ElementTheme.Dark;
                    break;
                case "Light":
                    _systemTheme = ElementTheme.Light;
                    break;
            }

            if (RequestedTheme == ElementTheme.Default)
            {
                _contentPresenter.SetValue(RequestedThemeProperty, _systemTheme);
            }
        }

        protected override void OnApplyTemplate()
        {
            _contentPresenter = GetTemplateChild("ContentPresenter") as NotificationFlyoutContentPresenter;
        }

        private void RequestedThemePropertyChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (RequestedTheme == ElementTheme.Default)
            {
                _contentPresenter.SetValue(RequestedThemeProperty, _systemTheme);
            }
            else
            {
                _contentPresenter.SetValue(RequestedThemeProperty, RequestedTheme);
            }
        }
    }
}