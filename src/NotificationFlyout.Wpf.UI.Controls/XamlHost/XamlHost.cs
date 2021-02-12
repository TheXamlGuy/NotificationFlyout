using Microsoft.Toolkit.Wpf.UI.XamlHost;
using System;
using System.Windows;

namespace NotificationFlyout.Wpf.UI.Controls
{
    internal class XamlHost<TXamlContent> : Window where TXamlContent : Windows.UI.Xaml.UIElement
    {
        protected new bool IsLoaded;
        private WindowsXamlHost _xamlHost;

        public XamlHost()
        {
            PrepareWindowsXamlHost();
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

        protected virtual WindowsXamlHost OnPreparingXamlHost(WindowsXamlHost xamlHost)
        {
            xamlHost.InitialTypeName = typeof(TXamlContent).FullName;
            xamlHost.HorizontalAlignment = HorizontalAlignment.Stretch;
            xamlHost.VerticalAlignment = VerticalAlignment.Stretch;

            return xamlHost;
        }

        private void OnContentRendered(object sender, EventArgs args)
        {
            IsLoaded = true;
            OnContentLoaded();
        }

        private void PrepareWindowsXamlHost()
        {
            _xamlHost = new WindowsXamlHost();
            OnPreparingXamlHost(_xamlHost);

            Content = _xamlHost;
        }
    }
}