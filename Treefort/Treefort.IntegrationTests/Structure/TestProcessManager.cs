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

        public ICommand When(IEvent @event)
        {
            return When((dynamic) @event);
        }

        public ICommand When(dynamic @event)
        {
            return null;
        }

        public ICommand When(TestEventThree @event)
        {
            return null;
        }

    }
}
