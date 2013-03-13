using System;

namespace Treefort.Events
{
    public interface IEventTypeResolver
    {
        Type Resolve(string eventType);
        string AsString(Type type);
    }
}