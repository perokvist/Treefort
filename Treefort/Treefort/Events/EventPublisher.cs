using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Treefort.Infrastructure;

namespace Treefort.Events
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IEnumerable<IEventListener> _eventListeners;
        private readonly ILogger _logger;

        public EventPublisher(IEnumerable<IEventListener> eventListeners, ILogger logger)
        {
            _eventListeners = eventListeners;
            _logger = logger;
        }
        
        public Task PublishAsync(IEvent @event)
        {
            _logger.Info(string.Format("Publisher Received {0} ({1})", @event, @event.CorrelationId));
            return PublishAsync(new[] {@event});
        }

        public Task PublishAsync(IEnumerable<IEvent> events)
        {
            var publishTasks = _eventListeners
                .Select(x => x.ReceiveAsync(events))
                .ToList();
            return Task.WhenAll(publishTasks);
        }
    }
}