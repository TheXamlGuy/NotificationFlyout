using System;
using System.Threading.Tasks;
using Windows.Media.Control;

namespace NotificationFlyoutSample
{
    public sealed partial class Shell
    {
        public Shell()
        {
            InitializeComponent();
        }


        private void MenuFlyoutItem_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var app = GetApplication();
            app.Exit();
        }
    }
}