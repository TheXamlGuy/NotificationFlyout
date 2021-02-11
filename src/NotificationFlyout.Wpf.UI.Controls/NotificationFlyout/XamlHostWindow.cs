using Microsoft.Toolkit.Wpf.UI.XamlHost;
using NotificationFlyout.Wpf.UI.Extensions;
using System;
using System.Windows;
using System.Windows.Media;

namespace NotificationFlyout.Wpf.UI.Controls
{
    internal class XamlHostWindow<TXamlContent> : Window where TXamlContent : Windows.UI.Xaml.UIElement
    {
        internal const double WindowSize = 5;
        protected new bool IsLoaded;
        private WindowsXamlHost _xamlHost;
        public XamlHostWindow()
        {
            PrepareDefaultWindow();
            PrepareWindowsXamlHost();

            Loaded += OnLoaded;
            ContentRendered += OnContentRendered;
        }

        internal TXamlContent GetHostContent()
        {
            if (_xamlHost == null) return null;
            return _xamlHost.GetUwpInternalObject() as TXamlContent;
        }

        protected virtual void OnContentLoaded()
        {
        }

        private void OnContentRendered(object sender, EventArgs args)
        {
            IsLoaded = true;
            OnContentLoaded();
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

        private void PrepareWindowsXamlHost()
        {
            _xamlHost = new WindowsXamlHost
            {
                InitialTypeName = typeof(TXamlContent).FullName,
                Height = 0,
                Width = 0,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            Content = _xamlHost;
        }
    }
}