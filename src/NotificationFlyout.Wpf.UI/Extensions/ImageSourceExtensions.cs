using Microsoft.Windows.Sdk;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media;

namespace NotificationFlyout.Wpf.UI.Extensions
{
    public static class ImageSourceExtensions
    {
        public static Icon ConvertToIcon(this ImageSource imageSource, uint dpi)
        {
            if (imageSource == null) return null;

            var uri = new Uri(imageSource.ToString(), UriKind.RelativeOrAbsolute);

            var streamResource = Application.GetResourceStream(uri);
            if (streamResource == null) throw new ArgumentException(nameof(streamResource));

            return new Icon(streamResource.Stream, new System.Drawing.Size(PInvoke.GetSystemMetricsForDpi((int)SystemMetricFlag.SM_CXICON, dpi), PInvoke.GetSystemMetricsForDpi((int)SystemMetricFlag.SM_CYICON, dpi)));
        }

        private enum SystemMetricFlag : int
        {
            SM_CXICON = 11,
            SM_CYICON = 12,
            SM_CXSMICON = 49,
            SM_CYSMICON = 50
        }
    }
}
