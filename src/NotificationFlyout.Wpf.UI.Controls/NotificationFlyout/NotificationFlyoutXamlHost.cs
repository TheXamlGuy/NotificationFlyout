using NotificationFlyout.Uwp.UI.Controls;
using NotificationFlyout.Wpf.UI.Extensions;
using NotificationFlyout.Uwp.UI.Extensions;
using NotificationFlyout.Wpf.UI.Helpers;
using System;
using System.Windows;
using Windows.UI.Xaml.Controls.Primitives;
using System.Windows.Input;

namespace NotificationFlyout.Wpf.UI.Controls
{
    internal class NotificationFlyoutXamlHost : XamlHostWindow<NotificationFlyoutHost>
    {
        private const string ShellTrayHandleName = "Shell_TrayWnd";

        private Uwp.UI.Controls.NotificationFlyout _flyout;

        private NotificationIconHelper _notificationIconHelper;
        private SystemPersonalisationHelper _systemPersonalisationHelper;
        private TaskbarHelper _taskbarHelper;

        internal event EventHandler ContextMenuRequested;

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
            var flyoutHost = GetHostContent();
            if (flyoutHost != null)
            {
                flyoutHost.HideFlyout();
            }
        }

        internal void ShowFlyout()
        {
            var flyoutHost = GetHostContent();
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

        protected override void OnContentLoaded()
        {
            PrepareNotificationIcon();
            PrepareTaskbar();
            UpdateWindow();
        }

        protected override void OnDeactivated(EventArgs args)
        {
            HideFlyout();
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
            if (args.MouseButton == MouseButton.Left)
            {
                ShowFlyout();
            }

            if (args.MouseButton == MouseButton.Right)
            {
                ContextMenuRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnTaskbarChanged(object sender, EventArgs args)
        {
            UpdateWindow();
        }

        private void OnThemeChanged(object sender, SystemPersonalisationChangedEventArgs args)
        {
            UpdateIcons();
        }

        private void PrepareNotificationIcon()
        {
            _notificationIconHelper = NotificationIconHelper.Create(this);
            _notificationIconHelper.IconInvoked += OnIconInvoked;

            _systemPersonalisationHelper = SystemPersonalisationHelper.Create(this);
            _systemPersonalisationHelper.ThemeChanged += OnThemeChanged;

            UpdateIcons();
        }

        private void PrepareTaskbar()
        {
            _taskbarHelper = TaskbarHelper.Create(this);
            _taskbarHelper.TaskbarChanged += OnTaskbarChanged;
        }

        protected override void OnClosed(EventArgs args)
        {
            _notificationIconHelper.Dispose();
        }

        private void UpdateFlyoutContent()
        {
            if (_flyout == null) return;

            var content = _flyout.Content;
            if (content == null) return;

            var flyoutHost = GetHostContent();
            if (flyoutHost != null)
            {
                flyoutHost.Content = content;
            }
        }

        private async void UpdateIcons()
        {
            if (!IsLoaded) return;

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

            var flyoutHost = GetHostContent();
            if (flyoutHost != null)
            {
                flyoutHost.RequestedTheme = requestedTheme;
            }
        }

        private void UpdateWindow()
        {
            if (!IsLoaded) return;

            var flyoutHost = GetHostContent();
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