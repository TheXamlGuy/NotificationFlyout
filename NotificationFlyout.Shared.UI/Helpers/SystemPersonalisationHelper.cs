using System;
using Windows.UI.ViewManagement;

namespace NotificationFlyout.Shared.UI.Helpers
{
    public class SystemPersonalisationHelper
    {
        private readonly UISettings _settings = new UISettings();

        private SystemPersonalisationHelper()
        {
            _settings.ColorValuesChanged += _settings_ColorValuesChanged;
        }

        private void _settings_ColorValuesChanged(UISettings sender, object args)
        {
            ThemeChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler ThemeChanged;

        public SystemTheme SystemTheme => GetSystemTheme();

        public static SystemPersonalisationHelper Create()
        {
            return new SystemPersonalisationHelper();
        }

        private SystemTheme GetSystemTheme()
        {
            var uiTheme = _settings.GetColorValue(UIColorType.Background).ToString();
            return uiTheme == "#FFFFFFFF" ? SystemTheme.Light : SystemTheme.Dark;
        }
    }
}
