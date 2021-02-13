using System.Windows;
using System.Windows.Media;

namespace NotificationFlyout.Wpf.UI.Extensions
{
    public static class VisualExtensions
    {
        private static Matrix GetDpi(this Visual visual)
        {
            var source = PresentationSource.FromVisual(visual);
            if (source?.CompositionTarget != null) return (Matrix)source?.CompositionTarget.TransformToDevice;

            return default;
        }

        public static double DpiY(this Visual visual)
        {
            return GetDpi(visual).M22;
        }

        public static double DpiX(this Visual visual)
        {
            return GetDpi(visual).M11;
        }
    }
}