using System;
using System.Threading.Tasks;
using Treefort.Commanding;
using Treefort.Events;

namespace Treefort.Application
{
    public abstract class StatelessApplicationService
    {
        private readonly IEventPublisher _eventPublisher;

        protected StatelessApplicationService(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        protected void Do(Func<Action<ICommand, IEvent>, Task> e)
        {
            e((command, @event) =>
            {
                @event.CorrelationId = command.CorrelationId;
                _eventPublisher.PublishAsync(@event);
            });
        }
    }
}