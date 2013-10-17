using System;
using Treefort.Domain;
using Treefort.Events;

namespace Treefort.IntegrationTests.Structure
{
    public class TestState : IState
    {
        public Guid AggregateId { get; set; }
        public void When(IEvent @event)
        {
            throw new NotImplementedException();
        }

        public long Version { get; set; }
    }
}