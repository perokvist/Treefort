using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treefort.Common
{
    public static class TypeExtansions
    {
        public static Type MakeGenericType(this Type type, Type openGeneric, Func<Type[], Type> targetParamAccessor)
        {
            return openGeneric.MakeGenericType(targetParamAccessor(type.GetGenericArguments()));
        }
    }
}
