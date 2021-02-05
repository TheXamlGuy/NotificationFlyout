using NotificationFlyout.Tray.Views;
using System;

namespace NotificationFlyout.Tray
{
    public class Program
    {
        [STAThread()]
        public static void Main()
        {
            using (new XamlHost.App())
            {
                var app = new App();
                new Shell();
                app.Run();
            }
        }
    }
}