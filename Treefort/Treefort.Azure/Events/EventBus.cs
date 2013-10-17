using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Treefort.Azure.Infrastructure;
using Treefort.Azure.Messaging;
using Treefort.Commanding;
using Treefort.Common;
using Treefort.Events;
using Treefort.Infrastructure;
using Treefort.Messaging;

namespace Treefort.Azure.Events
{
    public class EventBus : IEventBus
    {
        private readonly IMessageSender _sender;
        private readonly ITextSerializer _serializer;

        public EventBus(IMessageSender sender, ITextSerializer serializer)
        {
            _sender = sender;
            _serializer = serializer;
        }

        public Task PublishAsync(Envelope<IEvent> @event)
        {
            return _sender.SendAsync(() => BuildMessage(@event));
        }

        public void Publish(IEnumerable<Envelope<IEvent>> events)
        {
            events.ForEach(cmd => _sender.SendAsync(() => BuildMessage(cmd)));
        }

        private BrokeredMessage BuildMessage(Envelope<IEvent> @event)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            _serializer.Serialize(writer, @event.Body); //TODO check writer type
            stream.Position = 0;
            return new BrokeredMessage(stream, true)
            {
                MessageId = @event.Body.SourceId.ToString(),
                CorrelationId = @event.Body.CorrelationId.ToString() //TODO use evn correlation?
            };
        }
    }
}