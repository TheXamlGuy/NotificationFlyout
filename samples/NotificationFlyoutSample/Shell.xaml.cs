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

        private async void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //var gsmtcsm = await GetSystemMediaTransportControlsSessionManager();
            //var mediaProperties = await GetMediaProperties(gsmtcsm.GetCurrentSession());

        }

        //private static async Task<GlobalSystemMediaTransportControlsSessionManager> GetSystemMediaTransportControlsSessionManager() =>
        //await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();

        //private static async Task<GlobalSystemMediaTransportControlsSessionMediaProperties> GetMediaProperties(GlobalSystemMediaTransportControlsSession session) =>
        //    await session.TryGetMediaPropertiesAsync();
    }
}