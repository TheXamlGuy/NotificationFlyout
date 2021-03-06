﻿using System;
using System.Collections.Generic;

namespace TheXamlGuy.NotificationFlyout.Common.Helpers
{
    internal class WndProcHandlerCollection : List<WndProcHandlerReference>, IWndProcHandlerCollection
    {
        private static readonly Lazy<WndProcHandlerCollection> _current = new(() => new WndProcHandlerCollection());
        public static WndProcHandlerCollection Current => _current.Value;
    }
}