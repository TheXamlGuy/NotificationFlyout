using Microsoft.Toolkit.Wpf.UI.XamlHost;
using System;
using System.Windows;
using System.Windows.Media;

namespace TheXamlGuy.NotificationFlyout.Wpf.UI.Controls
{
    internal class TransparentXamlHost<TXamlContent> : XamlHost<TXamlContent> where TXamlContent : Windows.UI.Xaml.UIElement
    {
        internal const double WindowSize = 10;

        public TransparentXamlHost() => PrepareDefaultWindow();

        protected override void OnContentRendered(EventArgs args) => Hide();

        protected override WindowsXamlHost OnPreparingXamlHost(WindowsXamlHost xamlHost)
        {
            xamlHost.Height = 0;
            xamlHost.Width = 0;

            return base.OnPreparingXamlHost(xamlHost);
        }

        private void PrepareDefaultWindow()
        {
            ShowInTaskbar = false;
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            AllowsTransparency = true;
            Background = new SolidColorBrush(Colors.Transparent);
            Height = WindowSize;
            Width = WindowSize;
        }
    }
}