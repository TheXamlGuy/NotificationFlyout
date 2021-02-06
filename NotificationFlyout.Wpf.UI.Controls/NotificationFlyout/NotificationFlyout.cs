using NotificationFlyout.Uwp.UI.Controls;
using NotificationFlyout.Wpf.UI.Extensions;
using NotificationFlyout.Wpf.UI.Helpers;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace NotificationFlyout.Wpf.UI.Controls
{
    [ContentProperty(nameof(FlyoutPresenter))]
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

        public static DependencyProperty FlyoutPresenterProperty =
            DependencyProperty.Register(nameof(FlyoutPresenter),
                 typeof(NotificationFlyoutPresenter), typeof(NotificationFlyout),
                 new PropertyMetadata(null, OnFlyoutPresenterPropertyChanged));

        private const string ShellTrayHandleName = "Shell_TrayWnd";

        private readonly NotificationFlyoutXamlHost _xamlHost;

        public NotificationFlyout()
        {
            _xamlHost = new NotificationFlyoutXamlHost();
            _xamlHost.Show();
        }

        public NotificationFlyoutPresenter FlyoutPresenter
        {
            get => (NotificationFlyoutPresenter)GetValue(FlyoutPresenterProperty);
            set => SetValue(FlyoutPresenterProperty, value);
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
            _xamlHost.SetFlyoutPresenter(FlyoutPresenter);
        }

        private void OnIconPropertyChanged()
        {
            var shellTrayHandle = WindowHelper.GetHandle(ShellTrayHandleName);
            if (shellTrayHandle == null) return;

            var dpi = WindowHelper.GetDpi(shellTrayHandle);

            var iconSource = SystemSettingsHelper.DefaultSystemTheme == SystemTheme.Dark ? IconSource : LightIconSource;
            using var icon = iconSource.ConvertToIcon(dpi);

            _xamlHost.SetNotificationIcon(icon.Handle);
        }
    }
}