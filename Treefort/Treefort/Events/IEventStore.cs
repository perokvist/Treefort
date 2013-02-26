using System;
using System.Collections.Generic;

namespace Treefort.Events
{
    public interface IEventStore
    {
        void Store(Guid entityId, long version, IEnumerable<IEvent> events);

        IEventStream LoadEventStream(Guid entityId);
    }
}