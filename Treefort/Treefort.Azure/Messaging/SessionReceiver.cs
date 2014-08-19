using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Treefort.Azure.Messaging
{
    public static class SessionReceiver
    {
        public static async Task StartSessionAsync(Func<Task<MessageSession>> clientAccept,  Func<BrokeredMessage, Task> messageHandler, Action<string> logger,  OnMessageOptions options, CancellationToken token)
        {
            if (!token.IsCancellationRequested)
            {
                try
                {
                    var session = await clientAccept();
                    logger(string.Format("Session accepted: {0} on {1}", session.SessionId, Thread.CurrentThread.ManagedThreadId));
                    session.OnMessageAsync(messageHandler, options);
                }
                catch (TimeoutException ex)
                {
                    logger(string.Format("Session timeout: {0}", ex.Message));
                }

                await Task.Delay(300, token);
                await Task.Run(() => StartSessionAsync(clientAccept, messageHandler, logger, options, token), token);
            }
        } 
    }
}