using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Treefort.Events;

namespace Treefort.CommitLog
{
    public static class CommitLogExtensions
    {
        public static Func<string, IEnumerable<IEvent>, Task> ToStreamPoster(this ICommitLogClient client, Func<IEvent, byte[]> serializer)
        {
            return (streamName, events) => client.PostMessagesAsync(streamName, events.Select(x => new MessageToWrite(x.SourceId.ToString(), serializer(x))).ToList());
        }

        public static Func<IEnumerable<IEvent>, Task> ForStream(this Func<string, IEnumerable<IEvent>, Task> publisher, string streamName)
        {
            return events => publisher(streamName, events);
        }
    }
}