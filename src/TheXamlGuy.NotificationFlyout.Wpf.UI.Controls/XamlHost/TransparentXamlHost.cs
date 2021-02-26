using Microsoft.Toolkit.Wpf.UI.XamlHost;
using TheXamlGuy.NotificationFlyout.Wpf.UI.Extensions;
using System.Windows;
using System.Windows.Media;

namespace TheXamlGuy.NotificationFlyout.Wpf.UI.Controls
{
    internal class TransparentXamlHost<TXamlContent> : XamlHost<TXamlContent> where TXamlContent : Windows.UI.Xaml.UIElement
    {
        internal const double WindowSize = 0;

        public TransparentXamlHost()
        {
            Loaded += OnLoaded;
            PrepareDefaultWindow();
        }

        protected override WindowsXamlHost OnPreparingXamlHost(WindowsXamlHost xamlHost)
        {
            xamlHost.Height = 0;
            xamlHost.Width = 0;

            return base.OnPreparingXamlHost(xamlHost);
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
            Background = new SolidColorBrush(Colors.Transparent);
            Height = WindowSize;
            Width = WindowSize;
        }
    }
}