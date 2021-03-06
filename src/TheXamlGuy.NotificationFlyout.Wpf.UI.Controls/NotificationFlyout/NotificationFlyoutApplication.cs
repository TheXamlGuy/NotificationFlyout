﻿using TheXamlGuy.NotificationFlyout.Common.Helpers;
using System.Windows;
using System.Windows.Markup;
using System;
using Windows.UI.Xaml.Controls;
using TheXamlGuy.NotificationFlyout.Uwp.UI.Controls;
using TheXamlGuy.NotificationFlyout.Wpf.UI.Extensions;
using System.Windows.Media.Imaging;
using TheXamlGuy.NotificationFlyout.Common.Extensions;
using Microsoft.Windows.Sdk;
using TheXamlGuy.NotificationFlyout.Shared.UI;
using System.Windows.Media;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows.Threading;
using System.Threading;

namespace TheXamlGuy.NotificationFlyout.Wpf.UI.Controls
{
    [ContentProperty(nameof(Flyout))]
    public class NotificationFlyoutApplication : DependencyObject, INotificationFlyoutApplication
    {
        public static DependencyProperty FlyoutProperty =
            DependencyProperty.Register(nameof(Flyout),
                typeof(Uwp.UI.Controls.NotificationFlyout), typeof(NotificationFlyoutApplication),
                new PropertyMetadata(null, OnFlyoutPropertyChanged));

        private const string ShellTrayHandleName = "Shell_TrayWnd";
        private readonly NotificationIconHelper _notificationIconHelper;
        private readonly SystemPersonalisationHelper _systemPersonalisationHelper;
        private readonly TaskbarHelper _taskbarHelper;
        private bool _isDpiChanging;
        private TransparentXamlHost<ContentControl> _notificationFlyoutXamlHost;

        public NotificationFlyoutApplication()
        {
            Uwp.UI.Controls.NotificationFlyout.SetApplication(this);
            
            _notificationIconHelper = NotificationIconHelper.Create();
            _notificationIconHelper.IconInvoked += OnIconInvoked;

            _taskbarHelper = TaskbarHelper.Create();
            _taskbarHelper.TaskbarChanged += OnTaskbarChanged;

            _systemPersonalisationHelper = SystemPersonalisationHelper.Current;
            _systemPersonalisationHelper.ThemeChanged += OnThemeChanged;

            PrepareFlyoutHost();
            WndProcListener.Current.Start();
        }

        public Uwp.UI.Controls.NotificationFlyout Flyout
        {
            get => (Uwp.UI.Controls.NotificationFlyout)GetValue(FlyoutProperty);
            set => SetValue(FlyoutProperty, value);
        }

        public void CloseFlyout(bool shouldRespectIsLightDismissEnbabled)
        {
            if (Flyout == null) return;
            Flyout.Close(shouldRespectIsLightDismissEnbabled);
        }

        public void CloseFlyout() => CloseFlyout(false);

        public void Exit() => _notificationFlyoutXamlHost.Close();

        public void OpenAsWindow<TUIElement>() where TUIElement : Windows.UI.Xaml.UIElement
        {
            var window = new XamlHost<TUIElement>();
            window.Hidden();
            window.Show();
        }

        public void OpenFlyout()
        {
            if (Flyout == null) return;
            _notificationFlyoutXamlHost.Activate();

            Flyout.Open();
            UpdateFlyoutPlacement();
        }

        private static void OnFlyoutPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var sender = dependencyObject as NotificationFlyoutApplication;
            sender?.OnFlyoutPropertyChanged();
        }

        private void OnFlyoutIconSourcePropertyChanged(object sender, EventArgs args) => UpdateIcons();

        private void OnFlyoutInteractedWith(object sender, EventArgs args) => _notificationFlyoutXamlHost.Activate();

        private void OnFlyoutPlacementPropertyChanged(object sender, EventArgs args) => UpdateFlyoutPlacement();

        private void OnFlyoutPropertyChanged() => PrepareFlyout();

        private void OnIconInvoked(object sender, NotificationIconInvokedEventArgs args)
        {
            if (args.PointerButton == PointerButton.Left)
            {
                OpenFlyout();
            }

            if (args.PointerButton == PointerButton.Right)
            {
                ShowContextMenu();
            }
        }

        private void OnNotificationFlyoutXamlHostClosed(object sender, EventArgs args) => _notificationIconHelper?.Dispose();

        private void OnNotificationFlyoutXamlHostDeactivated(object sender, EventArgs args) => CloseFlyout(true);

