using System;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Treefort.Azure
{
    public class QueueReceiver : IMessageReceiver
    {
        private readonly QueueClient _client;

        public QueueReceiver(string connectionString, string path)
        {
            _client = QueueClient.CreateFromConnectionString(connectionString, path, ReceiveMode.PeekLock);
        }

        public void Start(Func<BrokeredMessage, Task> messageHandler)
        {
            var options = new OnMessageOptions {MaxConcurrentCalls = 1};
            _client.OnMessageAsync(messageHandler, options);
        }

        public Task StopAsync()
        {
            return _client.CloseAsync();
        }
    }
}