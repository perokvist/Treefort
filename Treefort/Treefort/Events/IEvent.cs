using System;

namespace Treefort.Events
{
    public interface IEvent
    {
        Guid CorrelationId { get; set; }
        Guid SourceId { get; }

    }
}