using Microsoft.Win32;
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
        private readonly string PersonalizeKey = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        private SystemTheme _currentTheme;
        private bool _isColorPrevalence;

        private SystemPersonalisationHelper(Window window)
        {
            var source = HwndSource.FromHwnd(window.GetHandle());
            source.AddHook(new HwndSourceHook(WndProc));

            _currentTheme = GetTheme();
            _isColorPrevalence = GetIsColorPrevalence();
        }

        public event EventHandler<SystemPersonalisationChangedEventArgs> ThemeChanged;

        public bool IsColorPrevalence => GetIsColorPrevalence();
        public SystemTheme Theme => GetTheme();

        public static SystemPersonalisationHelper Create(Window window)
        {
            return new SystemPersonalisationHelper(window);
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr DefWindowProcW(IntPtr handle, uint msg, IntPtr wParam, IntPtr lParam);

        private bool GetIsColorPrevalence()
        {
            using var baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
            using var subKey = baseKey.OpenSubKey(PersonalizeKey);
            return subKey.GetValue<int>("ColorPrevalence", 0) > 0;
        }

        private SystemTheme GetTheme()
        {
            var color = _settings.GetColorValue(UIColorType.Background).ToString();
            return color == "#FFFFFFFF" ? SystemTheme.Light : SystemTheme.Dark;
        }

        private void RaiseThemeChangedEvent()
        {
            var theme = GetTheme();
            var isColorPrevalence = GetIsColorPrevalence();

            if (theme != _currentTheme || _isColorPrevalence != isColorPrevalence)
            {
                _currentTheme = theme;
                _isColorPrevalence = isColorPrevalence;

                ThemeChanged?.Invoke(this, new SystemPersonalisationChangedEventArgs(theme, isColorPrevalence));
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