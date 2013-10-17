using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treefort.Application;
using Treefort.Domain;
using Treefort.Events;

namespace Treefort.IntegrationTests.Structure
{
    public class TestApplicationService : StatelessApplicationService, IApplicationService
    {
        public TestApplicationService(IEventPublisher eventPublisher) : base(eventPublisher)
        {
        }

        public Task HandleAsync(Commanding.ICommand command)
        {
            When((dynamic)command);
            return Task.FromResult(new object());
        }

        public void When(TestCommand command)
        {
            Do(action =>
            {
                action(command,new TestEvent());
                action(command,new TestEventTwo());
                return Task.FromResult(new object());
            });  
        }

        public void When(TestCommandThree command)
        {
            Do(action =>
            {
                action(command, new TestEventThree());
                return Task.FromResult(new object());
            });
        }
    }
}
