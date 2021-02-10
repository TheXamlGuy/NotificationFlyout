using Microsoft.Toolkit.Wpf.UI.XamlHost;
using NotificationFlyout.Uwp.UI.Controls;
using NotificationFlyout.Wpf.UI.Extensions;
using NotificationFlyout.Uwp.UI.Extensions;
using NotificationFlyout.Wpf.UI.Helpers;
using System;
using System.Windows;
using System.Windows.Media;
using Windows.UI.Xaml.Controls.Primitives;

namespace NotificationFlyout.Wpf.UI.Controls
{
    internal class NotificationFlyoutXamlHost : Window
    {
        private const string ShellTrayHandleName = "Shell_TrayWnd";
        private const double WindowSize = 5;

        private Uwp.UI.Controls.NotificationFlyout _flyout;
        private bool _isLoaded;
        private NotificationIconHelper _notificationIconHelper;
        private SystemPersonalisationHelper _systemPersonalisationHelper;
        private TaskbarHelper _taskbarHelper;
        private WindowsXamlHost _xamlHost;

        public NotificationFlyoutXamlHost()
        {
            PrepareDefaultWindow();
            PrepareWindowsXamlHost();

            Loaded += OnLoaded;
        }

        public void SetFlyout(Uwp.UI.Controls.NotificationFlyout flyout)
        {
            if (_flyout != null)
            {
                _flyout.ContentChanged -= OnFlyoutContentChanged;
                _flyout.IconSourceChanged -= OnFlyoutIconSourceChanged;
                _flyout.RequestedThemeChanged -= OnFlyoutIconSourceChanged;
            }

            _flyout = flyout;
            _flyout.ContentChanged += OnFlyoutContentChanged;
            _flyout.IconSourceChanged += OnFlyoutIconSourceChanged;
            _flyout.RequestedThemeChanged += OnFlyoutRequestedThemeChanged;

            UpdateIcons();
            UpdateFlyoutContent();
            UpdateRequestedTheme();
        }

        internal void HideFlyout()
        {
            var flyoutHost = GetFlyoutHost();
            if (flyoutHost != null)
            {
                flyoutHost.HideFlyout();
            }
        }

        internal void ShowFlyout()
        {
            var flyoutHost = GetFlyoutHost();
            if (flyoutHost != null)
            {
                var taskbarState = _taskbarHelper.GetCurrentState();
                var flyoutPlacement = taskbarState.Position switch
                {
                    TaskbarPosition.Left => FlyoutPlacementMode.Right,
                    TaskbarPosition.Top => FlyoutPlacementMode.Bottom,
                    TaskbarPosition.Right => FlyoutPlacementMode.Left,
                    TaskbarPosition.Bottom => FlyoutPlacementMode.Top,
                    _ => throw new ArgumentOutOfRangeException(),
                };

                Activate();
                flyoutHost.ShowFlyout(flyoutPlacement);
            }
        }

        private NotificationFlyoutHost GetFlyoutHost()
        {
            if (_xamlHost == null) return null;
            return _xamlHost.GetUwpInternalObject() as NotificationFlyoutHost;
        }

        private void OnFlyoutContentChanged(object sender, EventArgs args)
        {
            UpdateFlyoutContent();
        }

        private void OnFlyoutIconSourceChanged(object sender, EventArgs args)
        {
            UpdateIcons();
        }

        private void OnFlyoutRequestedThemeChanged(object sender, EventArgs args)
        {
            UpdateRequestedTheme();
        }

        private void OnIconInvoked(object sender, NotificationIconInvokedEventArgs args)
        {
            ShowFlyout();
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            PrepareNotificationIcon();
            PrepareTaskbar();

            _isLoaded = true;

            UpdateWindow();
            this.Hidden();
        }

        private void OnTaskbarChanged(object sender, EventArgs args)
        {
            UpdateWindow();
        }

