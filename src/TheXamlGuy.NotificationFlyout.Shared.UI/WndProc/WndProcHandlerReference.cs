using System;

namespace TheXamlGuy.NotificationFlyout.Shared.UI.Helpers
{
    internal class WndProcHandlerReference
    {
        private readonly WeakReference _reference;

        public WndProcHandlerReference(object handler) => _reference = new WeakReference(handler);

        public bool IsDead => _reference.Target == null;

        public void Handle(uint message, IntPtr wParam, IntPtr lParam)
        {
            if (_reference.Target == null) return;

            var target = _reference.Target;
            if (target is IWndProcHandler handler)
            {
                handler.Handle(message, wParam, lParam);
            }
        }

        public bool Matches(object instance) => _reference.Target == instance;
    }
}