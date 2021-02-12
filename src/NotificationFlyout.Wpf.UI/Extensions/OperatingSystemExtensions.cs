using NotificationFlyout.Wpf.UI.Helpers;
using System;

namespace NotificationFlyout.Wpf.UI.Extensions
{
    public static class OperatingSystemExtensions
    {
        public static bool IsGreaterThan(this OperatingSystem operatingSystem, OperatingSystemVersion version)
        {
            return operatingSystem.Version.Build > (int)version;
        }
    }
}