using System;
using TheXamlGuy.NotificationFlyout.Wpf.UI.Controls;

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
                    Flyout = new SampleFlyout()
                };

                app.Run();
            }
        }
    }
}