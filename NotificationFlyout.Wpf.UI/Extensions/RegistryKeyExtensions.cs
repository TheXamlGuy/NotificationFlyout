using Microsoft.Win32;

namespace NotificationFlyout.Wpf.UI.Extensions
{
    public static class RegistryKeyExtensions
    {
        public static T GetValue<T>(this RegistryKey self, string valueName, T defaultValue)
        {
            return self.GetValue(valueName, defaultValue) is T t ? t : defaultValue;
        }
    }
}
