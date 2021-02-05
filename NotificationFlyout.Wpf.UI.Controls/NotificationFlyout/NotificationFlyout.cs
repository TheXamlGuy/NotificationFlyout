using NotificationFlyout.Wpf.UI.Extensions;
using NotificationFlyout.Wpf.UI.Helpers;
using System.Windows;

namespace NotificationFlyout.Wpf.UI.Controls
{
    public class NotificationFlyout : DependencyObject
    {
        private const string ShellTrayHandleName = "Shell_TrayWnd";

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon),
                typeof(NotificationFlyoutIcon), typeof(NotificationFlyout),
                new PropertyMetadata(null, OnIconPropertyChanged));

        public static DependencyProperty FlyoutContentProperty =
            DependencyProperty.Register(nameof(FlyoutContent),
                 typeof(Windows.UI.Xaml.UIElement), typeof(NotificationFlyout),
                 new PropertyMetadata(null, OnFlyoutContentPropertyChanged));

        private NotificationFlyoutXamlHost _xamlHost;

        public NotificationFlyout()
        {
            _xamlHost = new NotificationFlyoutXamlHost();
            _xamlHost.Show();
        }

        public Windows.UI.Xaml.UIElement FlyoutContent
        {
            get => (Windows.UI.Xaml.UIElement)GetValue(FlyoutContentProperty);
            set => SetValue(FlyoutContentProperty, value);
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

        private static void OnFlyoutContentPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var sender = dependencyObject as NotificationFlyout;
            sender?.OnFlyoutContentPropertyChanged();
        }

        private static void OnIconPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var sender = dependencyObject as NotificationFlyout;
            sender?.OnIconPropertyChanged();
        }

        private void OnFlyoutContentPropertyChanged()
        {
            _xamlHost.FlyoutContent = FlyoutContent;
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