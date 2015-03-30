using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Treefort.Events;

namespace Treefort.CommitLog
{
    public static class Initializer
    {
        public static Task ReplayAsync(ICommitLogClient client, string stream, ICheckpointReader checkpointReader,  Func<byte[], IEvent> deserializer, Action<IEvent> dispatcher)
        {
            Func<IEvent, Task> adapter = @event =>
            {
                dispatcher(@event);
                return Task.FromResult(0);
            };

            return ReplayAsync(client, stream, checkpointReader,deserializer, adapter);
        }

        public static async Task ReplayAsync(ICommitLogClient client, string stream, ICheckpointReader checkpointReader, Func<byte[], IEvent> deserializer, Func<IEvent, Task> dispatcher)
        {
            var position = checkpointReader.Read();
            var reader = await client.GetMessageReaderAsync(stream);
            var cts = new CancellationTokenSource();

                while (!cts.Token.IsCancellationRequested)
                {
                    var result = await reader.GetMessagesAsync(cts.Token, position, 1000);
                    if (result.HasMessages())
                    {
                        foreach (var @event in result.Messages.Select(m => deserializer(m.Value)))
                        {
                            await dispatcher(@event);
                        }
                        position = result.NextOffset;
                    }
                    else
                    {
                        cts.Cancel();
                    }
                }
        }
    }
}