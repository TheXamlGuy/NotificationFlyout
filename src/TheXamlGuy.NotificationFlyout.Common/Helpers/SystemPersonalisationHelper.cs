using Microsoft.Win32;
using System;
using TheXamlGuy.NotificationFlyout.Common.Extensions;
using Windows.UI.ViewManagement;

namespace TheXamlGuy.NotificationFlyout.Common.Helpers
{
    public class SystemPersonalisationHelper : IWndProcHandler
    {
        private static readonly Lazy<SystemPersonalisationHelper> _current = new(() => new SystemPersonalisationHelper());

        private readonly UISettings _settings = new();
        private readonly string PersonalizeKey = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        private SystemTheme _currentTheme;
        private bool _isColorPrevalence;

        private SystemPersonalisationHelper()
        {
            WndProcHandlerSubscriber.Current.Subscribe(this);

            _currentTheme = GetTheme();
            _isColorPrevalence = GetIsColorPrevalence();
        }

        public event EventHandler<SystemPersonalisationChangedEventArgs> ThemeChanged;

        public static SystemPersonalisationHelper Current => _current.Value;
        public bool IsColorPrevalence => GetIsColorPrevalence();
        public SystemTheme Theme => GetTheme();

        public void Handle(uint message, IntPtr wParam, IntPtr lParam)
        {
            if (message == (int)WndProcMessages.WM_SETTINGCHANGE)
            {
                RaiseThemeChangedEvent();
            }
        }

        private bool GetIsColorPrevalence()
        {
            using var baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
            using var subKey = baseKey.OpenSubKey(PersonalizeKey);
            return subKey.GetValue<int>("ColorPrevalence", 0) > 0;
        }

        private SystemTheme GetTheme()
        {
            var color = _settings.GetColorValue(UIColorType.Background).ToString();
            return color == "#FFFFFFFF" ? SystemTheme.Light : SystemTheme.Dark;
        }

        private void RaiseThemeChangedEvent()
        {
            var theme = GetTheme();
            var isColorPrevalence = GetIsColorPrevalence();

            if (theme != _currentTheme || _isColorPrevalence != isColorPrevalence)
            {
                _currentTheme = theme;
                _isColorPrevalence = isColorPrevalence;

                ThemeChanged?.Invoke(this, new SystemPersonalisationChangedEventArgs(theme, isColorPrevalence));
            }
        }
    }
}