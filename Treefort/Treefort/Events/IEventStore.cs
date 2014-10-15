using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Treefort.Events
{
    public interface IEventStore
    {
        Task<IEventStream> LoadEventStreamAsync(string streamName, int version = int.MaxValue);

        Task AppendAsync(string streamName, int version, IEnumerable<IEvent> events);
        
    }
}