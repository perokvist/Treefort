using System.Linq;
using Treefort.Events;
namespace Treefort.Infrastructure
{
    public class PublishingEventStore : IEventStore
    {
        private readonly IEventStore _eventStore;
        private readonly IEventPublisher _eventPublisher;

        public PublishingEventStore(IEventStore eventStore, IEventPublisher eventPublisher)
        {
            _eventStore = eventStore;
            _eventPublisher = eventPublisher;
        }

        public async System.Threading.Tasks.Task AppendAsync(System.Guid entityId, long version, System.Collections.Generic.IEnumerable<IEvent> events)
        {
            var enumerable = events as IEvent[] ?? events.ToArray();
            await _eventStore.AppendAsync(entityId, version, enumerable);
            //NOTE if the app breaks here = trouble :)
            await _eventPublisher.PublishAsync(enumerable);
        }

        public System.Threading.Tasks.Task<IEventStream> LoadEventStreamAsync(System.Guid entityId)
        {
            return _eventStore.LoadEventStreamAsync(entityId);
        }
    }
}