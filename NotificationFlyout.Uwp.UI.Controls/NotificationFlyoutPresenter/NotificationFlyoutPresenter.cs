using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NotificationFlyout.Uwp.UI.Controls
{
    public class NotificationFlyoutPresenter : ContentControl
    {
        private NotificationFlyoutContentPresenter _contentPresenter;

        private bool _isColorPrevalence;
        private ElementTheme _systemTheme;

        public NotificationFlyoutPresenter()
        {
            DefaultStyleKey = typeof(NotificationFlyoutPresenter);       
            RegisterPropertyChangedCallback(RequestedThemeProperty, RequestedThemePropertyChanged);
        }

        public void UpdateFlyoutTheme(string theme, bool isColorPrevalence)
        {
            _isColorPrevalence = isColorPrevalence;
            switch (theme)
            {
                case "Dark":
                    _systemTheme = ElementTheme.Dark;
                    break;
                case "Light":
                    _systemTheme = ElementTheme.Light;
                    break;
            }

            UpdateThemeVisualState();
        }

        protected override void OnApplyTemplate()
        {
            var _contentPresenter = GetTemplateChild("ContentPresenter") as NotificationFlyoutContentPresenter;
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            UpdateThemeVisualState();
        }

        private void RequestedThemePropertyChanged(DependencyObject sender, DependencyProperty dependencyProperty)
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

        private void UpdateThemeVisualState()
        {
            if (_contentPresenter == null) return;
            if (RequestedTheme == ElementTheme.Default)
            {
                _contentPresenter.SetValue(RequestedThemeProperty, _systemTheme);
            }

            VisualStateManager.GoToState(_contentPresenter, _isColorPrevalence ? "ColorPrevalenceTheme" : "DefaultTheme", true);
        }
    }
}