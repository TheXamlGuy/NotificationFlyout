using NotificationFlyout.Wpf.UI.Extensions;
using System;

namespace NotificationFlyout.Wpf.UI.Helpers
{
    public static class SystemSettingsHelper
    {
       public static SystemTheme DefaultSystemTheme => GetDefaultSystemTheme();

        private static SystemTheme GetDefaultSystemTheme()
        {
            return Environment.OSVersion.IsGreaterThan(OperatingSystemVersion.Windows10_1809) && DoesSystemUsesLightTheme() ? SystemTheme.Light : SystemTheme.Dark;
        }

        private static bool DoesSystemUsesLightTheme()
        {
            var personalizeKey = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
            return RegistryHelper.GetDwordValue<int>(personalizeKey, "SystemUsesLightTheme") > 0;
        }
    }
}
