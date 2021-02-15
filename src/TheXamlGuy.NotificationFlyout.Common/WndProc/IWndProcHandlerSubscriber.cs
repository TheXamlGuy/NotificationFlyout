
namespace TheXamlGuy.NotificationFlyout.Common.Helpers
{
    public interface IWndProcHandlerSubscriber
    {
        void Subscribe<TWndProcHandler>(TWndProcHandler handler) where TWndProcHandler : IWndProcHandler;
    }
}