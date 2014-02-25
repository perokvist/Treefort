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

        //public static IEnumerable<IProcessor> StartInMemory(
        //    IEnumerable<Func<IEventStore, IEventPublisher, IApplicationService>> appServiceFactories
        //    )
        //{
        //    return WithEventStore(
        //        () => new InMemoryEventStore(() => new InMemoryEventStream()),
        //        appServiceFactories);
        //}

        
        //public static IEnumerable<IProcessor> WithEventStore(
        //    Func<IEventStore> eventStoreFactory,
        //    IEnumerable<Func<IEventStore, IEventPublisher, IApplicationService>> appServiceFactories
        //    )
        //{
        //    return WithEventStore(eventStoreFactory, appServiceFactories, Enumerable.Empty<IReceptor>);
        //}

        //public static IEnumerable<IProcessor> WithEventStore(
        //    Func<IEventStore> eventStoreFactory,
        //    IEnumerable<Func<IEventStore, IEventPublisher, IApplicationService>> appServiceFactories,
        //    Func<IEnumerable<IReceptor>> receptorAccessor)
        //{
        //    var publisher = CreateEventPublisherWithEventBus(
        //        new TopicSender("Events"), Enumerable.Empty<IEventListener>(), null, null);
        //    var innerEventStore = eventStoreFactory();

        //    var publishingEventStore = new PublishingEventStore(innerEventStore,
        //        publisher);

        //    var appServices = appServiceFactories.Select(factory => factory(publishingEventStore, publisher));

        //    Func<ILogger, ICommandRouter> routerFactory =
        //        logger =>
        //        {
        //            var router = new InMemoryCommandRouter(logger);
        //            appServices.ForEach(router.RegisterHandler);
        //            return router;
        //        };
        //    return Start(routerFactory, receptorAccessor, 
        //        () => new ConsoleLogger(),
        //        () => new JsonTextSerializer());
        //}


        //public static IEnumerable<IProcessor> Start(
        //    Func<ILogger, ICommandRouter> commandRouterFunc,
        //    Func<IEnumerable<IReceptor>> receptorsFunc,
        //    Func<ILogger> loggerFactory,
        //    Func<ITextSerializer> serializerFactory)
        //{
        //    var serializer = serializerFactory();
        //    var logger = loggerFactory();

        //    var cp = CreateCommandProcessor(
        //        new SubscriptionReceiver("Commands", "CommandSubscription"),
        //        commandRouterFunc(logger), serializer);

        //    var commandBus = new CommandBus(new TopicSender("Commands"), serializer);

        //    var ep = CreateEventProcessor(
        //        new SubscriptionReceiver("Events", "EventSubscription"),
        //        () => new [] { new ReceptorListener(receptorsFunc(), commandBus) },
        //        logger, serializer);

        //    return new[] {ep, cp};
        //}

        //public static IProcessor CreateCommandProcessor(
        //    IMessageReceiver messageReceiver,
        //    ICommandRouter router,
        //    ITextSerializer serializer)
        //{
        //    return new CommandProcessor(messageReceiver, router, serializer);
        //}

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
        
        private static IProcessor CreateEventProcessor(
            IMessageReceiver receiver,
            IEventPublisher publisher,
            ITextSerializer serializer)
        {
            var eventPublisher = publisher;
            var eventReciever = receiver;
            return new EventProcessor(eventReciever, eventPublisher, serializer);
        }

        private static IEventPublisher CreateEventPublisherWithEventBus(
            IMessageSender messageSender,
            IEnumerable<IEventListener> additionalEventListeners, 
            ITextSerializer serializer, 
            ILogger logger)
        {
            var eventBus = new EventBus(messageSender, serializer);
            var eventBusListener = new EventBusEventListener(eventBus);
            var listerners = new List<IEventListener> {eventBusListener};
            listerners.AddRange(additionalEventListeners);
            var eventPublisher = new EventPublisher(listerners, logger);
            return eventPublisher;
        }
    }
}