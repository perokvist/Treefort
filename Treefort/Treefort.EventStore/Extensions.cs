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
                ClrType = type.AssemblyQualifiedName,
                CreatedAt = DateTime.UtcNow
            };
            
            return new EventData(Guid.NewGuid(), type.Name, true, @event.ToJsonBytes(), md.ToJsonBytes());
        }

        public static async Task<List<ResolvedEvent>> ReadAllEventsFromStream(this IEventStoreConnection connection,
            string streamName, int version = int.MaxValue)
        {
            var result = new List<ResolvedEvent>();
            StreamEventsSlice currentSlice;
            var sliceStart = StreamPosition.Start;
            const int size = 500;
            do
            {
                var sliceCount = sliceStart + size <= version
                    ? size
                    : version - sliceStart + 1;

                currentSlice = await connection.ReadStreamEventsForwardAsync(streamName, sliceStart, sliceCount, false);

                if (currentSlice.Status == SliceReadStatus.StreamNotFound)
                    throw new Exception("");

                if (currentSlice.Status == SliceReadStatus.StreamDeleted)
                    throw new Exception("");

                sliceStart = currentSlice.NextEventNumber;

                result.AddRange(currentSlice.Events);

            } while (version >= currentSlice.NextEventNumber && !currentSlice.IsEndOfStream);

            return result;

        }

        public static IEvent ToEvent(this ResolvedEvent @event, IJsonConverter converter)
        {
            Func<ResolvedEvent, IEvent> map = e => (IEvent)converter.DeserializeObject(Encoding.UTF8.GetString(e.Event.Data), Type.GetType(e.Event.Metadata.ParseJson<MetaData>().ClrType));
            return map(@event);
        }
    }
}