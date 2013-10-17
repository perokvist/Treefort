using System;
using System.Collections.Generic;
using System.Linq;
using Treefort.Common.Extensions;
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
            var eventListner = new EventListener(projections());
            var eventPublisher = new EventPublisher(new List<IEventListener> { eventListner }, new ReceptorSubject(receptors(), logger), logger);
            var observableEventStore = new ObservableEventStore(eventStoreFactory());
            var router = routerFactory(logger, appServiceFactories.Select(fac => fac(observableEventStore, eventPublisher))); 
            var appServer = new ApplicationServer(router, logger);
            observableEventStore.Subscribe(eventPublisher);
            eventPublisher.Subscribe(cmd => appServer.DispatchAsync(cmd));
            return appServer;
        }
    }
}
