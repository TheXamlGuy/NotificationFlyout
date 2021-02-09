using System;

namespace NotificationFlyout.Sample.Wpf
{
    public class Program
    {
        [STAThread()]
        public static void Main()
        {
            using (new XamlHost.App())
            {
                var app = new App();
                new SampleNotificationFlyout();
                app.Run();
            }
        }
    }
}
