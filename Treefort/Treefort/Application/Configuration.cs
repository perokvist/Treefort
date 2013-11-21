using System;
using System.Collections.Generic;
using System.Linq;
using Treefort.Commanding;
using Treefort.Common;
using Treefort.Domain;
using Treefort.Events;
using Treefort.Infrastructure;
using Treefort.Read;

namespace Treefort.Application
{
    public class Configuration
    {
        //TODO fluent config 

        public static IApplicationServer InMemory(IEnumerable<Func<IEventStore, IEventPublisher, IApplicationService>> appServiceFactories, Func<IEnumerable<IProjection>> projections, Func<IEnumerable<IReceptor>> receptors)
        {
            return WithEventStore(() => new InMemoryEventStore(() => new InMemoryEventStream()), appServiceFactories, projections, receptors);
        }
        
        public static IApplicationServer WithEventStore(Func<IEventStore> eventStoreFactory, IEnumerable<Func<IEventStore, IEventPublisher, IApplicationService>> appServiceFactories, Func<IEnumerable<IProjection>> projections, Func<IEnumerable<IReceptor>> receptors)
        {
            return WithEventStore((logger, services) => new InMemoryCommandRouter(logger).Tap(r => services.ForEach(r.RegisterHandler)),
                eventStoreFactory, appServiceFactories, projections, receptors);
        }

        public static IApplicationServer WithEventStore(Func<ILogger, IEnumerable<IApplicationService>, ICommandRouter> routerFactory, Func<IEventStore> eventStoreFactory, IEnumerable<Func<IEventStore, IEventPublisher, IApplicationService>> appServiceFactories, Func<IEnumerable<IProjection>> projections, Func<IEnumerable<IReceptor>> receptors)
        {
            var logger = new ConsoleLogger();
            var eventListeners = new List<IEventListener>
            {
                new ProjectionEventListener(projections())
            };
            var eventPublisher = new EventPublisher(eventListeners, logger);
            var publishingEventStore = new PublishingEventStore(eventStoreFactory(), eventPublisher);
            var router = routerFactory(logger, appServiceFactories.Select(fac => fac(publishingEventStore, eventPublisher)));
            var appServer = new ApplicationServer(router, logger);
            eventListeners.Add(new ReceptorListener(receptors(), appServer));
            return appServer;
        }
    }
}
