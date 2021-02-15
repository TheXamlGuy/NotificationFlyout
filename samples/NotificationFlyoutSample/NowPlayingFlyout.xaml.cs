using Windows.UI.Xaml;

namespace NotificationFlyoutSample
{
    public sealed partial class NowPlayingFlyout
    {
        public NowPlayingFlyout()
        {
            InitializeComponent();
            Opened += OnOpened;
        }

        private void OnOpened(object sender, object args)
        {
            RootFrame.Navigate(typeof(NowPlayingPage));
        }

        private void OnCloseMenuFlyoutItemClick(object sender, RoutedEventArgs args)
        {
            var app = GetApplication();
            app.Exit();
        }
    }
}