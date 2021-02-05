using Microsoft.Win32;
using NotificationFlyout.Wpf.UI.Extensions;
using System;

namespace NotificationFlyout.Wpf.UI.Helpers
{
    public static class SystemSettingsHelper
    {
        private static readonly string PersonalizeKey = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";

        public static SystemTheme DefaultSystemTheme => GetDefaultSystemTheme();

        private static SystemTheme GetDefaultSystemTheme()
        {
            return Environment.OSVersion.IsGreaterThan(OperatingSystemVersion.Windows10_1809) &&
                   ReadDword(PersonalizeKey, "SystemUsesLightTheme")
                ? SystemTheme.Light
                : SystemTheme.Dark;
        }

        private static bool ReadDword(string key, string valueName, int defaultValue = 0)
        {
            using var baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
            using var subKey = baseKey.OpenSubKey(key);
            return subKey.GetValue<int>(valueName, defaultValue) > 0;
        }

        public static bool IsTransparencyEnabled => ReadDword(PersonalizeKey, "EnableTransparency");
    }
}
