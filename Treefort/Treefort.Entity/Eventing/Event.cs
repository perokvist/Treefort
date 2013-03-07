using System;
using Treefort.Events;

namespace Treefort.EntityFramework.Eventing
{
    public class Event : IEvent
    {
        protected Event()
        {
            
        }

        public Event(string json, Type type)
        {
            this.Json = json;
            Type = type.ToString();
        }

        public Guid Id { get; set; }
        public string Json { get; set; }
        public string Type { get; set; }
    }
}