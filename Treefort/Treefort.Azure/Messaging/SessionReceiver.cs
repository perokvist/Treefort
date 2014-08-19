using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Treefort.Azure.Messaging
{
    public static class SessionReceiver
    {
        public static async Task StartSessionAsync(Func<Task<MessageSession>> clientAccept,  Func<BrokeredMessage, Task> messageHandler, OnMessageOptions options, CancellationToken token)
        {
            if (!token.IsCancellationRequested)
            {
                try
                {
                    var session = await clientAccept();
                    session.OnMessageAsync(messageHandler, options);
                }
                catch (TimeoutException)
                {
                }

                await Task.Delay(300, token);
                await Task.Run(() => StartSessionAsync(clientAccept, messageHandler, options, token), token);
            }
        } 
    }
}