using System;
using System.Threading.Tasks;
using Treefort.Domain;
using Treefort.Events;

namespace Treefort.CommitLog
{
    public static class StateEventHandler
    {

        public static async Task ForAsync<TState, TEvent>(TEvent @event,
           IStateRepository<TState> repository)
            where TEvent : IEvent
           where TState : IState, new()
        {
            var state = await repository.GetOrCreateAsync(@event.SourceId);
            state.When(@event);
            await repository.InsertOrUpdateAsync(state);
        }

        public static async Task ForAsync<TState, TEvent>(TEvent @event, Action<TEvent, TState> applyAction,
            IStateRepository<TState> repository)
            where TState : IState, new()
            where TEvent : IEvent
        {
            var state = await repository.GetOrCreateAsync(@event.SourceId);
            applyAction(@event, state);
            await repository.InsertOrUpdateAsync(state);
        }

    }
}