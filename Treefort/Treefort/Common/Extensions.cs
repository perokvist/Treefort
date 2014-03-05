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

        public static void CastAction<T>(this object self, Action<T> action)
            where T : class
        {
            var cast = self as T;
            if (cast != null)
                action(cast);
        }
    }
}