using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Treefort.Commanding;
using Treefort.Common.Extensions;
using Treefort.Domain;
using Treefort.Events;

namespace Treefort.Application
{
    public abstract class StatefulApplicationService<TAggregate, TState>
        where TAggregate : class
        where TState : class, IState, new()
    {
        private readonly IEventStore _eventStore;
        private readonly Func<TState, TAggregate> _aggregateFactory;

        protected StatefulApplicationService(IEventStore eventStore,
            Func<TState, TAggregate> aggregateFactory)
        {
            _eventStore = eventStore;
            _aggregateFactory = aggregateFactory;
        }

        protected async Task UpdateAsync(ICommand command, Func<TAggregate, IEnumerable<IEvent>> executeCommandUsingThis)
        {
            await _eventStore
                        .LoadEventStreamAsync(command.AggregateId)
                        .ContinueWith(t => UpdateAsync(command, executeCommandUsingThis, t.Result))
                        .Unwrap()
                        .ConfigureAwait(false);
        }

        private async Task<IEvent[]> UpdateAsync(ICommand command,
            Func<TAggregate, IEnumerable<IEvent>> executeCommandUsingThis,
            IEventStream eventStream)
        {
            //State
            var state = new TState();

            //Replay events
            eventStream.ForEach(e => state.When((dynamic)e));

            //Aggregate
            var aggregate = _aggregateFactory(state);
            var events = executeCommandUsingThis(aggregate)
                .ToArray();

            //Correlation
            events.ForEach(e => e.CorrelationId = command.CorrelationId);

            //Persist
            await _eventStore
                .StoreAsync(command.AggregateId, eventStream.Version, events)
                .ConfigureAwait(false);
            return events;
        }
    }
}