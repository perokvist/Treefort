using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treefort.Events;

namespace Treefort.IntegrationTests.Structure
{
    public class TestEvent : IEvent
    {
        public Guid CorrelationId { get; set; }
        public Guid SourceId { get; private set; }
    }
}
