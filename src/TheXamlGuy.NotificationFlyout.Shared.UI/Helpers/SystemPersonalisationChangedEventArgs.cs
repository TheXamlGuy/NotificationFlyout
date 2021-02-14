using System;

namespace TheXamlGuy.NotificationFlyout.Shared.UI.Helpers
{
    public class SystemPersonalisationChangedEventArgs : EventArgs
    {
        internal SystemPersonalisationChangedEventArgs(SystemTheme theme, bool isColorPrevalence)
        {
            Theme = theme;
            IsColorPrevalence = isColorPrevalence;
        }

        public SystemTheme Theme { get; private set; }

        public bool IsColorPrevalence { get; private set; }
    }
}