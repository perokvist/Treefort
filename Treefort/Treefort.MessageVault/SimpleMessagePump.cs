using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Treefort.CommitLog;
using Message = MessageVault.Message;

namespace Treefort.MessageVault
{
    public class SimpleMessagePump : IMessagePump
    {
        private readonly ConcurrentQueue<Message> _queue;
        private readonly CancellationToken _token;
        private readonly Action<string> _infoLogger;

        public SimpleMessagePump(ConcurrentQueue<Message> queue, CancellationToken token, Action<string> infoLogger)
        {
            _queue = queue;
            _token = token;
            _infoLogger = infoLogger;
        }

        public void OnMessage(Func<CommitLog.Message, Task> messageAction)
        {
            Task.Factory.StartNew(async () =>
            {
                while (!_token.IsCancellationRequested)
                {
                    Message m;
                    
                    if (!_queue.TryPeek(out m)) continue;
                    if (m == null) continue;
                    _infoLogger(string.Format("Pumping message id : {0} - key : {1}", m.Id, m.Key)); 
                    await messageAction(new CommitLog.Message(new MessageIdAdapter(m.Id), m.Key, m.Value));
                    _queue.TryDequeue(out m);
                    //TODO possilbe to add delay
                }
            }, _token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
    }
}