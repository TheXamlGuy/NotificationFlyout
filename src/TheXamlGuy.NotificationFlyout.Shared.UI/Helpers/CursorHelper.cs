using Microsoft.Windows.Sdk;

namespace TheXamlGuy.NotificationFlyout.Shared.UI.Helpers
{
    internal class CursorHelper
    {
        public static POINT GetPhysicalCursorPos()
        {
            PInvoke.GetPhysicalCursorPos(out var point);
            return point;
        }
    }
}