using NotificationFlyout.Uwp.UI.Controls;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace NotificationFlyout.Wpf.UI.Controls
{
    [ContentProperty(nameof(Content))]
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
                 typeof(Windows.UI.Xaml.UIElement), typeof(NotificationFlyout),
                 new PropertyMetadata(null, OnFlyoutPresenterPropertyChanged));

        private readonly NotificationFlyoutXamlHost _xamlHost;

        public NotificationFlyout()
        {
            _xamlHost = new NotificationFlyoutXamlHost();
            _xamlHost.Show();
        }

        public Windows.UI.Xaml.UIElement Content
        {
            get => (Windows.UI.Xaml.UIElement)GetValue(ContentProperty);
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

        public void HideFlyout()
        {
            _xamlHost.HideFlyout();
        }

        public void ShowFlyout()
        {
            _xamlHost.ShowFlyout();
        }

        private static void OnFlyoutPresenterPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var sender = dependencyObject as NotificationFlyout;
            sender?.OnFlyoutPresenterPropertyChanged();
        }

        private static void OnIconPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var sender = dependencyObject as NotificationFlyout;
            sender?.OnIconPropertyChanged();
        }

        private void OnFlyoutPresenterPropertyChanged()
        {
            _xamlHost.SetFlyoutContent(Content);
        }

        private void OnIconPropertyChanged()
        {
            _xamlHost.SetIcons(IconSource, LightIconSource);
        }
    }
}