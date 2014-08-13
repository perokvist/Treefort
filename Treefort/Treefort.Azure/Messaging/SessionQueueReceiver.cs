using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Treefort.Azure.Messaging
{
    class SessionQueueReceiver : IMessageReceiver
    {
        private readonly QueueClient _client;

        public SessionQueueReceiver(string connectionString, string path)
        {
            _client = QueueClient.CreateFromConnectionString(connectionString, path, ReceiveMode.PeekLock);
        }


        public void Start(Func<BrokeredMessage, Task> messageHandler)
        {
            StartSessionAsync(messageHandler, new OnMessageOptions() {MaxConcurrentCalls = 1});
        }

        private async Task StartSessionAsync(Func<BrokeredMessage, Task> messageHandler, OnMessageOptions options)
        {
            var s = await _client.AcceptMessageSessionAsync(TimeSpan.FromMinutes(1));
            s.OnMessageAsync(messageHandler, options);
            await Task.Delay(300);
            await Task.Factory.StartNew(() => StartSessionAsync(messageHandler, options));
        }

        public Task StopAsync()
        {
            return _client.CloseAsync();
        }
    }
}
