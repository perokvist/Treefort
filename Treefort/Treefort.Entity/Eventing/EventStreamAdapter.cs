using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Treefort.Common;
using Treefort.EntityFramework.Extensions;
using Treefort.Events;
using System;
using Treefort.Infrastructure;

namespace Treefort.EntityFramework.Eventing
{
    public class EventStreamAdapter : Collection<IEvent>, IEventStream
    {
        private readonly EventStream _eventStream;
        private readonly IJsonConverter _jsonConverter;
        private readonly IEventTypeResolver _eventTypeResolver;
        private readonly bool _initialized;

        public EventStreamAdapter(EventStream eventStream, IJsonConverter jsonConverter, IEventTypeResolver eventTypeResolver)
        {
            _eventStream = eventStream;
            _jsonConverter = jsonConverter;
            _eventTypeResolver = eventTypeResolver;
            this.AddRange(_eventStream.Events.OrderBy(e => e.Created)
                .Select(e => _jsonConverter.DeserializeObject(e.Json, _eventTypeResolver.Resolve(e.Type)))
                .Cast<IEvent>()
                .ToList());
            _initialized = true;
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
            var e = _eventStream.Events.SingleOrDefault(ev => ev.CorrelationId == @event.CorrelationId);
            base.RemoveItem(index);
            _eventStream.Events.Remove(e);
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
            if (_initialized)
                _eventStream.Events.Add(new Event(_jsonConverter.SerializeObject(item), _eventTypeResolver.AsString(item.GetType()), item.CorrelationId)); 
        }
    }
}