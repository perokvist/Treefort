using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Treefort.CommitLog
{
    public interface ICommitLogClient : IDisposable
    {
        Task<PostMessagesResponse> PostMessagesAsync(string stream, ICollection<MessageToWrite> messages);
        Task<IMessageReader> GetMessageReaderAsync(string stream);
    }
}