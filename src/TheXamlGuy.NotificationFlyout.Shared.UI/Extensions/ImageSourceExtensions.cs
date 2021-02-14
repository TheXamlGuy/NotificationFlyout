using Microsoft.Windows.Sdk;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace TheXamlGuy.NotificationFlyout.Shared.UI.Extensions
{
    public static class ImageSourceExtensions
    {
        public static async Task<Icon> ConvertToIconAsync(this ImageSource imageSource, uint dpi)
        {
            var bitmapImage = (BitmapImage)imageSource;
            if (!ExecutionMode.IsRunningWithIdentity())
            {
                var uri = $"{AppDomain.CurrentDomain.BaseDirectory}{bitmapImage.UriSour‌​ce}".Replace("ms-appx:///", "").Replace("/", "\\");

                using var stream = File.OpenRead(uri);
                return ExtractIcon(dpi, stream);
            }
            else
            {
                var storageFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(bitmapImage.UriSource);

                using var stream = await storageFile.OpenStreamForReadAsync();
                return ExtractIcon(dpi, stream);
            }
        }

        private static Icon ExtractIcon(uint dpi, Stream stream)
        {
            var bitmap = (Bitmap)Image.FromStream(stream);
            var icon = Icon.FromHandle(bitmap.GetHicon());

            return new Icon(icon, new Size(PInvoke.GetSystemMetricsForDpi((int)SystemMetricFlag.SM_CXICON, dpi), PInvoke.GetSystemMetricsForDpi((int)SystemMetricFlag.SM_CYICON, dpi)));
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