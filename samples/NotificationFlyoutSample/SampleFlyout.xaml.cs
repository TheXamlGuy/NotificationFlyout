using TheXamlGuy.NotificationFlyout.Uwp.UI.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch.IsOn)
            {
                this.IsLightDismissEnabled = true;
            }
            else
            {
                this.IsLightDismissEnabled = false;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            
            if (comboBox.SelectedIndex == 0)
            {
                this.Placement = NotificationFlyoutPlacement.Auto;
            }
            else
            {
                this.Placement = NotificationFlyoutPlacement.FullRight;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var app = GetApplication();
            app.OpenAsWindow<Test>();
        }
    }
}