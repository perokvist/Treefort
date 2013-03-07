﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Treefort.Common.Extensions;
using Treefort.Events;

namespace Treefort.Infrastructure
{
    /// <summary>
    /// Delegating Event Store Decorator
    /// </summary>
    public class DelegatingEventStore : IEventStore
    {
        private readonly IEventStore _store;
        private readonly IEnumerable<IEventListener> _eventListners;

        public DelegatingEventStore(IEventStore store, IEnumerable<IEventListener> eventListners)
        {
            _store = store;
            _eventListners = eventListners;
        }

        public async Task StoreAsync(System.Guid entityId, long version, IEnumerable<IEvent> events)
        {
            var enumerableEvents = events as IEvent[] ?? events.ToArray();
            await _store.StoreAsync(entityId, version, enumerableEvents);
            await Task.WhenAll(_eventListners.Select(el => el.ReceiveAsync(enumerableEvents)));
        }

        public Task<IEventStream> LoadEventStreamAsync(System.Guid entityId)
        {
            return _store.LoadEventStreamAsync(entityId);
        }
    }
}