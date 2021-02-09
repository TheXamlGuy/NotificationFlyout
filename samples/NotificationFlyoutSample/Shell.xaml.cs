namespace NotificationFlyoutSample
{
    public sealed partial class Shell
    {
        public Shell()
        {
            InitializeComponent();
        }

        private void Theme_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            switch (Theme.SelectedIndex)
            {
                case 0:
                    RequestedTheme = Windows.UI.Xaml.ElementTheme.Default;
                    break;
                case 1:
                    RequestedTheme = Windows.UI.Xaml.ElementTheme.Dark;
                    break;
                case 2:
                    RequestedTheme = Windows.UI.Xaml.ElementTheme.Light;
                    break;
            }
        }
    }
}
