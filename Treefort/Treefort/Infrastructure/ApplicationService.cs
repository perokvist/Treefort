using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Treefort.Commanding;
using Treefort.Common.Extensions;
using Treefort.Events;
namespace Treefort.Infrastructure
{
    public class ApplicationService : IApplicationService
    {
        private readonly IEventStore _eventStore;
        private readonly ICommandRoutes _commandRoutes;

        public ApplicationService(IEventStore eventStore, ICommandRoutes commandRoutes)
        {
            _eventStore = eventStore;
            _commandRoutes = commandRoutes;
        }

        public async Task HandleAsync(ICommand command)
        {
            //Validate route
            if (!_commandRoutes.HasRoute(command))
                return;

            //Load events
            var eventStream = await _eventStore
                .LoadEventStreamAsync(command.AggregateId)
                .ConfigureAwait(false);
            //Instantiate aggregate
            var aggregate = _commandRoutes.AggregateFactory(command)();
            
            //Replay events
            eventStream.ForEach(e => aggregate.Handle((dynamic)e));
            
            //Execute command
            Action<IEvent> setCorrelation = e => e.CorrelationId = command.CorrelationId;
            var events = aggregate.Handle((dynamic)command);
            EnumerableExtansions.ForEach(events, setCorrelation);
            
            //Store events
            await _eventStore
                .StoreAsync(command.AggregateId, eventStream.Version, events)
                .ConfigureAwait(false);
        }
    }

}