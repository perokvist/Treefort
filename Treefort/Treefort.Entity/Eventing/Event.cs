using System;
using Treefort.Events;

namespace Treefort.EntityFramework.Eventing
{
    public class Event : IEvent
    {
        protected Event()
        {
            
        }

        public Event(string json, string type)
        {
            this.Json = json;
            Type = type;
            Created = DateTime.UtcNow;
        }

        public Guid Id { get; set; }
        public string Json { get; set; }
        public string Type { get; set; }
        public DateTime Created { get; set; }
        public string CorrelationId { get; set; }
    }
}