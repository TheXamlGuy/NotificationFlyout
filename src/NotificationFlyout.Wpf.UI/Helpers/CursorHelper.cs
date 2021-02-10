using Microsoft.Windows.Sdk;

namespace NotificationFlyout.Wpf.UI.Helpers
{
    internal class CursorHelper
    {
        public static POINT GetPhysicalCursorPos()
        {
            PInvoke.GetPhysicalCursorPos(out POINT point);
            return point;
        }
    }
}
