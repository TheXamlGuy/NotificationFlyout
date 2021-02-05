using Microsoft.Win32;

namespace NotificationFlyout.Wpf.UI.Helpers
{
    internal static class RegistryHelper
    {
        public static TValue GetDwordValue<TValue>(string key, string valueName)
        {
            using var baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
            using var subKey = baseKey.OpenSubKey(key);

            return (TValue)subKey.GetValue(valueName, 0);
        }
    }
}
