﻿using Microsoft.Windows.Sdk;

namespace NotificationFlyout.Uwp.UI.Extensions
{
    internal class ExecutionMode
    {
        internal static bool IsRunningWithIdentity()
        {
            uint packageNameLength = 0;
            int result = PInvoke.GetCurrentPackageFullName(ref packageNameLength, "1024");

            return result != 15700;
        }
    }
}