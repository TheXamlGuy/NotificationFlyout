using NotificationFlyout.Wpf.UI.Extensions;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Windows.UI.ViewManagement;

namespace NotificationFlyout.Wpf.UI.Helpers
{
    public class SystemPersonalisationHelper
    {
        private readonly UISettings _settings = new();

        private SystemTheme _currentTheme;


        private SystemPersonalisationHelper(Window window)
        {
            var source = HwndSource.FromHwnd(window.GetHandle());
            source.AddHook(new HwndSourceHook(WndProc));

            _settings.ColorValuesChanged += OnColorValuesChanged;
            _currentTheme = GetSystemTheme();
        }

        public event EventHandler<ThemeChangedEventArgs> ThemeChanged;

        public SystemTheme SystemTheme => GetSystemTheme();

        public static SystemPersonalisationHelper Create(Window window)
        {
            return new SystemPersonalisationHelper(window);
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr DefWindowProcW(IntPtr handle, uint msg, IntPtr wParam, IntPtr lParam);

        private SystemTheme GetSystemTheme()
        {
            var uiTheme = _settings.GetColorValue(UIColorType.Background).ToString();
            return uiTheme == "#FFFFFFFF" ? SystemTheme.Light : SystemTheme.Dark;
        }

        private void OnColorValuesChanged(UISettings sender, object args)
        {
            RaiseThemeChangedEvent();
        }

        private void RaiseThemeChangedEvent()
        {
            var theme = GetSystemTheme();
            if (theme != _currentTheme)
            {
                ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(theme));
                _currentTheme = theme;
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == (int)WndProcMessages.WM_SETTINGCHANGE)
            {
                RaiseThemeChangedEvent();
            }

            return DefWindowProcW(hwnd, (uint)msg, wParam, (lParam));
        }
    }
}
