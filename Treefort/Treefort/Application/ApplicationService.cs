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

            Func<Type, Guid, string> streamNameFactory = (t, g) => string.Format("{0}-{1}", char.ToLower(t.Name[0]) + t.Name.Substring(1), g.ToString("N"));

            await UpdateAsync(aggregateFactory, store, streamNameFactory, command, executeCommandUsingThis);
        }

        public static async Task UpdateAsync<TAggregate, TState>
           (Func<TState, TAggregate> aggregateFactory,
            IEventStore store,
            Func<Type, Guid, string> streamNameFactory,
            ICommand command,
            Func<TAggregate, IEnumerable<IEvent>> executeCommandUsingThis)
            where TAggregate : class
            where TState : class, IState, new()
        {
            await store
                        .LoadEventStreamAsync(streamNameFactory(typeof(TAggregate), command.AggregateId))
                        .ContinueWith(t => UpdateAsync(aggregateFactory, store, command, executeCommandUsingThis, t.Result, streamNameFactory))
                        .Unwrap()
                        .ConfigureAwait(false);
        }

        public static async Task<IEvent[]> UpdateAsync<TAggregate, TState>(
            Func<TState, TAggregate> aggregateFactory,
                    IEventStore store,
                    ICommand command,
                    Func<TAggregate, IEnumerable<IEvent>> executeCommandUsingThis,
                    IEventStream eventStream,
                    Func<Type, Guid, string> streamNameFactory)
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

            //Version
            var expectedVersion = command is IBasedOnVersion ? ((IBasedOnVersion) command).OriginalVersion : eventStream.Version;

            //Persist
            await store
                .AppendAsync(streamNameFactory(typeof(TAggregate), command.AggregateId), expectedVersion, events)
                .ConfigureAwait(false);
            return events;
        }
    }
}