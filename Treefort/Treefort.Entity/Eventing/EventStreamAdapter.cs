using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Treefort.Common.Extensions;
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
            AddEvent(item);
        }

        protected override void SetItem(int index, IEvent item)
        {
            base.SetItem(index, item);
            AddEvent(item);
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            _eventStream.Events.Clear();
        }

        protected override void RemoveItem(int index)
        {
            var @event = Items[index];
            base.RemoveItem(index);
            _eventStream.Events.Remove((Event)@event);
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
            collection.ForEach(Add);
        }

        private void AddEvent(IEvent item)
        {
            _eventStream.Events.Add(new Event(_jsonConverter.SerializeObject(item), item.GetType()));
        }
    }
}