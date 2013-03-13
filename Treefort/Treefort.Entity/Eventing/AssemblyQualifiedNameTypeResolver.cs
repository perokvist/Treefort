using System;
using Treefort.Events;

namespace Treefort.EntityFramework.Eventing
{
    public class AssemblyQualifiedNameTypeResolver : IEventTypeResolver
    {
        public System.Type Resolve(string eventType)
        {
            return Type.GetType(eventType);
        }

        public string AsString(System.Type type)
        {
            return type.AssemblyQualifiedName;
        }
    }
}