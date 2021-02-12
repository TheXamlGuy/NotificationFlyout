using Windows.UI.Xaml;

namespace NotificationFlyout.Uwp.UI
{
    public interface INotificationFlyoutApplication
    {
        void Exit();

        void OpenAsWindow<TUIElement>() where TUIElement : UIElement;
    }
}