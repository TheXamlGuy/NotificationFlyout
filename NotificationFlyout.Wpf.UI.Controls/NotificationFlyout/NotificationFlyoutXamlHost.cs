using Microsoft.Toolkit.Wpf.UI.XamlHost;
using NotificationFlyout.Uwp.UI.Controls;
using NotificationFlyout.Wpf.UI.Extensions;
using NotificationFlyout.Wpf.UI.Helpers;
using System;
using System.Windows;
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
        private bool _taskbarChanged;
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

        internal void ShowFlyout()
        {
            var flyoutContentControl = GetNotificationFlyoutPresenter();
            if (flyoutContentControl != null)
            {
                var taskbarState = _taskbarHelper.GetCurrentState();

                var flyoutPlacement = FlyoutPlacementMode.Bottom;
                switch (taskbarState.Position)
                {
                    case TaskbarPosition.Left:
                        flyoutPlacement = FlyoutPlacementMode.Right;
                        break;
                    case TaskbarPosition.Top:
                        flyoutPlacement = FlyoutPlacementMode.Bottom;
                        break;
                    case TaskbarPosition.Right:
                        flyoutPlacement = FlyoutPlacementMode.Left;
                        break;
                    case TaskbarPosition.Bottom:
                        flyoutPlacement = FlyoutPlacementMode.Top;
                        break;
                }

                UpdateWindow();
                Activate();

                flyoutContentControl.ShowFlyout(flyoutPlacement);
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
            var flyoutContentControl = GetNotificationFlyoutPresenter();
            if (flyoutContentControl != null)
            {
                flyoutContentControl.Content = FlyoutContent;
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
            _taskbarChanged = true;

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
            Height = 0;
            Width = 0;
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
            var taskbarState = _taskbarHelper.GetCurrentState();

            var screen = Screen.FromHandle(this.GetHandle());
            MaxHeight = screen.Bounds.Height / 2;

            var windowWidth = DesiredSize.Width * this.DpiX();
            var windowHeight = DesiredSize.Height * this.DpiY();

            var taskbarRect = taskbarState.Rect;
            switch (taskbarState.Position)
            {
                case TaskbarPosition.Left:
                    this.SetWindowPosition(taskbarRect.Bottom - windowHeight, taskbarRect.Right, windowHeight, windowWidth);
                    break;
                case TaskbarPosition.Top:
                    this.SetWindowPosition(taskbarRect.Bottom, FlowDirection == FlowDirection.RightToLeft ? taskbarRect.Left : taskbarRect.Right - windowWidth, windowHeight, windowWidth);
                    break;
                case TaskbarPosition.Right:
                    this.SetWindowPosition(taskbarRect.Bottom - windowHeight, taskbarRect.Left - windowWidth, windowHeight, windowWidth);
                    break;
                case TaskbarPosition.Bottom:
                    this.SetWindowPosition(taskbarRect.Top - windowHeight, FlowDirection == FlowDirection.RightToLeft ? taskbarRect.Left : taskbarRect.Right - windowWidth, windowHeight, windowWidth);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}