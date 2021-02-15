using System;
using System.Linq;

namespace TheXamlGuy.NotificationFlyout.Common.Helpers
{
    public class WndProcListener
    {
        private static readonly Lazy<WndProcListener> _current = new(() => new WndProcListener());

        private readonly WndProcHelper _wndProcHelper;

        private WndProcListener() => _wndProcHelper = WndProcHelper.Create();

        public static WndProcListener Current => _current.Value;

        public IntPtr Handle => _wndProcHelper.Handle;

        public void Start()
        {
            _wndProcHelper.WndProcMessage -= OnWndProcMessage;
            _wndProcHelper.WndProcMessage += OnWndProcMessage;
        }

        private void OnWndProcMessage(object sender, WndProcHelperMessageEventArgs args)
        {
            WndProcHandlerReference[] handlers;
            var subscribers = WndProcHandlerCollection.Current;

            lock (subscribers)
            {
                handlers = subscribers.ToArray();
            }

            foreach (var handler in handlers)
            {
                handler.Handle(args.Message, args.WParam, args.LParam);
            }

            var deadHandlers = handlers.Where(x => x.IsDead).ToList();
            if (deadHandlers.Count > 0)
            {
                lock (subscribers)
                {
                    foreach (var deadHandler in deadHandlers) subscribers.Remove(deadHandler);
                }
            }
        }
    }
}