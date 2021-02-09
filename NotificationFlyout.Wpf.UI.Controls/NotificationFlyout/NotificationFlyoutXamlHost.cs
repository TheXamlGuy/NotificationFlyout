using Microsoft.Toolkit.Wpf.UI.XamlHost;
using NotificationFlyout.Uwp.UI.Controls;
using NotificationFlyout.Wpf.UI.Extensions;
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

        private ImageSource _defaultIconSource;
        private bool _isLoaded;
        private ImageSource _lightIconSource;
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

        public void SetFlyoutPresenter(NotificationFlyoutPresenter flyoutPresenter)
        {
            var flyoutHost = GetFlyoutHost();
            if (flyoutHost != null)
            {
                flyoutHost.FlyoutPresenter = flyoutPresenter;

                var theme = _systemPersonalisationHelper.Theme.ToString();
                var isColorPrevalence = _systemPersonalisationHelper.IsColorPrevalence;

                flyoutHost.FlyoutPresenter.UpdateFlyoutTheme(theme, isColorPrevalence);
            }
        }

        internal void HideFlyout()
        {
            var flyoutHost = GetFlyoutHost();
            if (flyoutHost != null)
            {
                flyoutHost.HideFlyout();
            }
        }

        internal void SetIcons(ImageSource defaultIconSource, ImageSource lightIconSource)
        {
            _defaultIconSource = defaultIconSource;
            _lightIconSource = lightIconSource;

            UpdateIcon();
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
            UpdateIcon();
            this.Hidden();
        }

        private void OnTaskbarChanged(object sender, EventArgs args)
        {
            UpdateWindow();
        }

        private void OnThemeChanged(object sender, SystemPersonalisationChangedEventArgs args)
        {
            NewMethod(args);
            UpdateIcon();
        }

        private void NewMethod(SystemPersonalisationChangedEventArgs args)
        {
            var flyoutHost = GetFlyoutHost();
            if (flyoutHost != null)
            {
                var theme = args.Theme.ToString();
                var isColorPrevalence = args.IsColorPrevalence;

                flyoutHost.FlyoutPresenter.UpdateFlyoutTheme(theme, isColorPrevalence);
            }
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

        private void UpdateIcon()
        {
            if (!_isLoaded) return;

            var shellTrayHandle = WindowHelper.GetHandle(ShellTrayHandleName);
            if (shellTrayHandle == null) return;

            var dpi = WindowHelper.GetDpi(shellTrayHandle);

            var iconSource = _systemPersonalisationHelper.Theme == SystemTheme.Dark ? _defaultIconSource : _lightIconSource;
            if (iconSource == null) return;

            using var icon = iconSource.ConvertToIcon(dpi);
            _notificationIconHelper.SetIcon(icon.Handle);
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