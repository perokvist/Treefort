using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Treefort.Events
{
    public interface IEventStore
    {
        Task StoreAsync(Guid entityId, long version, IEnumerable<IEvent> events);

        Task<IEventStream> LoadEventStreamAsync(Guid entityId);
    }
}