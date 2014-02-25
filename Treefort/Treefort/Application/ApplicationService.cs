using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Treefort.Commanding;
using Treefort.Common;
using Treefort.Domain;
using Treefort.Events;

namespace Treefort.Application
{
    public static class ApplicationService
    {
        public static async Task UpdateAsync<TAggregate, TState>
            (Func<TState, TAggregate> aggregateFactory, 
            IEventStore store, 
            ICommand command, 
            Func<TAggregate, IEnumerable<IEvent>> executeCommandUsingThis)
                where TAggregate : class
                where TState : class, IState, new()
        {
            await store
                        .LoadEventStreamAsync(command.AggregateId)
                        .ContinueWith(t => UpdateAsync(aggregateFactory,store, command, executeCommandUsingThis, t.Result))
                        .Unwrap()
                        .ConfigureAwait(false);

        }
        public static async Task<IEvent[]> UpdateAsync<TAggregate, TState>(
            Func<TState, TAggregate> aggregateFactory,
                    IEventStore store, 
                    ICommand command,
                    Func<TAggregate, IEnumerable<IEvent>> executeCommandUsingThis,
                    IEventStream eventStream)
                    where TAggregate : class
                    where TState : class, IState, new()
        {
            //State
            var state = new TState();

            //Replay events
            eventStream.ForEach(e => state.When((dynamic)e));

            //Aggregate
            var aggregate = aggregateFactory(state);
            var events = executeCommandUsingThis(aggregate)
                .ToArray();

            //Correlation
            events.ForEach(e => e.CorrelationId = command.CorrelationId);

            //Persist
            await store
                .StoreAsync(command.AggregateId, eventStream.Version, events)
                .ConfigureAwait(false);
            return events;
        }
    }
}