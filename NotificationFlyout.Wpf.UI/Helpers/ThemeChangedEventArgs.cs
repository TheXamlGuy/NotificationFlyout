using System;

namespace NotificationFlyout.Wpf.UI.Helpers
{
    public class ThemeChangedEventArgs : EventArgs
    {
        internal ThemeChangedEventArgs(SystemTheme theme)
        {
            Theme = theme;
        }

        public SystemTheme Theme { get; private set; }
    }
}
