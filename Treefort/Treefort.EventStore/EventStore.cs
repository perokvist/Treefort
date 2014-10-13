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
        private readonly Func<IEvent, EventData> _eventDataProvider;
        private readonly Func<ResolvedEvent, IJsonConverter, IEvent> _eventConverter;

        public EventStore(IEventStoreConnection storeConnection, IJsonConverter jsonConverter) 
            :this(storeConnection, jsonConverter, @event => @event.ToEventData(),(data, converter) => data.ToEvent(converter))
        {
            
        }

        public EventStore(IEventStoreConnection storeConnection, IJsonConverter jsonConverter, 
            Func<IEvent, EventData> eventDataProvider, Func<ResolvedEvent,IJsonConverter,IEvent> eventConverter)
        {
            _storeConnection = storeConnection;
            _jsonConverter = jsonConverter;
            _eventDataProvider = eventDataProvider;
            _eventConverter = eventConverter;
        }

        public async Task AppendAsync(string streamName, int version, IEnumerable<IEvent> events)
        {
            var data = events.Select(e => _eventDataProvider(e));
            await _storeConnection.AppendToStreamAsync(streamName, version, data);
        }

        public async Task<IEventStream> LoadEventStreamAsync(string streamName)
        {
            var events = await _storeConnection.ReadAllEventsFromStream(streamName);
            return new EventStream(events.Select(e => _eventConverter(e, _jsonConverter)), events.Any() ? events.Last().OriginalEventNumber : -1);
        }
    }
}