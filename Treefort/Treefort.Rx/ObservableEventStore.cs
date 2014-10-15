using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Treefort.Common;
using Treefort.Events;

namespace Treefort.Rx
{
    public class ObservableEventStore : IEventStore, IObservable<IEvent>
    {
        private readonly IEventStore _eventStore;
        private readonly Subject<IEvent> _subject;

        public ObservableEventStore(IEventStore eventStore)
        {
            _eventStore = eventStore;
            _subject = new Subject<IEvent>();
        }

        public async Task AppendAsync(string streamName, int version, IEnumerable<IEvent> events)
        {
            var enumerableEvents = events as IEvent[] ?? events.ToArray();
            await _eventStore.AppendAsync(streamName, version, enumerableEvents);
            enumerableEvents.ForEach(x => _subject.OnNext(x));
        }

        public Task<IEventStream> LoadEventStreamAsync(string streamName, int version = int.MaxValue)
        {
            return _eventStore.LoadEventStreamAsync(streamName);
        }

        public IDisposable Subscribe(IObserver<IEvent> observer)
        {
            return _subject.Subscribe(observer);
        }
    }
}
