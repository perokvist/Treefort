using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Treefort.Events;
using Treefort.Infrastructure;

namespace Treefort.EventStore
{
    public static class Extensions
    {

        public static EventData ToEventData(this IEvent @event)
        {
            var type = @event.GetType();
            var md = new MetaData
            {
                ClrType = type.FullName,
                CreatedAt = DateTime.UtcNow
            };
            
            return new EventData(Guid.NewGuid(), type.Name, true, @event.ToJsonBytes(), md.ToJsonBytes());
        }

        public static async Task<List<ResolvedEvent>> ReadAllEventsFromStream(this IEventStoreConnection connection,
            string streamName)
        {
            var result = new List<ResolvedEvent>();
            StreamEventsSlice currentSlice;
            var nextSliceStart = StreamPosition.Start;
            do
            {
                currentSlice = await connection.ReadStreamEventsForwardAsync(streamName, nextSliceStart, 100, false);
                nextSliceStart = currentSlice.NextEventNumber;
                result.AddRange(currentSlice.Events);

            } while (!currentSlice.IsEndOfStream);

            return result;

        }

        public static IEvent ToEvent(this ResolvedEvent @event, IJsonConverter converter)
        {
            Func<ResolvedEvent, IEvent> map = e => (IEvent)converter.DeserializeObject(Encoding.UTF8.GetString(e.Event.Data), Type.GetType(e.Event.Metadata.ParseJson<MetaData>().ClrType));
            return map(@event);
        }
    }
}