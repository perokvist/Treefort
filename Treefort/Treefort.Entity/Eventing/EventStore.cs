using System;
using System.Collections.ObjectModel;
using System.Data;
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

        public Task<IEventStream> LoadEventStreamAsync(System.Guid entityId)
        {
            return _eventContext.Streams.SingleOrDefaultAsync(e => e.AggregateId == entityId)
                .ContinueWith(s => _adapterFactory(s.Result ?? new EventStream()));
        }

        public async Task StoreAsync(System.Guid entityId, long version, System.Collections.Generic.IEnumerable<IEvent> events)
        {
            var stream = await _eventContext.Streams.SingleOrDefaultAsync(s => s.AggregateId == entityId);
            
            if(version != stream.Version)
                throw new OptimisticConcurrencyException("EventStream version is old");

            var adapter = _adapterFactory(stream ?? _eventContext.Streams.Add(new EventStream() { AggregateId = entityId }));
            adapter.Version = version + 1;
            adapter.AddRange(events);
            await _eventContext.SaveChangesAsync();
        }
    }
}