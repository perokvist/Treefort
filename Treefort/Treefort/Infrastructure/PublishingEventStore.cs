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

        public System.Threading.Tasks.Task<IEventStream> LoadEventStreamAsync(string streamName, int version = int.MaxValue)
        {
            return _eventStore.LoadEventStreamAsync(streamName);

        }

        public async System.Threading.Tasks.Task AppendAsync(string streamName, int version, System.Collections.Generic.IEnumerable<IEvent> events)
        {
            var enumerable = events as IEvent[] ?? events.ToArray();
            await _eventStore.AppendAsync(streamName, version, enumerable);
            //NOTE if the app breaks here = trouble :)
            await _eventPublisher.PublishAsync(enumerable);
        }
    }
}