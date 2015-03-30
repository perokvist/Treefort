using System;
using System.Threading;
using System.Threading.Tasks;

namespace Treefort.CommitLog
{
    public interface IMessageReader : IDisposable
    {
        long GetPosition();
        IMessageResult ReadMessages(long from, long till, int maxCount);

        IMessagePump CreatePump(CancellationToken ct, long start, int bufferSize, int cacheSize);

        Task<IMessageResult> GetMessagesAsync(CancellationToken ct, long start,
            int limit);
    }
}