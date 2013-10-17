using System.IO;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Treefort.Application;
using Treefort.Azure.Infrastructure;
using Treefort.Azure.Messaging;
using Treefort.Infrastructure;

namespace Treefort.Azure.Commanding
{
    public class CommandProcessor : IProcessor
    {
        private readonly IMessageReceiver _messageReceiver;
        private readonly ICommandRouter _commandRouter;
        private readonly ITextSerializer _serializer;

        public CommandProcessor(
            IMessageReceiver messageReceiver,
            ICommandRouter commandRouter,
            ITextSerializer serializer)
        {
            _messageReceiver = messageReceiver;
            _commandRouter = commandRouter;
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

            var command = payload as Treefort.Commanding.ICommand;
            return _commandRouter
                .GetServiceFor(command)
                .HandleAsync(command);
        }
    }
}