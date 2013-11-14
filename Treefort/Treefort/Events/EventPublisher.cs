using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Treefort.Commanding;
using Treefort.Common;
using Treefort.Infrastructure;

namespace Treefort.Events
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IEnumerable<IEventListener> _eventListeners;
        private readonly ILogger _logger;
        private Subject<IEvent> _subject;

        //NOTE beware of Autofac IDisposabe With ISubject
        public EventPublisher(IEnumerable<IEventListener> eventListeners, ILogger logger)
        {
            _eventListeners = eventListeners;
            _logger = logger;
            _subject = new Subject<IEvent>();
        }

        void IObserver<IEvent>.OnCompleted()
        {
        }

        void IObserver<IEvent>.OnError(Exception error)
        {
        }

        void IObserver<IEvent>.OnNext(IEvent value)
        {
            _logger.Info(string.Format("Publisher Received {0} ({1})", value, value.CorrelationId));
            _eventListeners.ForEach(el => el.ReceiveAsync(new[] { value })); //TODO possible to group by correlation
            _subject.OnNext(value);
        }

        public IDisposable Subscribe(IObserver<IEvent> observer)
        {
            return _subject.Subscribe(observer);
        }
    }
}