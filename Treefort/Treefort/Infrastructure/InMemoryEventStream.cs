using System.Collections.Generic;
using Treefort.Events;

namespace Treefort.Infrastructure
{
    public class InMemoryEventStream : List<IEvent>, IEventStream
    {
        public long Version { get; set; }
        public long EventCount
        {
            get { return base.Count; }
        }
    }
}