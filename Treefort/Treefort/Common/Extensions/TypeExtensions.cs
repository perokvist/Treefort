using System;

namespace Treefort.Common.Extensions
{
    public static class TypeExtansions
    {
        public static Type MakeGenericType(this Type type, Type openGeneric, Func<Type[], Type> targetParamAccessor)
        {
            return openGeneric.MakeGenericType(targetParamAccessor(type.GetGenericArguments()));
        }
    }
}
