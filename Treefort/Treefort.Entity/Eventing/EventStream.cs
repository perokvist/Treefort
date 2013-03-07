using System;
using System.Collections.Generic;

namespace Treefort.EntityFramework.Eventing
{
    public class EventStream 
    {
        public long Version { get; set; }
        public Guid AggregateId { get; set; }
        public virtual ICollection<Event> Events { get; set; } 
    }
}