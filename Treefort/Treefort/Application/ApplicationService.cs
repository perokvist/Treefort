using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Treefort.Commanding;
using Treefort.CommitLog;
using Treefort.Common;
using Treefort.Domain;
using Treefort.Events;

namespace Treefort.Application
{
    public static class ApplicationService
    {
        /// <summary>
        /// Gets publisher for stream
        /// </summary>
        /// <param name="streamName"></param>
        /// <param name="publisher"></param>
        /// <returns></returns>
         public static Func<IEnumerable<IEvent>, Task> StreamPublisher(string streamName, Func<string, IEnumerable<IEvent>, Task> publisher)
        {
            return events => publisher(streamName, events);
        }

        /// <summary>
        /// Gets helper function for publisher to streams
        /// </summary>
        /// <param name="client"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public static Func<string, IEnumerable<IEvent>, Task> Publisher(ICommitLogClient client, Func<IEvent, byte[]> serializer)
        {
            return (streamName, events) => client.PostMessagesAsync(streamName, events.Select(x => new MessageToWrite(x.SourceId.ToString(), serializer(x))).ToList());
        }

        /// <summary>
        /// Executes commmand for given aggregate
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <typeparam name="TAggregate"></typeparam>
        /// <typeparam name="TState"></typeparam>
        /// <param name="command"></param>
        /// <param name="stateFactory"></param>
        /// <param name="aggregateFactory"></param>
        /// <param name="executeCommandUsingThis"></param>
        /// <param name="publisher"></param>
        /// <returns></returns>
        /// <remarks>Only publishers events - does not apply the to the state</remarks>
        public static async Task Execute<TCommand, TAggregate, TState>(
            TCommand command,
            Func<Guid, Task<TState>> stateFactory,
            Func<TState, TAggregate> aggregateFactory,
            Func<TAggregate, IEnumerable<IEvent>> executeCommandUsingThis,
            Func<IEnumerable<IEvent>, Task> publisher
            )
            where TCommand : ICommand
            where TAggregate : class
            where TState : class, IState, new()
        {
            //State
            var state = await stateFactory(command.AggregateId);

            //Aggregate
            var aggregate = aggregateFactory(state);
            var events = executeCommandUsingThis(aggregate)
                .ToArray();

            //Correlation
            events.ForEach(e => e.CorrelationId = command.CorrelationId);

            await publisher(events);

        }

        /// <summary>
        /// Executes commmand for given aggregate
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <typeparam name="TAggregate"></typeparam>
        /// <typeparam name="TState"></typeparam>
        /// <param name="command"></param>
        /// <param name="stateRepository"></param>
        /// <param name="aggregateFactory"></param>
        /// <param name="executeCommandUsingThis"></param>
        /// <param name="publisher"></param>
        /// <returns></returns>
        public static async Task Execute<TCommand, TAggregate, TState>(
           TCommand command,
           IStateRepository<TState> stateRepository,
           Func<TState, TAggregate> aggregateFactory,
           Func<TAggregate, IEnumerable<IEvent>> executeCommandUsingThis,
           Func<IEnumerable<IEvent>, Task> publisher
           )
            where TCommand : ICommand
            where TAggregate : class
            where TState : class, IState, new()
        {
            //State
            var state = await stateRepository.GetOrCreateAsync(command.AggregateId);

            //Aggregate
            var aggregate = aggregateFactory(state);
            var events = executeCommandUsingThis(aggregate)
                .ToArray();

            //Correlation
            events.ForEach(e => e.CorrelationId = command.CorrelationId);

            await publisher(events);
            await stateRepository.InsertOrUpdateAsync(state);
        }


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