        private async void OnNotificationFlyoutXamlHostDpiChanged(object sender, DpiChangedEventArgs args)
        {
            if (_isDpiChanging)
                return;

            _isDpiChanging = true;

            await Application.Current.Dispatcher.Invoke(async () =>
            {
                _notificationFlyoutXamlHost.Visibility = Visibility.Visible;
                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    VisualTreeHelper.SetRootDpi(_notificationFlyoutXamlHost, args.OldDpi);
                }), DispatcherPriority.ContextIdle, null);

                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    VisualTreeHelper.SetRootDpi(_notificationFlyoutXamlHost, args.NewDpi);
                }), DispatcherPriority.ContextIdle, null);

                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    _notificationFlyoutXamlHost.Visibility = Visibility.Hidden;
                    _isDpiChanging = false;
                }), DispatcherPriority.ContextIdle, null);
            });

            UpdateIcons();
        }

        private void OnTaskbarChanged(object sender, EventArgs args) => UpdateFlyoutPlacement();

        private void OnThemeChanged(object sender, SystemPersonalisationChangedEventArgs args)
        {
            UpdateIcons();
            UpdateTheme();
        }

        private void PrepareFlyout()
        {
            if (Flyout == null) return;

            Flyout.IconSourcePropertyChanged += OnFlyoutIconSourcePropertyChanged;
            Flyout.PlacementPropertyChanged += OnFlyoutPlacementPropertyChanged;
            Flyout.InteractedWith += OnFlyoutInteractedWith;

            var content = _notificationFlyoutXamlHost.GetHostContent();
            if (content != null)
            {
                content.Content = Flyout;
            }

            UpdateIcons();
        }

        private void PrepareFlyoutHost()
        {
            _notificationFlyoutXamlHost = new TransparentXamlHost<ContentControl>();

            _notificationFlyoutXamlHost.DpiChanged += OnNotificationFlyoutXamlHostDpiChanged;
            _notificationFlyoutXamlHost.Closed += OnNotificationFlyoutXamlHostClosed;
            _notificationFlyoutXamlHost.Deactivated += OnNotificationFlyoutXamlHostDeactivated;

            var taskbarState = _taskbarHelper.GetCurrentState();
            _notificationFlyoutXamlHost.Left = taskbarState.Screen.WorkingArea.Left;
            _notificationFlyoutXamlHost.Top = taskbarState.Screen.WorkingArea.Top;

            _notificationFlyoutXamlHost.Show();
        }

        private void ShowContextMenu()
        {
            var dpiX = _notificationFlyoutXamlHost.DpiX();
            var dpiY = _notificationFlyoutXamlHost.DpiY();

            PInvoke.GetPhysicalCursorPos(out POINT point);
            Flyout.ShowContextMenuAt(point.x / dpiX, point.y / dpiY);
        }

        private void UpdateFlyoutPlacement()
        {
            if (Flyout == null) return;

            var dpiX = _notificationFlyoutXamlHost.DpiX();
            var dpiY = _notificationFlyoutXamlHost.DpiY();

            var taskbarState = _taskbarHelper.GetCurrentState();
            _notificationFlyoutXamlHost.Left = taskbarState.Screen.Bounds.Left / dpiX; ;
            _notificationFlyoutXamlHost.Top = 0;

            double horizontalOffset;
            double verticalOffset;

            var workingAreaHeight = taskbarState.Screen.WorkingArea.Height / dpiX;
            var workingAreaWidth = taskbarState.Screen.WorkingArea.Width / dpiY;

            NotificationFlyoutTaskbarPlacement flyoutTaskBarPlacement;
            switch (taskbarState.Placement)
            {
                case TaskbarPlacement.Left:
                    flyoutTaskBarPlacement = NotificationFlyoutTaskbarPlacement.Left;
                    verticalOffset = taskbarState.Screen.WorkingArea.Height / dpiX;
                    horizontalOffset = taskbarState.Rect.Width / dpiY;
                    break;

                case TaskbarPlacement.Top:
                    flyoutTaskBarPlacement = NotificationFlyoutTaskbarPlacement.Top;
                    verticalOffset = taskbarState.Rect.Height / dpiX;
                    horizontalOffset = (_notificationFlyoutXamlHost.FlowDirection == FlowDirection.RightToLeft ? 0 : taskbarState.Screen.WorkingArea.Width) / dpiY;
                    break;

                case TaskbarPlacement.Right:
                    flyoutTaskBarPlacement = NotificationFlyoutTaskbarPlacement.Right;
                    verticalOffset = taskbarState.Screen.WorkingArea.Height / dpiX;
                    horizontalOffset = taskbarState.Screen.WorkingArea.Width / dpiY;
                    break;

                case TaskbarPlacement.Bottom:
                    flyoutTaskBarPlacement = NotificationFlyoutTaskbarPlacement.Bottom;
                    verticalOffset = taskbarState.Screen.WorkingArea.Height / dpiX;
                    horizontalOffset = (_notificationFlyoutXamlHost.FlowDirection == FlowDirection.RightToLeft ? 0 : taskbarState.Screen.WorkingArea.Width) / dpiY;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            Flyout.SetPlacement(horizontalOffset, verticalOffset, workingAreaHeight, workingAreaWidth, flyoutTaskBarPlacement);
        }

        private async void UpdateIcons()
        {
            if (Flyout == null) return;

            var shellTrayHandle = WindowHelper.GetHandle(ShellTrayHandleName);
            if (shellTrayHandle == null) return;

            var dpi = WindowHelper.GetDpi(shellTrayHandle);

            var desiredIconSource = _systemPersonalisationHelper.Theme == SystemTheme.Dark ? Flyout.IconSource : Flyout.LightIconSource;
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
            if (Flyout == null) return;
            Flyout.UpdateTheme(_systemPersonalisationHelper.IsColorPrevalence);
        }
    }
}