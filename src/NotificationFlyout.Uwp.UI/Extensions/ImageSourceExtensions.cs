using Microsoft.Windows.Sdk;
using System;
using System.Drawing;
using System.IO;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace NotificationFlyout.Uwp.UI.Extensions
{
    public static class ImageSourceExtensions
    {
        public static Icon ConvertToIcon(this ImageSource imageSource, uint dpi)
        {
            var bitmapImage = (BitmapImage)imageSource;
            var uri = $"{AppDomain.CurrentDomain.BaseDirectory}{bitmapImage.UriSour‌​ce}".Replace("ms-appx:///", "").Replace("/", "\\");

            using var stream = File.OpenRead(uri);
            return new Icon(stream, new Size(PInvoke.GetSystemMetricsForDpi((int)SystemMetricFlag.SM_CXICON, dpi), PInvoke.GetSystemMetricsForDpi((int)SystemMetricFlag.SM_CYICON, dpi)));
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
