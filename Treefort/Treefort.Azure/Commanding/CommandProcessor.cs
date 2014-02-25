using System.IO;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Treefort.Application;
using Treefort.Azure.Infrastructure;
using Treefort.Azure.Messaging;
using Treefort.Commanding;
using Treefort.Infrastructure;

namespace Treefort.Azure.Commanding
{
    public class CommandProcessor : IProcessor
    {
        private readonly IMessageReceiver _messageReceiver;
        private readonly ITextSerializer _serializer;
        private readonly ICommandDispatcher _commandRouter;

        public CommandProcessor(
            IMessageReceiver messageReceiver,
            ICommandDispatcher commandDisptacher,
            ITextSerializer serializer)
        {
            _messageReceiver = messageReceiver;
            _commandRouter = commandDisptacher;
            _serializer = serializer;
        }

        public void Start()
        {
            //TODO locks
            _messageReceiver.Start(HandleCommandAsync);
        }

        public void Stop()
        {
            _messageReceiver.StopAsync().Wait();
        }

        private Task HandleCommandAsync(BrokeredMessage message)
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

            var command = payload as ICommand;
            return _commandRouter
                .DispatchAsync(command);
        }
    }
}