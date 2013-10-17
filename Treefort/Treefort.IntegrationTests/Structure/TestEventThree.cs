using System;
using Treefort.Events;

namespace Treefort.IntegrationTests.Structure
{
    public class TestEventThree : IEvent
    {
        public Guid CorrelationId { get; set; }
    }
}