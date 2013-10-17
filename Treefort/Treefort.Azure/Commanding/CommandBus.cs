using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Treefort.Azure.Infrastructure;
using Treefort.Azure.Messaging;
using Treefort.Commanding;
using Treefort.Common;
using Treefort.Infrastructure;
using Treefort.Messaging;

namespace Treefort.Azure.Commanding
{
    public class CommandBus : ICommandBus
    {
        private readonly IMessageSender _sender;
        private readonly ITextSerializer _serializer;

        public CommandBus(IMessageSender sender, ITextSerializer serializer)
        {
            _sender = sender;
            _serializer = serializer;
        }

        public Task SendAsync(Envelope<ICommand> command)
        {
            return _sender.SendAsync(() => BuildMessage(command));
        }

        public void Send(IEnumerable<Envelope<ICommand>> commands)
        {
            commands.ForEach(cmd => _sender.SendAsync(() => BuildMessage(cmd)));
        }

        private BrokeredMessage BuildMessage(Envelope<ICommand> command)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            _serializer.Serialize(writer, command.Body); //TODO check writer type
            stream.Position = 0;
            return new BrokeredMessage(stream, true)
            {
                MessageId = command.Body.AggregateId.ToString(),
                CorrelationId = command.Body.CorrelationId.ToString() //TODO use evn correlation?
            };
        }
    }
}