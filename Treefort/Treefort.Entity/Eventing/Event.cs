using System;
using Treefort.Events;

namespace Treefort.EntityFramework.Eventing
{
    public class Event : IEvent
    {
        protected Event()
        {
            
        }

        public Event(string json, string type, Guid correlationId)
        {
            this.Json = json;
            Type = type;
            CorrelationId = correlationId;
            Created = DateTime.UtcNow;
        }

        public Guid Id { get; set; }
        public string Json { get; set; }
        public string Type { get; set; }
        public DateTime Created { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid SourceId { get; private set; }
    }
}