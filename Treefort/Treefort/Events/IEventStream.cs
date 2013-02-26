using System.Collections.Generic;

namespace Treefort.Events
{
    public interface IEventStream : ICollection<IEvent>
    {
        long Version { get; set; }
        long EventCount { get; }
        void AddRange(IEnumerable<IEvent> collection);

    }
}