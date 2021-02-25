using TheXamlGuy.NotificationFlyout.Common.Extensions;
using TheXamlGuy.NotificationFlyout.Common.Helpers;
using TheXamlGuy.NotificationFlyout.Uwp.UI.Controls;
using TheXamlGuy.NotificationFlyout.Wpf.UI.Extensions;
using System;
using System.Windows;
using Windows.UI.Xaml.Controls.Primitives;
using System.Windows.Media.Imaging;

namespace TheXamlGuy.NotificationFlyout.Wpf.UI.Controls
{
    internal class NotificationFlyoutXamlHost : TransparentXamlHost<NotificationFlyoutPresenterHost>
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
                _flyout.PlacementChanged -= OnPlacementChanged;
            }

            _flyout = flyout;
            _flyout.IconSourceChanged += OnIconSourceChanged;
            _flyout.ContextMenuChanged += OnContextMenuChanged;
            _flyout.PlacementChanged += OnPlacementChanged;

            var content = GetHostContent();
            if (content != null)
            {
                content.SetOwningFlyout(_flyout);
            }

            UpdateIcons();
            UpdateContextMenu();
        }

        internal void ShowFlyout()
        {
            var content = GetHostContent();
            if (content != null)
            {
                var taskbarState = _taskbarHelper.GetCurrentState();
                var flyoutPlacement = FlyoutPlacementMode.Top;

                switch (_flyout.Placement)
                {
                    case NotificationFlyoutPlacement.Auto:
                        flyoutPlacement = taskbarState.Placement switch
                        {
                            TaskbarPlacement.Left => FlyoutPlacementMode.Right,
                            TaskbarPlacement.Top => FlyoutPlacementMode.Bottom,
                            TaskbarPlacement.Right => FlyoutPlacementMode.Left,
                            TaskbarPlacement.Bottom => FlyoutPlacementMode.Top,
                            _ => throw new ArgumentOutOfRangeException(),
                        };
                        break;
                }

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
            UpdatePlacement();
            UpdateTheme();
        }

        protected override void OnDeactivated(EventArgs args) => HideFlyout();

        private void OnContextMenuChanged(object sender, EventArgs args) => UpdateContextMenu();

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

        private void OnPlacementChanged(object sender, EventArgs args) => UpdatePlacement();

        private void OnTaskbarChanged(object sender, EventArgs args) => UpdatePlacement();

        private void OnThemeChanged(object sender, SystemPersonalisationChangedEventArgs args)
        {
            UpdateTheme();
            UpdateIcons();
        }

        private void UpdateContextMenu()
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

            var shellTrayHandle = WindowHelper.GetHandle(ShellTrayHandleName);
            if (shellTrayHandle == null) return;

            var dpi = WindowHelper.GetDpi(shellTrayHandle);

            var desiredIconSource = _systemPersonalisationHelper.Theme == SystemTheme.Dark ? _flyout.IconSource : _flyout.LightIconSource;
            if (desiredIconSource == null)
            {
                var fallbackIconSource = new BitmapImage(new Uri($"pack://application:,,,/{GetType().Namespace};component/Assets/notification-icon-{(_systemPersonalisationHelper.Theme == SystemTheme.Dark ? "default" : "light")}.ico"));
                using var icon = fallbackIconSource.ConvertToIcon(dpi);
                _notificationIconHelper.SetIcon(icon.Handle);
            }
            else
            {
                using var icon = await desiredIconSource.ConvertToIconAsync(dpi);
                _notificationIconHelper.SetIcon(icon.Handle);
            }
        }

        private void UpdateTheme()
        {
            var content = GetHostContent();
            if (content != null)
            {
                content.UpdateTheme(_systemPersonalisationHelper.IsColorPrevalence);
            }
        }

        private void UpdatePlacement()
        {
            if (!IsLoaded) return;

            var flyoutHost = GetHostContent();
            if (flyoutHost == null) return;

            var taskbarState = _taskbarHelper.GetCurrentState();
            Left = taskbarState.Screen.WorkingArea.Left;
            Top = taskbarState.Screen.WorkingArea.Top;

            var width = WindowSize * this.DpiX();
            var height = WindowSize * this.DpiY();

            double top = 0, left = 0;

            var taskbarRect = taskbarState.Rect;
            var taskBarPlacement = taskbarState.Placement;
            var presenterPlacement = NotificationFlyoutPresenterPlacement.Bottom;

            switch (_flyout.Placement)
            {
                case NotificationFlyoutPlacement.Auto:
                    switch (taskBarPlacement)
                    {
                        case TaskbarPlacement.Left:
                            presenterPlacement = NotificationFlyoutPresenterPlacement.Left;
                            top = taskbarRect.Bottom - height;
                            left = taskbarRect.Right;
                            break;

                        case TaskbarPlacement.Top:
                            presenterPlacement = NotificationFlyoutPresenterPlacement.Top;
                            top = taskbarRect.Bottom;
                            left = FlowDirection == FlowDirection.RightToLeft ? taskbarRect.Left : taskbarRect.Right - width;
                            break;

                        case TaskbarPlacement.Right:
                            presenterPlacement = NotificationFlyoutPresenterPlacement.Right;
                            top = taskbarRect.Bottom - height;
                            left = taskbarRect.Left - width;
                            break;

                        case TaskbarPlacement.Bottom:
                            presenterPlacement = NotificationFlyoutPresenterPlacement.Bottom;
                            top = taskbarRect.Top - height;
                            left = FlowDirection == FlowDirection.RightToLeft ? taskbarRect.Left : taskbarRect.Right - width;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case NotificationFlyoutPlacement.Right:
                    presenterPlacement = NotificationFlyoutPresenterPlacement.FullRight;
                    switch (taskBarPlacement)
                    {
                        case TaskbarPlacement.Left:
                        case TaskbarPlacement.Top:
                        case TaskbarPlacement.Right:
                            left = taskbarState.Screen.Bounds.Width - width;
                            top = taskbarState.Screen.Bounds.Height - height;
                            break;

                        case TaskbarPlacement.Bottom:
                            top = taskbarRect.Top - height;
                            left = FlowDirection == FlowDirection.RightToLeft ? taskbarRect.Left : taskbarRect.Right - width;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
            }

            this.SetWindowPosition(top, left, height, width);
            flyoutHost.UpdatePlacement(presenterPlacement);
        }
    }
}