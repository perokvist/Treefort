using System.Collections.Generic;

namespace Treefort.CommitLog
{
    public interface IMessageResult
    {
        bool HasMessages();

        IList<Message> Messages { get;  }

        long NextOffset { get; }
    }
}