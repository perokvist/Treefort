using System;

namespace Treefort.Common
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