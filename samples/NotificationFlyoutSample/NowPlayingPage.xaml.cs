using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace NotificationFlyoutSample
{
    public sealed partial class NowPlayingPage : Page
    {
        public NowPlayingPage()
        {
            InitializeComponent();

            ViewModel = new NowPlayingPageViewModel();
            DataContext = ViewModel;
        }
 
        public NowPlayingPageViewModel ViewModel { get; }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            if (DataContext is INavigation navigation)
            {
                navigation.OnNavigatedTo();
            }

            base.OnNavigatedTo(args);
        }
    }
}
