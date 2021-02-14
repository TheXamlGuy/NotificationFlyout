using TheXamlGuy.NotificationFlyout.Shared.UI.Extensions;
using TheXamlGuy.NotificationFlyout.Shared.UI.Helpers;
using TheXamlGuy.NotificationFlyout.Uwp.UI.Controls;
using TheXamlGuy.NotificationFlyout.Wpf.UI.Extensions;
using System;
using System.Windows;
using Windows.UI.Xaml.Controls.Primitives;

namespace TheXamlGuy.NotificationFlyout.Wpf.UI.Controls
{
    internal class NotificationFlyoutXamlHost : TransparentXamlHost<NotificationFlyoutHost>
    {
        private const string ShellTrayHandleName = "Shell_TrayWnd";

        private NotificationFlyoutContextMenuXamlHost _contextMenuXamlHost;
        private Uwp.UI.Controls.NotificationFlyout _flyout;
        private NotificationIconHelper _notificationIconHelper;
        private SystemPersonalisationHelper _systemPersonalisationHelper;
        private TaskbarHelper _taskbarHelper;

        internal void HideFlyout()
        {
            var flyoutHost = GetHostContent();
            if (flyoutHost != null)
            {
                flyoutHost.HideFlyout();
            }
        }

        internal void SetOwningFlyout(Uwp.UI.Controls.NotificationFlyout flyout)
        {
            if (_flyout != null)
            {
                _flyout.IconSourceChanged -= OnIconSourceChanged;
                _flyout.ContextMenuChanged -= OnContextMenuChanged;
            }

            _flyout = flyout;
            _flyout.IconSourceChanged += OnIconSourceChanged;
            _flyout.ContextMenuChanged += OnContextMenuChanged;

            var content = GetHostContent();
            if (content != null)
            {
                content.SetOwningFlyout(_flyout);
            }

            UpdateIcons();
            PrepareContextMenu();
        }

        internal void ShowFlyout()
        {
            var content = GetHostContent();
            if (content != null)
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
                content.ShowFlyout(flyoutPlacement);
            }
        }

        protected override void OnClosed(EventArgs args)
        {
            _notificationIconHelper.Dispose();

            if (_contextMenuXamlHost == null) return;
            _contextMenuXamlHost.Close();
        }

        protected override void OnContentLoaded()
        {
            PrepareNotificationIcon();
            PrepareTaskbar();
            UpdateWindow();
        }

        protected override void OnDeactivated(EventArgs args) => HideFlyout();

        private void OnContextMenuChanged(object sender, EventArgs args) => PrepareContextMenu();

        private void OnIconInvoked(object sender, NotificationIconInvokedEventArgs args)
        {
            if (args.PointerButton == PointerButton.Left)
            {
                ShowFlyout();
            }

            if (args.PointerButton == PointerButton.Right)
            {
                ShowContextMenuFlyout();
            }
        }

        private void OnIconSourceChanged(object sender, EventArgs args) => UpdateIcons();

        private void OnTaskbarChanged(object sender, EventArgs args) => UpdateWindow();

        private void OnThemeChanged(object sender, SystemPersonalisationChangedEventArgs args)
        {
            UpdateFlyoutTheme(args.IsColorPrevalence);
            UpdateIcons();
        }

        private void UpdateFlyoutTheme(bool isColorPrevalence)
        {
            var content = GetHostContent();
            if (content != null)
            {
             //   content.UpdateFlyoutTheme(isColorPrevalence);
            }
        }

        private void PrepareContextMenu()
        {
            if (_contextMenuXamlHost != null)
            {
                _contextMenuXamlHost.Close();
                _contextMenuXamlHost = null;
            }

            var contextMenu = _flyout.ContextMenu;
            if (contextMenu == null) return;

            if (_contextMenuXamlHost == null)
            {
                _contextMenuXamlHost = new NotificationFlyoutContextMenuXamlHost();
                _contextMenuXamlHost.Show();
            }

            _contextMenuXamlHost.SetOwningFlyout(_flyout);
        }

        private void PrepareNotificationIcon()
        {
            _notificationIconHelper = NotificationIconHelper.Create();
            _notificationIconHelper.IconInvoked += OnIconInvoked;

            _systemPersonalisationHelper = SystemPersonalisationHelper.Current;
            _systemPersonalisationHelper.ThemeChanged += OnThemeChanged;

            UpdateIcons();
        }

        private void PrepareTaskbar()
        {
            _taskbarHelper = TaskbarHelper.Create();
            _taskbarHelper.TaskbarChanged += OnTaskbarChanged;
        }

        private void ShowContextMenuFlyout()
        {
            if (_contextMenuXamlHost == null) return;
            _contextMenuXamlHost.ShowContextMenuFlyout();
        }

        private async void UpdateIcons()
        {
            if (!IsLoaded) return;
            if (_flyout == null) return;

            var iconSource = _flyout.IconSource;
            var lightIconSource = _flyout.LightIconSource;

            var shellTrayHandle = WindowHelper.GetHandle(ShellTrayHandleName);
            if (shellTrayHandle == null) return;

            var desiredIconSource = _systemPersonalisationHelper.Theme == SystemTheme.Dark ? iconSource : lightIconSource;
            if (desiredIconSource == null) return;

            var dpi = WindowHelper.GetDpi(shellTrayHandle);
            using var icon = await desiredIconSource.ConvertToIconAsync(dpi);
            _notificationIconHelper.SetIcon(icon.Handle);
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