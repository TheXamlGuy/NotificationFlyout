using Windows.UI.Xaml;

namespace TheXamlGuy.NotificationFlyout.Shared.UI
{
    public interface INotificationFlyoutApplication
    {
        void Exit();

        void OpenAsWindow<TUIElement>() where TUIElement : UIElement;
    }
}