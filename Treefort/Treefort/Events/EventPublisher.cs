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
        private readonly ISubject<IEvent, ICommand> _receptors;
        private readonly ILogger _logger;
        private readonly ISubject<ICommand> _subject;

        //NOTE beware of Autofac IDisposabe With ISubject
        public EventPublisher(IEnumerable<IEventListener> eventListeners, ISubject<IEvent, ICommand> receptors, ILogger logger)
        {
            _eventListeners = eventListeners;
            _receptors = receptors;
            _logger = logger;
            _subject = new Subject<ICommand>(); //TODO move receptors to bootstrapping instead ?
            _receptors.Subscribe(_subject);
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
            _receptors.OnNext(value); //TODO fix this ugly solution
            _eventListeners.ForEach(el => el.ReceiveAsync(new[] { value })); //TODO possible to group by correlation
        }

        IDisposable IObservable<ICommand>.Subscribe(IObserver<ICommand> observer)
        {
            return _subject.Subscribe(observer);
        }

    }
}