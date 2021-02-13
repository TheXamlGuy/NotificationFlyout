using System;

namespace NotificationFlyout.Shared.UI.Extensions
{
    public static class OperatingSystemExtensions
    {
        public static bool IsGreaterThan(this OperatingSystem operatingSystem, OperatingSystemVersion version) => operatingSystem.Version.Build > (int)version;
    }
}