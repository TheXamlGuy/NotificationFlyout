using NotificationFlyout.Wpf.UI.Controls;
using System;

namespace NotificationFlyoutSample.Host
{
    public class Program
    {
        [STAThread()]
        public static void Main()
        {
            using (new NotificationFlyoutSample.App())
            {
                var app = new App();
                new NotificationFlyoutApplication
                {
                    Flyout = new Shell()
                };
                app.Run();
            }
        }
    }
}