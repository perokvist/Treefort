using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treefort.Events;
using Treefort.Read;

namespace Treefort.IntegrationTests.Structure
{
    public class TestProjection : IProjection
    {
        public bool Retrived { get; set; }
        public int EventCount { get; set; }

        public Task WhenAsync(IEvent @event)
        {
            Retrived = true;
            EventCount++;
            return new Task(() => {  });
        }
    }
}
