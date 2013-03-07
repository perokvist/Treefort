﻿using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Treefort.Events;

namespace Treefort.EntityFramework.Eventing
{
    public class EventStore : IEventStore
    {
        private readonly IEventContext _eventContext;
        private readonly Func<EventStream, IEventStream> _adapterFactory;

        public EventStore(IEventContext eventContext, Func<EventStream, IEventStream> adapterFactory)
        {
            _eventContext = eventContext;
            _adapterFactory = adapterFactory;
        }

        public async Task<IEventStream> LoadEventStreamAsync(System.Guid entityId)
        {
            var stream = await _eventContext.Streams.SingleOrDefaultAsync(e => e.AggregateId == entityId) ??
                         new EventStream() { Events = new Collection<Event>()};
            return _adapterFactory(stream);
        }

        public async Task StoreAsync(System.Guid entityId, long version, System.Collections.Generic.IEnumerable<IEvent> events)
        {
            var stream = _eventContext.Streams.SingleOrDefault(s => s.AggregateId == entityId) ??
                         _eventContext.Streams.Add(new EventStream() {AggregateId = entityId});
            var adapter = _adapterFactory(stream);
            adapter.Version = version;
            adapter.AddRange(events);
            await _eventContext.SaveChangesAsync();
        }
    }
}