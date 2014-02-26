using System;
using System.Collections.Generic;
using System.Linq;
using Treefort.Application;
using Treefort.Azure.Commanding;
using Treefort.Azure.Events;
using Treefort.Azure.Infrastructure;
using Treefort.Azure.Messaging;
using Treefort.Commanding;
using Treefort.Common;
using Treefort.Domain;
using Treefort.Events;
using Treefort.Infrastructure;

namespace Treefort.Azure
{
    public class Configuration
    {

        public static IProcessor CreateQueueCommandProcessor(string connectionString, string path, ICommandDispatcher dispatcher)
        {
            var receiver = new QueueReceiver(connectionString, path);
            var textSerializer = new JsonTextSerializer();
            return new CommandProcessor(receiver, dispatcher, textSerializer);
        }

        public static IProcessor CreateEventProcessor(
            IMessageReceiver receiver,
            Func<IEnumerable<IEventListener>> eventListenerFactory,
            ILogger logger,
            ITextSerializer serializer)
        {
            var eventPublisher = new EventPublisher(eventListenerFactory(), logger);
            var eventReciever = receiver;
            return new EventProcessor(eventReciever, eventPublisher, serializer);
        }
    }
}