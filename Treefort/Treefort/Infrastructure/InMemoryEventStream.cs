using System.Collections.Generic;
using Treefort.Events;

namespace Treefort.Infrastructure
{
    public class InMemoryEventStream : List<IEvent>, IEventStream
    {
        public int Version { get; set; }
        public int EventCount
        {
            get { return base.Count; }
        }
    }
}