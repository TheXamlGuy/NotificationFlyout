using TheXamlGuy.NotificationFlyout.Common.Helpers;
using System.Windows;
using System.Windows.Markup;
using System;
using Windows.UI.Xaml.Controls;
using TheXamlGuy.NotificationFlyout.Uwp.UI.Controls;
using TheXamlGuy.NotificationFlyout.Wpf.UI.Extensions;
using System.Windows.Media.Imaging;
using TheXamlGuy.NotificationFlyout.Common.Extensions;

namespace TheXamlGuy.NotificationFlyout.Wpf.UI.Controls
{
    [ContentProperty(nameof(Flyout))]
    public class NotificationFlyoutApplication : DependencyObject
    {
        public static DependencyProperty FlyoutProperty =
            DependencyProperty.Register(nameof(Flyout),
                typeof(Uwp.UI.Controls.NotificationFlyout), typeof(NotificationFlyoutApplication),
                new PropertyMetadata(null, OnFlyoutPropertyChanged));

        private const string ShellTrayHandleName = "Shell_TrayWnd";
        private TransparentXamlHost<ContentControl> _notificationFlyoutXamlHost;
        private NotificationIconHelper _notificationIconHelper;
        private SystemPersonalisationHelper _systemPersonalisationHelper;
        private TaskbarHelper _taskbarHelper;

        public NotificationFlyoutApplication()
        {
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

        private static void OnFlyoutPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var sender = dependencyObject as NotificationFlyoutApplication;
            sender?.OnFlyoutPropertyChanged();
        }

        private void OnFlyoutPropertyChanged() => PrepareFlyout();

        private void PrepareFlyout()
        {
            if (Flyout == null) return;
            Flyout.IconSourceChanged += OnIconSourceChanged;

            var content = _notificationFlyoutXamlHost.GetHostContent();
            if (content != null)
            {
                content.Content = Flyout;
            }

            SetIcons();
            SetFlyoutPlacement();
        }

        private void OnIconInvoked(object sender, NotificationIconInvokedEventArgs args)
        {
            if (args.PointerButton == PointerButton.Left)
            {
                _notificationFlyoutXamlHost.Activate();
                Flyout.Show();
            }

            if (args.PointerButton == PointerButton.Right)
            {
            }
        }


        private void OnIconSourceChanged(object sender, EventArgs args) => SetIcons();

        private void OnNotificationFlyoutXamlHostClosed(object sender, EventArgs args)
        {
            _notificationIconHelper?.Dispose();
        }

        private void OnTaskbarChanged(object sender, EventArgs args) => SetFlyoutPlacement();

        private void OnThemeChanged(object sender, SystemPersonalisationChangedEventArgs args)
        {

        }

        private void PrepareFlyoutHost()
        {
            _notificationFlyoutXamlHost = new TransparentXamlHost<ContentControl>();
            _notificationFlyoutXamlHost.Closed += OnNotificationFlyoutXamlHostClosed;

            var taskbarState = _taskbarHelper.GetCurrentState();
            _notificationFlyoutXamlHost.Left = taskbarState.Screen.WorkingArea.Left;
            _notificationFlyoutXamlHost.Top = taskbarState.Screen.WorkingArea.Top;

            _notificationFlyoutXamlHost.Show();
        }

        private void SetFlyoutPlacement()
        {
            var taskbarState = _taskbarHelper.GetCurrentState();

            _notificationFlyoutXamlHost.Left = 0;
            _notificationFlyoutXamlHost.Top = 0;

            double left;
            double top;

            var dpiX = _notificationFlyoutXamlHost.DpiX();
            var dpiY = _notificationFlyoutXamlHost.DpiY();

            NotificationFlyoutTaskbarPlacement flyoutTaskBarPlacement;
            switch (taskbarState.Placement)
            {
                case TaskbarPlacement.Left:
                    flyoutTaskBarPlacement = NotificationFlyoutTaskbarPlacement.Left;
                    top = taskbarState.Rect.Bottom / dpiX;
                    left = taskbarState.Rect.Right / dpiY;
                    break;

                case TaskbarPlacement.Top:
                    flyoutTaskBarPlacement = NotificationFlyoutTaskbarPlacement.Top;
                    top = taskbarState.Rect.Bottom / dpiX;
                    left = (_notificationFlyoutXamlHost.FlowDirection == FlowDirection.RightToLeft ? taskbarState.Rect.Left : taskbarState.Rect.Right) / dpiY;
                    break;

                case TaskbarPlacement.Right:
                    flyoutTaskBarPlacement = NotificationFlyoutTaskbarPlacement.Right;
                    top = taskbarState.Rect.Bottom / dpiX;
                    left = taskbarState.Rect.Left / dpiY;
                    break;

                case TaskbarPlacement.Bottom:
                    flyoutTaskBarPlacement = NotificationFlyoutTaskbarPlacement.Bottom;
                    top = taskbarState.Rect.Top / dpiX;
                    left = (_notificationFlyoutXamlHost.FlowDirection == FlowDirection.RightToLeft ? taskbarState.Rect.Left : taskbarState.Rect.Right) / dpiY;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            Flyout.SetPlacement(left, top, flyoutTaskBarPlacement);
        }

        private async void SetIcons()
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
            //var content = GetHostContent();
            //if (content != null)
            //{
            //    content.UpdateTheme(_systemPersonalisationHelper.IsColorPrevalence);
            //}
        }
    }
}