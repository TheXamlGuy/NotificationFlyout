using TheXamlGuy.NotificationFlyout.Shared.UI.Helpers;
using System.Windows;
using System.Windows.Markup;
using TheXamlGuy.NotificationFlyout.Shared.UI;

namespace TheXamlGuy.NotificationFlyout.Wpf.UI.Controls
{
    [ContentProperty(nameof(Flyout))]
    public class NotificationFlyoutApplication : DependencyObject, INotificationFlyoutApplication
    {
        public static DependencyProperty FlyoutProperty =
            DependencyProperty.Register(nameof(Flyout),
                typeof(Uwp.UI.Controls.NotificationFlyout), typeof(NotificationFlyoutApplication),
                new PropertyMetadata(null, OnFlyoutPropertyChanged));

        private static NotificationFlyoutApplication _application;
        private NotificationFlyoutXamlHost _notificationFlyoutXamlHost;

        public NotificationFlyoutApplication()
        {
            _application = this;
            Uwp.UI.Controls.NotificationFlyout.SetApplication(this);

            WndProcListener.Current.Start();
            PrepareFlyoutHost();
        }

        public static INotificationFlyoutApplication Current => _application;

        public Uwp.UI.Controls.NotificationFlyout Flyout
        {
            get => (Uwp.UI.Controls.NotificationFlyout)GetValue(FlyoutProperty);
            set => SetValue(FlyoutProperty, value);
        }

        public void Exit() => _notificationFlyoutXamlHost.Close();

        public void HideFlyout() => _notificationFlyoutXamlHost.HideFlyout();

        public void OpenAsWindow<TUIElement>() where TUIElement : Windows.UI.Xaml.UIElement
        {
            var window = new XamlHost<TUIElement>();
            window.Show();
        }

        public void ShowFlyout() => _notificationFlyoutXamlHost.ShowFlyout();

        private static void OnFlyoutPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var sender = dependencyObject as NotificationFlyoutApplication;
            sender?.OnFlyoutPropertyChanged();
        }

        private void OnFlyoutPropertyChanged() => _notificationFlyoutXamlHost.SetOwningFlyout(Flyout);

        private void PrepareFlyoutHost()
        {
            _notificationFlyoutXamlHost = new NotificationFlyoutXamlHost();
            _notificationFlyoutXamlHost.Show();
        }
    }
}