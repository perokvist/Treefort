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
using Treefort.Messaging;
using Treefort.Read;

namespace Treefort.Azure
{
    public class Configuration
    {
        public static IProcessor CommandProcessorInMemory(IEnumerable<Func<IEventStore, IEventPublisher, IApplicationService>> appServiceFactories)
        {
            return CommandProcessorWithEventStore(() => new InMemoryEventStore(() => new InMemoryEventStream()), appServiceFactories);
        }

        public static IProcessor CommandProcessorWithEventStore(Func<IEventStore> eventStoreFactory, IEnumerable<Func<IEventStore, IEventPublisher, IApplicationService>> appServiceFactories)
        {
            return WithEventStore((logger, services) => new InMemoryCommandRouter(logger).Tap(r => services.ForEach(r.RegisterHandler)),
                eventStoreFactory, appServiceFactories, 
                () => new SubscriptionReceiver("Commands", "CommandSubscription"),
                () => new TopicSender("Commands"),
                () => new JsonTextSerializer(),
                () => new List<IEventListener> { new EventBusEventListener(new EventBus(new TopicSender("Events"), new JsonTextSerializer() )) });
        }

        public static IProcessor WithEventStore
            (Func<ILogger, IEnumerable<IApplicationService>, ICommandRouter> routerFactory, 
            Func<IEventStore> eventStoreFactory, 
            IEnumerable<Func<IEventStore, IEventPublisher, IApplicationService>> appServiceFactories, 
            Func<IMessageReceiver> messageReceiverFactory,
            Func<IMessageSender> messageSenderFactory,
            Func<ITextSerializer> serializerFactory,
            Func<IEnumerable<IEventListener>> eventListenersFactory)
        {
            var logger = new ConsoleLogger();
            var eventPublisher = new EventPublisher(eventListenersFactory(), 
                new ReceptorSubject(Enumerable.Empty<IReceptor>(), logger), logger);
            var observableEventStore = new ObservableEventStore(eventStoreFactory());
            var router = routerFactory(logger, appServiceFactories.Select(fac => fac(observableEventStore, eventPublisher)));
            var processor =  new CommandProcessor(messageReceiverFactory(), router, serializerFactory());
            var commandBus = new CommandBus(messageSenderFactory(), new JsonTextSerializer());
            observableEventStore.Subscribe(eventPublisher);
            eventPublisher.Subscribe(cmd => commandBus.SendAsync(new Envelope<ICommand>(cmd))); //TODO cmd -> envelope 
            return processor;
        }

        public static IProcessor EventProcessor(Func<IEnumerable<IReceptor>> receptors)
        {
         return new EventProcessor(
             new SubscriptionReceiver("Events", "EventSubscription"), 
             new EventPublisher(null, new ReceptorSubject(receptors(), new ConsoleLogger()), new ConsoleLogger()),
             new JsonTextSerializer());   
        }
    }
}