        private void OnThemeChanged(object sender, SystemPersonalisationChangedEventArgs args)
        {
            UpdateIcons();
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

        private void PrepareNotificationIcon()
        {
            _notificationIconHelper = NotificationIconHelper.Create(this);
            _notificationIconHelper.IconInvoked += OnIconInvoked;

            _systemPersonalisationHelper = SystemPersonalisationHelper.Create(this);
            _systemPersonalisationHelper.ThemeChanged += OnThemeChanged;
        }

        private void PrepareTaskbar()
        {
            _taskbarHelper = TaskbarHelper.Create(this);
            _taskbarHelper.TaskbarChanged += OnTaskbarChanged;
        }

        private void PrepareWindowsXamlHost()
        {
            _xamlHost = new WindowsXamlHost
            {
                InitialTypeName = typeof(NotificationFlyoutHost).FullName
            };

            _xamlHost.Height = 0;
            _xamlHost.Width = 0;

            Content = _xamlHost;
        }

        private void UpdateFlyoutContent()
        {
            if (_flyout == null) return;

            var content = _flyout.Content;
            if (content == null) return;

            var flyoutHost = GetFlyoutHost();
            if (flyoutHost != null)
            {
                flyoutHost.Content = content;
            }
        }

        private async void UpdateIcons()
        {
            if (!_isLoaded) return;

            if (_flyout == null) return;

            var _defaultIconSource = _flyout.IconSource;
            var _lightIconSource = _flyout.LightIconSource;

            var shellTrayHandle = WindowHelper.GetHandle(ShellTrayHandleName);
            if (shellTrayHandle == null) return;

            var dpi = WindowHelper.GetDpi(shellTrayHandle);

            var iconSource = _systemPersonalisationHelper.Theme == SystemTheme.Dark ? _defaultIconSource : _lightIconSource;
            if (iconSource == null) return;

            using var icon = await iconSource.ConvertToIconAsync(dpi);
            _notificationIconHelper.SetIcon(icon.Handle);
        }

        private void UpdateRequestedTheme()
        {
            if (_flyout == null) return;

            var requestedTheme = _flyout.RequestedTheme;

            var flyoutHost = GetFlyoutHost();
            if (flyoutHost != null)
            {
                flyoutHost.RequestedTheme = requestedTheme;
            }
        }

        private void UpdateWindow()
        {
            if (!_isLoaded) return;

            var flyoutHost = GetFlyoutHost();
            if (flyoutHost == null) return;

            var taskbarState = _taskbarHelper.GetCurrentState();

            Left = taskbarState.Screen.WorkingArea.Left;
            Top = taskbarState.Screen.WorkingArea.Top;

            var windowWidth = WindowSize * this.DpiX();
            var windowHeight = WindowSize * this.DpiY();

            double top, left, height, width;
       
            var taskbarRect = taskbarState.Rect;
            switch (taskbarState.Position)
            {
                case TaskbarPosition.Left:
                    top = taskbarRect.Bottom - windowHeight;
                    left = taskbarRect.Right;
                    height = windowHeight;
                    width = windowWidth;
                    break;
                case TaskbarPosition.Top:
                    top = taskbarRect.Bottom;
                    left = FlowDirection == FlowDirection.RightToLeft ? taskbarRect.Left : taskbarRect.Right - windowWidth;
                    height = windowHeight;
                    width = windowWidth;
                    break;
                case TaskbarPosition.Right:
                    top = taskbarRect.Bottom - windowHeight;
                    left = taskbarRect.Left - windowWidth;
                    height = windowHeight;
                    width = windowWidth;
                    break;
                case TaskbarPosition.Bottom:
                    top = taskbarRect.Top - windowHeight;
                    left = FlowDirection == FlowDirection.RightToLeft ? taskbarRect.Left : taskbarRect.Right - windowWidth;
                    height = windowHeight;
                    width = windowWidth;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            this.SetWindowPosition(top, left, height, width);
            flyoutHost.SetFlyoutPlacement(taskbarState.Position.ToString());
        }
    }
}