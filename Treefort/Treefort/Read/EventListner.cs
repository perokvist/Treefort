﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Treefort.Events;

namespace Treefort.Read
{
    public class EventListner : IEventListner
    {
        private readonly IEnumerable<IProjection> _listerners;

        public EventListner(IEnumerable<IProjection> listerners)
        {
            _listerners = listerners;
        }

        public Task ReceiveAsync(IEnumerable<IEvent> events)
        {
            return Task.WhenAll(events.SelectMany(e => _listerners.Select(l => l.WhenAsync(e))));
        }
    }
}