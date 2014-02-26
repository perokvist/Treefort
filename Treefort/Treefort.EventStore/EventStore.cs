using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Treefort.Events;
using Treefort.Infrastructure;

namespace Treefort.EventStore
{
    public class EventStore : IEventStore
    {
        private readonly IEventStoreConnection _storeConnection;
        private readonly IJsonConverter _jsonConverter;

        public EventStore(IEventStoreConnection storeConnection, IJsonConverter jsonConverter)
        {
            _storeConnection = storeConnection;
            _jsonConverter = jsonConverter;
        }

        public async Task StoreAsync(Guid entityId, long version, IEnumerable<IEvent> events)
        {
            var data = events.Select(e => e.ToEventData());
            await _storeConnection.AppendToStreamAsync(entityId.ToString(), (int)version, data);
        }

        public async Task<IEventStream> LoadEventStreamAsync(Guid entityId)
        {
            var events = await _storeConnection.ReadAllEventsFromStream(entityId.ToString());
            return new EventStream(events.Select(e => e.ToEvent(_jsonConverter)), events.Any() ? events.Last().OriginalEventNumber : -1);
        }
    }
}