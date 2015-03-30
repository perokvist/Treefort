using System;
using System.Threading;
using System.Threading.Tasks;
using Treefort.Events;

namespace Treefort.CommitLog
{
    public class StreamProcessor
    {
        private readonly ICommitLogClient _client;
        private readonly string _stream;
        private readonly ICheckpointWriter _checkpointWriter;
        private readonly Func<byte[], IEvent> _deserializer;
        private readonly Action<string> _infoLogger;

        public StreamProcessor(ICommitLogClient client, string stream,
            ICheckpointWriter checkpointWriter, Func<byte[], IEvent> deserializer, Action<string> infoLogger)
        {
            _client = client;
            _stream = stream;
            _checkpointWriter = checkpointWriter;
            _deserializer = deserializer;
            _infoLogger = infoLogger;
        }

        public void Run(CancellationToken token, Action<IEvent> dispatcher)
        {
            Func<IEvent, Task> adapter = @event =>
            {
                dispatcher(@event);
                return Task.FromResult(0);
            };

            Run(token, adapter);
        }

        public async void Run(CancellationToken token, Func<IEvent, Task> dispatcher)
        {
            var current = _checkpointWriter.GetOrInitPosition();
            var reader = await _client.GetMessageReaderAsync(_stream);

            Task.Factory.StartNew(() => FetchMessages(token, dispatcher, reader, current)
            , TaskCreationOptions.LongRunning);
        }

        private async void FetchMessages(CancellationToken token, Func<IEvent, Task> dispatcher, IMessageReader reader, long from)
        {
            var current = from;
            while (!token.IsCancellationRequested)
            {
                var result = await reader.GetMessagesAsync(token, current, 500);
                if (result.HasMessages())
                {
                    foreach (var message in result.Messages)
                    {
                        _infoLogger("Got message");
                        await dispatcher(_deserializer(message.Value));
                    }
                    current = result.NextOffset;
                    _checkpointWriter.Update(current);
                }
            }
        }
    }
}