using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Treefort.Entity.Extensions
{
    public static class ReflectionExtensions
    {
        public static object GetDateValue(this PropertyInfo p, object item)
        {
            var dt = p.GetValue(item) as DateTime?;
            if (!dt.HasValue)
                return null;

            var value = dt.GetValueOrDefault();

            if (value.Kind == DateTimeKind.Local)
                return value.ToUniversalTime();

            if (value.Kind == DateTimeKind.Unspecified)
                throw new InvalidOperationException(string.Format("{0} is of Unspecified kind", p.Name));

            return value;
        }
    }
}
