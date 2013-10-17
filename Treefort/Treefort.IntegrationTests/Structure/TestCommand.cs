using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treefort.Commanding;

namespace Treefort.IntegrationTests.Structure
{
    public class TestCommand : ICommand
    {
        public Guid AggregateId { get; private set; }
        public Guid CorrelationId { get; private set; }
    }
}
