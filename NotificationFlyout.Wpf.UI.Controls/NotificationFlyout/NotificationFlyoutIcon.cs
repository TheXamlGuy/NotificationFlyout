using System.Windows;
using System.Windows.Media;

namespace NotificationFlyout.Wpf.UI.Controls
{
    public class NotificationFlyoutIcon : DependencyObject
    {
        public static readonly DependencyProperty IconSourceProperty =
          DependencyProperty.Register(nameof(IconSource),
              typeof(ImageSource), typeof(NotificationFlyout));

        public static readonly DependencyProperty LightIconSourceProperty =
          DependencyProperty.Register(nameof(LightIconSource),
              typeof(ImageSource), typeof(NotificationFlyout));

        public ImageSource IconSource
        {
            get => (ImageSource)GetValue(IconSourceProperty);
            set => SetValue(IconSourceProperty, value);
        }

        public ImageSource LightIconSource
        {
            get => (ImageSource)GetValue(LightIconSourceProperty);
            set => SetValue(LightIconSourceProperty, value);
        }
    }
}