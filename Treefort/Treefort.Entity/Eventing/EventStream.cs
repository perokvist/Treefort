using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Treefort.EntityFramework.Eventing
{
    public class EventStream
    {
        public long Version { get; set; }
        public Guid AggregateId { get; set; }

        private ICollection<Event> _events;
        public virtual ICollection<Event> Events
        {
            get { return _events ?? (_events = new Collection<Event>()); }
            set { _events = value; }
        }
    }
}