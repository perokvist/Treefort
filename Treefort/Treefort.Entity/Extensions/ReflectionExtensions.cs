using System;
using System.Reflection;

namespace Treefort.EntityFramework.Extensions
{
    public static class ReflectionExtensions
    {
        public static object GetDateValue(this PropertyInfo p, object item)
        {
            var dt = item.Cast<DateTime?>();
            if (dt == null)
                return null;
            
            var value = dt.Value;

            if (value.Kind == DateTimeKind.Local)
                return value.ToUniversalTime();

            if (value.Kind == DateTimeKind.Unspecified)
                return DateTime.SpecifyKind(value, DateTimeKind.Utc);

            return value;
        }
    }
}
