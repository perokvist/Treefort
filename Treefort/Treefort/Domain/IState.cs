using System;
using System.Collections.Generic;
using Treefort.Events;

namespace Treefort.Domain
{
    public interface IState
    {
        Guid AggregateId { get; set; }
        void When(IEvent @event);
        long Version { get; set; }
    }
}