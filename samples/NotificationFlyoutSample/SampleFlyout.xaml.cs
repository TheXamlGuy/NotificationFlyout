using Windows.UI.Xaml;

namespace NotificationFlyoutSample
{
    public sealed partial class SampleFlyout
    {
        public SampleFlyout()
        {
            InitializeComponent();
        }

        private void OnCloseMenuFlyoutItemClick(object sender, RoutedEventArgs args)
        {
            var app = GetApplication();
            app.Exit();
        }
    }
}