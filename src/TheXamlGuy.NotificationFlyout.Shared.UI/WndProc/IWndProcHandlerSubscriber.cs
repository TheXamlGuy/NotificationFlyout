namespace NotificationFlyout.Shared.UI.Helpers
{
    public interface IWndProcHandlerSubscriber
    {
        void Subscribe<TWndProcHandler>(TWndProcHandler handler) where TWndProcHandler : IWndProcHandler;
    }
}