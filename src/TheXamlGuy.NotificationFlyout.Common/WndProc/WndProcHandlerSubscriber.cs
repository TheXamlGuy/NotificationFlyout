using System;
using System.Linq;

namespace TheXamlGuy.NotificationFlyout.Common.Helpers
{
    public class WndProcHandlerSubscriber : IWndProcHandlerSubscriber
    {
        private static readonly Lazy<WndProcHandlerSubscriber> _current = new(() => new WndProcHandlerSubscriber());
        public static WndProcHandlerSubscriber Current => _current.Value;

        public void Subscribe<TWndProcHandler>(TWndProcHandler handler) where TWndProcHandler : IWndProcHandler
        {
            var handlers = WndProcHandlerCollection.Current;
            lock (handlers)
            {
                if (handlers.Any(x => x.Matches(handler))) return;
                handlers.Add(new WndProcHandlerReference(handler));
            }
        }
    }
}