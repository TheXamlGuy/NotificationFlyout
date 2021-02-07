namespace NotificationFlyout.Sample
{
    public sealed partial class NotificationFlyoutPresenter
    {
        public NotificationFlyoutPresenter()
        {
            InitializeComponent();
        }

        private void ToggleSwitch_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (test.IsOn)
            {
                this.RequestedTheme = Windows.UI.Xaml.ElementTheme.Dark;
            }
            else
            {
                this.RequestedTheme = Windows.UI.Xaml.ElementTheme.Light;

            }
        }
    }
}
