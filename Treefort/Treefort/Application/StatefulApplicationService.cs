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
            await ApplicationService.UpdateAsync(_aggregateFactory, _eventStore, command, executeCommandUsingThis);
        }
    }
}