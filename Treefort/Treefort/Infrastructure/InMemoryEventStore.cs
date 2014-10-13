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
        private readonly IDictionary<string, IEventStream> _streams;

        public InMemoryEventStore(Func<IEventStream> eventStreamFactory)
        {
            _eventStreamFactory = eventStreamFactory;
            _streams = new ConcurrentDictionary<string, IEventStream>();
        }

        public Task AppendAsync(System.Guid entityId, long version, System.Collections.Generic.IEnumerable<IEvent> events)
        {
            return AppendAsync(entityId.ToString(), (int)version, events);
        }

        public Task<IEventStream> LoadEventStreamAsync(System.Guid entityId)
        {
            return LoadEventStreamAsync(entityId.ToString());
        }

        public async Task<IEventStream> LoadEventStreamAsync(string streamName)
        {
            return _streams.ContainsKey(streamName) ? _streams[streamName] : _eventStreamFactory();
        }

        public async Task AppendAsync(string streamName, int version, IEnumerable<IEvent> events)
        {
            var eventStream = _streams.ContainsKey(streamName) ? _streams[streamName] : _eventStreamFactory();
            if (version != eventStream.Version)
                throw new InvalidOperationException("EventStream version is old");

            eventStream.Version = eventStream.Version + 1;
            eventStream.AddRange(events);
            _streams[streamName] = eventStream; 
        }
    }
}