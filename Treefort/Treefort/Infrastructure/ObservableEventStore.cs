using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Treefort.Common;
using Treefort.Events;

namespace Treefort.Infrastructure
{
    public class ObservableEventStore : IObservableEventStore
    {
        private readonly IEventStore _eventStore;
        private readonly Subject<IEvent> _subject;

        public ObservableEventStore(IEventStore eventStore)
        {
            _eventStore = eventStore;
            _subject = new Subject<IEvent>();
        }

        public async Task StoreAsync(Guid entityId, long version, IEnumerable<IEvent> events)
        {
            var enumerableEvents = events as IEvent[] ?? events.ToArray();
            await _eventStore.StoreAsync(entityId, version, enumerableEvents);
            enumerableEvents.ForEach(_subject.OnNext);
        }

        public Task<IEventStream> LoadEventStreamAsync(Guid entityId)
        {
            return _eventStore.LoadEventStreamAsync(entityId);
        }

        public IDisposable Subscribe(IObserver<IEvent> observer)
        {
            return _subject.Subscribe(observer);
        }
    }
}