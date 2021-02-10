using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace NotificationFlyout.Uwp.UI.Controls
{
    public class NotificationFlyout : DependencyObject
    {
        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.Register(nameof(IconSource),
                typeof(ImageSource), typeof(NotificationFlyout),
                new PropertyMetadata(null, OnIconPropertyChanged));

        public static readonly DependencyProperty LightIconSourceProperty =
          DependencyProperty.Register(nameof(LightIconSource),
              typeof(ImageSource), typeof(NotificationFlyout),
                new PropertyMetadata(null, OnIconPropertyChanged));

        public static DependencyProperty ContentProperty =
            DependencyProperty.Register(nameof(Content),
                 typeof(UIElement), typeof(NotificationFlyout),
                 new PropertyMetadata(null));

        internal event EventHandler ContentChanged;
        internal event EventHandler IconSourceChanged;

        public UIElement Content
        {
            get => (UIElement)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

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

        private static void OnIconPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var sender = dependencyObject as NotificationFlyout;
            sender?.OnIconPropertyChanged();
        }

        private void OnIconPropertyChanged()
        {
            IconSourceChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
