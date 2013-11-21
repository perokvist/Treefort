using System.IO;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Treefort.Azure.Messaging;
using Treefort.Events;
using Treefort.Infrastructure;

namespace Treefort.Azure.Events
{
    public class EventProcessor : IProcessor
    {
        private readonly IMessageReceiver _messageReceiver;
        private readonly IEventPublisher _eventPublisher;
        private readonly ITextSerializer _serializer;

        public EventProcessor(
            IMessageReceiver messageReceiver,
            IEventPublisher eventPublisher,
            ITextSerializer serializer)
        {
            _messageReceiver = messageReceiver;
            _eventPublisher = eventPublisher;
            _serializer = serializer;
        }

        public void Start()
        {
            _messageReceiver.Start(HandleEventAsync);

        }

        public void Stop()
        {
            _messageReceiver.StopAsync().Wait();
        }

        private Task HandleEventAsync(BrokeredMessage message)
        {
            object payload;

            using (var stream = message.GetBody<Stream>())
            {
                using (var reader = new StreamReader(stream))
                {
                    payload = _serializer.Deserialize(reader);
                    //TODO deadletter
                }
            }
            var p = payload as IEvent;
            return _eventPublisher.PublishAsync(p);
        }
    }
}