using NotificationFlyout.Wpf.UI.Extensions;
using NotificationFlyout.Wpf.UI.Helpers;
using System.Windows;
using System.Windows.Markup;

namespace NotificationFlyout.Wpf.UI.Controls
{
    [ContentProperty(nameof(Content))]
    public class NotificationFlyout : DependencyObject
    {
        private const string ShellTrayHandleName = "Shell_TrayWnd";

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon),
                typeof(NotificationFlyoutIcon), typeof(NotificationFlyout),
                new PropertyMetadata(null, OnIconPropertyChanged));

        public static DependencyProperty ContentProperty =
            DependencyProperty.Register(nameof(Content),
                 typeof(Windows.UI.Xaml.UIElement), typeof(NotificationFlyout),
                 new PropertyMetadata(null, OnContentPropertyChanged));

        private NotificationFlyoutXamlHost _xamlHost;

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

        public NotificationFlyoutIcon Icon
        {
            get => (NotificationFlyoutIcon)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public void HideFlyout()
        {
            _xamlHost.HideFlyout();
        }

        public void ShowFlyout()
        {
            _xamlHost.ShowFlyout();
        }

        private static void OnContentPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var sender = dependencyObject as NotificationFlyout;
            sender?.OnContentPropertyChanged();
        }

        private static void OnIconPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var sender = dependencyObject as NotificationFlyout;
            sender?.OnIconPropertyChanged();
        }

        private void OnContentPropertyChanged()
        {
            _xamlHost.SetFlyoutContent(Content);
        }

        private void OnIconPropertyChanged()
        {
            var shellTrayHandle = WindowHelper.GetHandle(ShellTrayHandleName);
            if (shellTrayHandle == null) return;

            var dpi = WindowHelper.GetDpi(shellTrayHandle);

            var iconSource = SystemSettingsHelper.DefaultSystemTheme == SystemTheme.Dark ? Icon.IconSource : Icon.LightIconSource;
            using var icon = iconSource.ConvertToIcon(dpi);

            _xamlHost.SetNotificationIcon(icon.Handle);
        }
    }
}