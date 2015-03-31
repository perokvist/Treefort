using System;
using System.Threading;
using System.Threading.Tasks;
using Treefort.Events;

namespace Treefort.CommitLog
{
    public class QueueStreamProcessor
    {
        private readonly ICommitLogClient _client;
        private readonly string _stream;
        private readonly Func<byte[], IEvent> _deserializer;

        public QueueStreamProcessor(ICommitLogClient client, string stream, Func<byte[], IEvent> deserializer)
        {
            _client = client;
            _stream = stream;
            _deserializer = deserializer;
        }

        public async void Run(CancellationToken token, Func<IEvent, Task> dispatcher)
        {
            var reader = await _client.GetMessageReaderAsync(_stream);
            var pump = reader.CreatePump(token, 0, 100000, 0);
            pump.OnMessage(message => dispatcher(_deserializer(message.Value)));
        }
    }
}