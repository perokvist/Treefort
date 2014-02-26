using System.Collections.Generic;
using Treefort.Events;

namespace Treefort.EventStore
{
    public class EventStream : List<IEvent>, IEventStream
    {
        public EventStream(IEnumerable<IEvent> events, long version)
            : base(events)
        {
            Version = version;
        }

        public long Version { get; set; }

        public long EventCount
        {
            get { return Count; }
        }
    }
}