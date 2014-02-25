using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treefort.Application;
using Treefort.Commanding;
using Treefort.Domain;
using Treefort.Events;

namespace Treefort.IntegrationTests.Structure
{
    public class ProcessApplicationService : StatefulApplicationService<TestProcessManagerAggregate, TestState>, 
        IApplicationService, IReceptor
    {
        public ProcessApplicationService(IEventStore eventStore)
            : base(eventStore, state => new TestProcessManagerAggregate(state))
        {
        }
        
        public Task HandleAsync(ICommand command)
        {
            return When((dynamic) command);
        }
        
        public Task When(TestCommandTwo command)
        {
            return Task.FromResult(new object());
        }

        public Task<ICommand> HandleAsync(IEvent @event)
        {
            return When((dynamic) @event);
        }

        public Task<ICommand> When(dynamic @event)
        {
            return Task.FromResult<ICommand>(null);
        }

        public Task<ICommand> When(TestEventThree @event)
        {
            return Task.FromResult<ICommand>(null);
        }

    }
}
