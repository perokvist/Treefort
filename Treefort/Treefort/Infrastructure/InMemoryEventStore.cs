using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Treefort.Events;

namespace Treefort.Infrastructure
{
    public class InMemoryEventStore : IEventStore
    {
        private readonly Func<IEventStream> _eventStreamFactory;
        private readonly IDictionary<Guid, IEventStream> _streams;

        public InMemoryEventStore(Func<IEventStream> eventStreamFactory)
        {
            _eventStreamFactory = eventStreamFactory;
            _streams = new ConcurrentDictionary<Guid, IEventStream>();
        }

        public async Task StoreAsync(System.Guid entityId, long version, System.Collections.Generic.IEnumerable<IEvent> events)
        {
            var eventStream = _streams.ContainsKey(entityId) ? _streams[entityId] : _eventStreamFactory();
            eventStream.Version = eventStream.Version + 1;
            eventStream.AddRange(events);
            _streams[entityId] = eventStream;
        }

        public async Task<IEventStream> LoadEventStreamAsync(System.Guid entityId)
        {
            return _streams.ContainsKey(entityId) ? _streams[entityId] : _eventStreamFactory();
        }
    }
}