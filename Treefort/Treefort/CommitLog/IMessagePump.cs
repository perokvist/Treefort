using System;
using System.Threading.Tasks;

namespace Treefort.CommitLog
{
    public interface IMessagePump
    {
        void OnMessage(Func<Message, Task> messageAction);
    }
}