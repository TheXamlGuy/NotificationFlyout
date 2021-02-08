namespace NotificationFlyout.Sample
{
    public sealed partial class NotificationFlyoutPresenter
    {
        public NotificationFlyoutPresenter()
        {
            InitializeComponent();
        }

        private void test_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            if (test.SelectedIndex == 0)
            {
                this.RequestedTheme = Windows.UI.Xaml.ElementTheme.Default;

            }

            if (test.SelectedIndex == 1)
            {
                this.RequestedTheme = Windows.UI.Xaml.ElementTheme.Dark;

            }

            if (test.SelectedIndex == 2)
            {
                this.RequestedTheme = Windows.UI.Xaml.ElementTheme.Light;

            }
        }
    }
}
