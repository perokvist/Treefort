using System;

namespace Treefort.Commanding
{
    public interface ICommand
    {
        Guid AggregateId { get;  }

        Guid CorrelationId { get; }
    }
}
