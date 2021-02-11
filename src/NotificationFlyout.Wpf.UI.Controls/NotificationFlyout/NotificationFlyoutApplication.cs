using System;
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

        private readonly ContextMenuXamlHost _contextMenuXamlHost;
        private readonly NotificationFlyoutXamlHost _notificationFlyoutXamlHost;
        public NotificationFlyoutApplication()
        {
            _notificationFlyoutXamlHost = new NotificationFlyoutXamlHost();
            _notificationFlyoutXamlHost.ContextMenuRequested += OnContextMenuRequested;
            _notificationFlyoutXamlHost.Show();

            _contextMenuXamlHost = new ContextMenuXamlHost();
            _contextMenuXamlHost.Show();
        }

        public Uwp.UI.Controls.NotificationFlyout Flyout
        {
            get => (Uwp.UI.Controls.NotificationFlyout)GetValue(FlyoutProperty);
            set => SetValue(FlyoutProperty, value);
        }

        public void HideFlyout()
        {
            _notificationFlyoutXamlHost.HideFlyout();
        }

        public void ShowFlyout()
        {
            _notificationFlyoutXamlHost.ShowFlyout();
        }

        private static void OnFlyoutPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var sender = dependencyObject as NotificationFlyoutApplication;
            sender?.OnFlyoutPropertyChanged();
        }

        private void OnContextMenuRequested(object sender, EventArgs args)
        {
            _contextMenuXamlHost?.ShowContextMenuFlyout();
        }

        private void OnFlyoutPropertyChanged()
        {
            _notificationFlyoutXamlHost.SetFlyout(Flyout);
            _contextMenuXamlHost.SetFlyout(Flyout);
        }
    }
}