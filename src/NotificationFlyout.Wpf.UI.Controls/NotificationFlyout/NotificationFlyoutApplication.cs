using System.Windows;
using System.Windows.Markup;

namespace NotificationFlyout.Wpf.UI.Controls
{
    [ContentProperty(nameof(Flyout))]
    public class NotificationFlyoutApplication : DependencyObject
    {
        public static DependencyProperty FlyoutProperty =
            DependencyProperty.Register(nameof(Flyout),
                typeof(Uwp.UI.Controls.NotificationFlyout), typeof(NotificationFlyoutApplication),
                new PropertyMetadata(null, OnFlyoutPropertyChanged));

        private readonly NotificationFlyoutXamlHostWindow _xamlHost;

        public NotificationFlyoutApplication()
        {
            _xamlHost = new NotificationFlyoutXamlHostWindow();
            _xamlHost.Show();
        }

        public Uwp.UI.Controls.NotificationFlyout Flyout
        {
            get => (Uwp.UI.Controls.NotificationFlyout)GetValue(FlyoutProperty);
            set => SetValue(FlyoutProperty, value);
        }

        public void HideFlyout()
        {
            _xamlHost.HideFlyout();
        }

        public void ShowFlyout()
        {
            _xamlHost.ShowFlyout();
        }

        private static void OnFlyoutPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var sender = dependencyObject as NotificationFlyoutApplication;
            sender?.OnFlyoutPropertyChanged();
        }

        private void OnFlyoutPropertyChanged()
        {
            _xamlHost.SetFlyout(Flyout);
        }
    }
}