using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Treefort.CommitLog;

namespace Treefort.MessageVault
{
    public class MessageVaultClientAdapter : ICommitLogClient
    {
        private readonly global::MessageVault.Api.IClient _client;
        private readonly Action<string> _infoLogger;

        public MessageVaultClientAdapter(global::MessageVault.Api.IClient client) : this(client, s => { })
        {}

        public MessageVaultClientAdapter(global::MessageVault.Api.IClient client, Action<string> infoLogger)
        {
            _client = client;
            _infoLogger = infoLogger;
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public async Task<PostMessagesResponse> PostMessagesAsync(string stream, ICollection<MessageToWrite> messages)
        {
            var result = await _client.PostMessagesAsync(stream, messages.Select(x => new global::MessageVault.MessageToWrite(x.Key, x.Value)).ToList());
            return new PostMessagesResponse() { Position = result.Position };
        }

        public async Task<IMessageReader> GetMessageReaderAsync(string stream)
        {
            var result = await _client.GetMessageReaderAsync(stream);
            return new MessageVaultMessageReaderAdapter(result, _infoLogger);
        }

    }
}