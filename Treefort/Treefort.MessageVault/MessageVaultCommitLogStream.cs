using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MessageVault;
using MessageVault.Api;

namespace Treefort.MessageVault
{
    public class MessageVaultCommitLogStream : ICommitLogStream<PostMessagesResponse, MessageResult, Message, MessageToWrite>
    {
        private readonly IClient _client;
        private readonly string _stream;

        public MessageVaultCommitLogStream(IClient client, string stream)
        {
            _client = client;
            _stream = stream;
            ReaderFactory = new Lazy<Task<MessageReader>>(() => _client.GetMessageReaderAsync(_stream));
        }

        public Task<PostMessagesResponse> PostMessagesAsync(ICollection<MessageToWrite> messages)
        {
            return _client.PostMessagesAsync(_stream, messages);
        }

        public async Task<MessageResult> GetMessagesAsync(CancellationToken ct, long start, int limit)
        {
            var reader = await ReaderFactory.Value;
            return await reader.GetMessagesAsync(ct, start, limit);
        }

        public async Task<ConcurrentQueue<Message>> ToQueueAsync(CancellationToken ct, long start, int bufferSize, int cacheSize)
        {
            var reader = await ReaderFactory.Value;
            return reader.Subscribe(ct, start, bufferSize, cacheSize);
        }

        private Lazy<Task<MessageReader>> ReaderFactory { get; set; }
    }
}