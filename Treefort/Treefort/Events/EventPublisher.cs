using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Treefort.Infrastructure;

namespace Treefort.Events
{
    public class EventPublisher : IEventPublisher
    {
        private readonly Func<IEnumerable<IEvent>, Task> _eventListerners;
        private readonly Action<string> _log;

        public EventPublisher(IEnumerable<IEventListener> eventListeners, Action<string> logger)
            : this(events => Task.WhenAll(eventListeners.Select(x => x.ReceiveAsync(events)).ToList()), logger)
        {
        }

        public EventPublisher(IEnumerable<IEventListener> eventListeners, ILogger logger)
            : this(eventListeners, logger.Info)
        {
        }

        public EventPublisher(Action<string> logger, params IEventListener[] eventListeners) : this(eventListeners.ToArray(), logger)
        {}

        public EventPublisher(Func<IEnumerable<IEvent>, Task> eventListerner, Action<string> logger)
        {
            _eventListerners = eventListerner;
            _log = logger;
        }

        public Task PublishAsync(IEvent @event)
        {
            _log(string.Format("Publisher Received {0} ({1})", @event, @event.CorrelationId));
            return PublishAsync(new[] { @event });
        }

        public Task PublishAsync(IEnumerable<IEvent> events)
        {
            var enumerable = events as IEvent[] ?? events.ToArray();
            _log(string.Format("Publisher Received {0} events ({1})", enumerable.Count(), enumerable.First().CorrelationId));
            return _eventListerners(enumerable);
        }
    }
}