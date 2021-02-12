using Microsoft.Windows.Sdk;
using System.Windows;

namespace NotificationFlyout.Wpf.UI.Extensions
{
    internal static class RECTExtensions
    {
        internal static Rect ToRect(this RECT rect)
        {
            if (rect.right - rect.left < 0 || rect.bottom - rect.top < 0) return new Rect(rect.left, rect.top, 0, 0);

            return new Rect(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
        }
    }
}