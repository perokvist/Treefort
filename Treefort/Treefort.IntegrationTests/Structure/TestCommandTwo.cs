using System;
using Treefort.Commanding;

namespace Treefort.IntegrationTests.Structure
{
    public class TestCommandTwo : ICommand
    {
        public Guid AggregateId { get; private set; }
        public Guid CorrelationId { get; private set; }
    }
}