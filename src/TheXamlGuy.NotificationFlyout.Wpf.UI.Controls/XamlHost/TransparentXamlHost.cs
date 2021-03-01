using Microsoft.Toolkit.Wpf.UI.XamlHost;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace TheXamlGuy.NotificationFlyout.Wpf.UI.Controls
{
    internal class TransparentXamlHost<TXamlContent> : XamlHost<TXamlContent> where TXamlContent : Windows.UI.Xaml.UIElement
    {
        internal const double WindowSize = 1;

        public TransparentXamlHost() => PrepareDefaultWindow();

        protected override WindowsXamlHost OnPreparingXamlHost(WindowsXamlHost xamlHost)
        {
            xamlHost.Height = 0;
            xamlHost.Width = 0;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                Hide();
            }), DispatcherPriority.ContextIdle, null);

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