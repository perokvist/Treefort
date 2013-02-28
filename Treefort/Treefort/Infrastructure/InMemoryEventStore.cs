using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Treefort.Events;

namespace Treefort.Infrastructure
{
    public class InMemoryEventStore : IEventStore
    {
        private readonly IDictionary<Guid, IEventStream> _streams;

        public InMemoryEventStore()
        {
            _streams = new ConcurrentDictionary<Guid, IEventStream>();
        }

        public async Task StoreAsync(System.Guid entityId, long version, System.Collections.Generic.IEnumerable<IEvent> events)
        {
            var eventStream = _streams.ContainsKey(entityId) ? _streams[entityId] : new InMemoryEventStream();
            eventStream.Version = eventStream.Version + 1;
            eventStream.AddRange(events);
            _streams[entityId] = eventStream;
        }

        public IEventStream LoadEventStream(System.Guid entityId)
        {
            return _streams.ContainsKey(entityId) ? _streams[entityId] : new InMemoryEventStream();
        }
    }
}