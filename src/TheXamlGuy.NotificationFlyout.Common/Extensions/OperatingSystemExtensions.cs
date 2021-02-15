using System;

namespace TheXamlGuy.NotificationFlyout.Common.Extensions
{
    public static class OperatingSystemExtensions
    {
        public static bool IsGreaterThan(this OperatingSystem operatingSystem, OperatingSystemVersion version) => operatingSystem.Version.Build > (int)version;
    }
}