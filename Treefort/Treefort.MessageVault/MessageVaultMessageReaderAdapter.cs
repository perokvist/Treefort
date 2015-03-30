using System;
using System.Threading;
using System.Threading.Tasks;
using MessageVault;
using Treefort.CommitLog;

namespace Treefort.MessageVault
{
    public class MessageVaultMessageReaderAdapter : IMessageReader
    {
        private readonly MessageReader _reader;
        private readonly Action<string> _infoLogger;

        public MessageVaultMessageReaderAdapter(MessageReader reader)
            : this(reader, s => { })
        {}

        public MessageVaultMessageReaderAdapter(global::MessageVault.MessageReader reader, Action<string> infoLogger)
        {
            _reader = reader;
            _infoLogger = infoLogger;
        }

        public void Dispose()
        {
            _infoLogger(string.Format("Disposing {0}", this.GetType()));
            _reader.Dispose();
        }

        public long GetPosition()
        {
            return _reader.GetPosition();
        }

        public IMessageResult ReadMessages(long @from, long till, int maxCount)
        {
            return new MessageResultAdapter(_reader.ReadMessages(@from, till, maxCount));
        }

        public IMessagePump CreatePump(CancellationToken ct, long start, int bufferSize, int cacheSize)
        {
            var queue = _reader.Subscribe(ct, start, bufferSize, cacheSize);
            return new SimpleMessagePump(queue, ct, _infoLogger);
        }

        public async Task<IMessageResult> GetMessagesAsync(CancellationToken ct, long start, int limit)
        {
            var result = await _reader.GetMessagesAsync(ct, start, limit);
            return new MessageResultAdapter(result);
        }
    }
}