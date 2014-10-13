using System.Collections.Generic;
using Treefort.Events;

namespace Treefort.EventStore
{
    public class EventStream : List<IEvent>, IEventStream
    {
        public EventStream(IEnumerable<IEvent> events, int version)
            : base(events)
        {
            Version = version;
        }

        public int Version { get; set; }

        public int EventCount
        {
            get { return Count; }
        }
    }
}