using Microsoft.Toolkit.Wpf.UI.XamlHost;
using NotificationFlyout.Uwp.UI.Controls;
using NotificationFlyout.Wpf.UI.Extensions;
using NotificationFlyout.Wpf.UI.Helpers;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using Windows.UI.Xaml.Controls.Primitives;

namespace NotificationFlyout.Wpf.UI.Controls
{
    internal class NotificationFlyoutXamlHost : Window
    {
        internal static DependencyProperty FlyoutContentProperty =
             DependencyProperty.Register(nameof(FlyoutContent),
                 typeof(Windows.UI.Xaml.UIElement), typeof(NotificationFlyoutXamlHost),
                 new PropertyMetadata(null, OnFlyoutContentPropertyChanged));

        private WindowsXamlHost _host;

        private NotificationIconHelper _notificationIconHelper;
        private TaskbarHelper _taskbarHelper;

        public NotificationFlyoutXamlHost()
        {
            PrepareDefaultWindow();
            PrepareWindowsXamlHost();

            Loaded += OnLoaded;
        }

        public Windows.UI.Xaml.UIElement FlyoutContent
        {
            get => (Windows.UI.Xaml.UIElement)GetValue(FlyoutContentProperty);
            set => SetValue(FlyoutContentProperty, value);
        }

        internal void HideFlyout()
        {
            var flyoutContentControl = GetNotificationFlyoutPresenter();
            if (flyoutContentControl != null)
            {
                flyoutContentControl.HideFlyout();
            }
        }

        internal void SetNotificationIcon(IntPtr handle)
        {
            _notificationIconHelper.SetIcon(handle);
        }

        private const double MaximumOffset = 80;

        internal void ShowFlyout()
        {
            var flyoutPresenter = GetNotificationFlyoutPresenter();
            if (flyoutPresenter != null)
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
                flyoutPresenter.ShowFlyout(flyoutPlacement);
            }
        }

        private static void OnFlyoutContentPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var sender = dependencyObject as NotificationFlyoutXamlHost;
            sender?.OnFlyoutContentPropertyChanged();
        }

        private NotificationFlyoutPresenter GetNotificationFlyoutPresenter()
        {
            return _host.GetUwpInternalObject() as NotificationFlyoutPresenter;
        }

        private void OnFlyoutContentPropertyChanged()
        {
            var flyoutPresenter = GetNotificationFlyoutPresenter();
            if (flyoutPresenter != null)
            {
                flyoutPresenter.Content = FlyoutContent;
            }
        }

        private void OnIconInvoked(object sender, NotificationIconInvokedEventArgs args)
        {
            ShowFlyout();
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            PrepareNotificationIcon();
            PrepareTaskbar();

            UpdateWindow();
        }

        private void OnTaskbarChanged(object sender, EventArgs args)
        {
            var taskbarState = _taskbarHelper.GetCurrentState();
            Left = taskbarState.Screen.WorkingArea.Left;
            Top = taskbarState.Screen.WorkingArea.Top;

            UpdateWindow();
        }

        private void PrepareDefaultWindow()
        {
            ShowInTaskbar = false;
            ShowActivated = false;
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            AllowsTransparency = true;
            Background = new SolidColorBrush(Colors.Transparent);
            Height = 5;
            Width = 5;
        }

        private void PrepareNotificationIcon()
        {
            _notificationIconHelper = NotificationIconHelper.Create(this);
            _notificationIconHelper.IconInvoked += OnIconInvoked;
        }

        private void PrepareTaskbar()
        {
            _taskbarHelper = TaskbarHelper.Create(this);
            _taskbarHelper.TaskbarChanged += OnTaskbarChanged;
        }

        private void PrepareWindowsXamlHost()
        {
            _host = new WindowsXamlHost
            {
                InitialTypeName = typeof(NotificationFlyoutPresenter).FullName
            };

            _host.Height = 0;
            _host.Width = 0;

            Content = _host;
        }

        private void UpdateWindow()
        {
            var flyoutPresenter = GetNotificationFlyoutPresenter();
            var taskbarState = _taskbarHelper.GetCurrentState();

            var screen = Screen.FromHandle(this.GetHandle());
            MaxHeight = screen.Bounds.Height / 2;

            var windowWidth = DesiredSize.Width * this.DpiX();
            var windowHeight = DesiredSize.Height * this.DpiY();

            double top;
            double left;
            double height;
            double width;
            double verticalOffset = 0;
            double horizontalOffset = 0;

            var taskbarRect = taskbarState.Rect;
            switch (taskbarState.Position)
            {
                case TaskbarPosition.Left:
                    top = taskbarRect.Bottom - windowHeight;
                    left = taskbarRect.Right;
                    height = windowHeight;
                    width = windowWidth;
                    horizontalOffset = -MaximumOffset;
                    break;
                case TaskbarPosition.Top:
                    top = taskbarRect.Bottom;
                    left = FlowDirection == FlowDirection.RightToLeft ? taskbarRect.Left : taskbarRect.Right - windowWidth;
                    height = windowHeight;
                    width = windowWidth;
                    verticalOffset = -MaximumOffset;
                    break;
                case TaskbarPosition.Right:
                    top = taskbarRect.Bottom - windowHeight;
                    left = taskbarRect.Left - windowWidth;
                    height = windowHeight;
                    width = windowWidth;
                    horizontalOffset = MaximumOffset;
                    break;
                case TaskbarPosition.Bottom:
                    top = taskbarRect.Top - windowHeight;
                    left = FlowDirection == FlowDirection.RightToLeft ? taskbarRect.Left : taskbarRect.Right - windowWidth;
                    height = windowHeight;
                    width = windowWidth;
                    verticalOffset = MaximumOffset;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            this.SetWindowPosition(top, left, height, width);
            flyoutPresenter.SetOffset(verticalOffset, horizontalOffset);
        }
    }
}