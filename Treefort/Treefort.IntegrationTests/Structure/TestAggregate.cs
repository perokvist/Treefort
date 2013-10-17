using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treefort.Events;

namespace Treefort.IntegrationTests.Structure
{
    public class TestAggregate
    {
        public IEnumerable<IEvent> AggragateMethod(string test)
        {
            return new ArraySegment<IEvent>(new IEvent[] { });
        } 

    }
}
