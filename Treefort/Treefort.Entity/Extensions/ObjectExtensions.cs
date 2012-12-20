using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Treefort.Common.Extensions;

namespace Treefort.Entity.Extensions
{
    public static class ObjectExtensions
    {
        public static void ForceUtc<T>(this T item)
        {
            typeof(T).GetProperties()
                .Where(p => p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?))
                .ForEach(p => p.SetValue(item, p.GetDateValue(item)));
        }

        public static T Cast<T>(this object item) 
        {
            return ((T) item);
        }
        
    }
}
