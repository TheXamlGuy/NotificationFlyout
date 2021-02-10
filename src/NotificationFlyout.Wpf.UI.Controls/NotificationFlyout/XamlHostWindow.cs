using Microsoft.Toolkit.Wpf.UI.XamlHost;
using NotificationFlyout.Wpf.UI.Extensions;
using System.Windows;
using System.Windows.Media;

namespace NotificationFlyout.Wpf.UI.Controls
{
    internal class XamlHostWindow<TXamlContent> : Window where TXamlContent : class
    {
        internal const double WindowSize = 5;
        private WindowsXamlHost _xamlHost;

        public XamlHostWindow()
        {
            PrepareDefaultWindow();
            PrepareWindowsXamlHost();

            Loaded += OnLoaded;
        }


        internal TXamlContent GetHostContent()
        {
            if (_xamlHost == null) return null;
            return _xamlHost.GetUwpInternalObject() as TXamlContent;
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            this.Hidden();
        }

        private void PrepareDefaultWindow()
        {
            ShowInTaskbar = false;
            ShowActivated = false;
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            AllowsTransparency = true;
            Background = new SolidColorBrush(Colors.Red);
            Height = WindowSize;
            Width = WindowSize;
        }

        private void PrepareWindowsXamlHost()
        {
            _xamlHost = new WindowsXamlHost
            {
                InitialTypeName = typeof(TXamlContent).FullName
            };

            _xamlHost.Height = 0;
            _xamlHost.Width = 0;
            _xamlHost.HorizontalAlignment = HorizontalAlignment.Stretch;
            _xamlHost.VerticalAlignment = VerticalAlignment.Stretch;

            Content = _xamlHost;
        }
    }
}