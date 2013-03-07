using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Treefort.Events;
using System;

namespace Treefort.EntityFramework.Eventing
{
    public class EventStreamAdapter : Collection<IEvent>, IEventStream
    {
        private readonly EventStream _eventStream;
        private readonly IJsonConverter _jsonConverter;

        public EventStreamAdapter(EventStream eventStream, IJsonConverter jsonConverter)
        {
            _eventStream = eventStream;
            _jsonConverter = jsonConverter;
            this.AddRange(_eventStream.Events
                .Select(e => _jsonConverter.DeserializeObject(e.Json, Type.GetType(e.Type)))
                .Cast<IEvent>());
        }

        protected override void InsertItem(int index, IEvent item)
        {
            base.InsertItem(index, item);
            _eventStream.Events.Add(new Event(_jsonConverter.SerializeObject(item), item.GetType()));
        }

        protected override void SetItem(int index, IEvent item)
        {
            base.SetItem(index, item);
            _eventStream.Events.Add(new Event(_jsonConverter.SerializeObject(item), item.GetType()));
        }

        public long EventCount
        {
            get { return Count; }
        }

        public long Version
        {
            get { return _eventStream.Version; }
            set { _eventStream.Version = value; }
        }

        public void AddRange(IEnumerable<IEvent> collection)
        {
            var i = 0;
            foreach (var @event in collection)
            {
                SetItem(i, @event);
                i++;
            }
        }
    }
}