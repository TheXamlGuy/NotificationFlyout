using Microsoft.Win32;

namespace TheXamlGuy.NotificationFlyout.Common.Extensions
{
    public static class RegistryKeyExtensions
    {
        public static T GetValue<T>(this RegistryKey key, string valueName, T defaultValue = default)
        {
            return string.IsNullOrWhiteSpace(valueName) ? defaultValue : key.GetValue(valueName, defaultValue) is T value ? value : defaultValue;
        }
    }
}