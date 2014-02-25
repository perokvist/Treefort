using System;
using Treefort.Events;

namespace Treefort.EntityFramework.Eventing
{
    public class AssemblyQualifiedNameTypeResolver : IEventTypeResolver
    {
        public Type Resolve(string eventType)
        {
            return Type.GetType(eventType);
        }

        public string AsString(Type type)
        {
            return type.AssemblyQualifiedName;
        }
    }
}