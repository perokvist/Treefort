using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Treefort.Application;

namespace Treefort.Azure
{
    public class CommandProcessor
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
            _messageReceiver.StopAsync();
        }

        private Task HandleCommandAsync(BrokeredMessage message)
        {
            object payload;

            using (var stream = message.GetBody<Stream>())
            {
                using (var reader = new StreamReader(stream))
                {
                    try
                    {
                        payload = _serializer.Deserialize(reader);
                    }
                    catch (SerializationException e)
                    {
                        throw;
                        //return MessageReleaseAction.DeadLetterMessage(e.Message, e.ToString());
                    }
                }
            }

            try
            {
                var command = payload as Treefort.Commanding.ICommand;
                return _commandRouter
                    .GetServiceFor(command)
                    .HandleAsync(command);
            }
            catch (Exception e)
            {
                //return HandleProcessingException(message, traceIdentifier, e);
            }

            //return CompleteMessage(message, traceIdentifier);
            return null;
        }
    }
}