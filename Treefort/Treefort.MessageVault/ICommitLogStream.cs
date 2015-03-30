using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Treefort.MessageVault
{
    public interface ICommitLogStream<TResponse, TResult, TMessage, TMessageToWrite>
    {
        Task<TResponse> PostMessagesAsync(ICollection<TMessageToWrite> messages);

        Task<TResult> GetMessagesAsync(CancellationToken ct, long start, int limit);

        Task<ConcurrentQueue<TMessage>> ToQueueAsync(
            CancellationToken ct,
            long start,
            int bufferSize,
            int cacheSize);
    }
}