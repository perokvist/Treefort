using System;
using System.Collections.Generic;

namespace Treefort.Infrastructure
{
    public static class Extensions
    {
        public static T Tap<T>(this T self, Action<T> action)
        {
            action(self);
            return self;
        }
    }
}