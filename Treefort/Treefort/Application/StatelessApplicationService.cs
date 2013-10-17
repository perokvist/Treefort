using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Treefort.Commanding;
using Treefort.Common.Extensions;
using Treefort.Events;

namespace Treefort.Application
{
    public abstract class StatelessApplicationService
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly Subject<IEvent> _eventSubject;

        protected StatelessApplicationService(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
            _eventSubject = new Subject<IEvent>();
        }
        
        protected void Do(Func<Action<ICommand, IEvent>, Task> e)
        {
            _eventSubject.Subscribe(_eventPublisher);
            e((command, @event) => _eventSubject.OnNext(@event.Tap(x => x.CorrelationId = command.CorrelationId)))
                .ContinueWith(task => _eventSubject
                    .Tap(x=> x.OnCompleted())
                    .Tap(x => x.Dispose()));
        }
    }
}