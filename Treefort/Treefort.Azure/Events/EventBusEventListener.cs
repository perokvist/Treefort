using System.Collections.Generic;
using System.Threading.Tasks;
using Treefort.Commanding;
using Treefort.Events;
using Treefort.Messaging;

namespace Treefort.Azure.Events
{
    public class EventBusEventListener : IEventListener
    {
        private readonly IEventBus _eventBus;

        public EventBusEventListener(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public async Task ReceiveAsync(IEnumerable<IEvent> events)
        {
            foreach (var @event in events)
            {
                await _eventBus.PublishAsync(new Envelope<IEvent>(@event)); //TODO check impl convert
            }
        }
    }
} ;
