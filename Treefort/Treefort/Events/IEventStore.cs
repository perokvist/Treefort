using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Treefort.Events
{
    public interface IEventStore
    {
        //Task AppendAsync(Guid entityId, long version, IEnumerable<IEvent> events);

        //Task<IEventStream> LoadEventStreamAsync(Guid streamId);

        Task<IEventStream> LoadEventStreamAsync(string streamName);

        Task AppendAsync(string streamName, int version, IEnumerable<IEvent> events);
        
    }
}