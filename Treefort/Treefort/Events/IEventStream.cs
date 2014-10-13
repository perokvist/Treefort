using System.Collections.Generic;

namespace Treefort.Events
{
    public interface IEventStream : ICollection<IEvent>
    {
        int Version { get; set; }
        int EventCount { get; }
        void AddRange(IEnumerable<IEvent> collection);

    }
}