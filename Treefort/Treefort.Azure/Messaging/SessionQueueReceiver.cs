using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Treefort.Azure.Messaging
{
    public class SessionQueueReceiver : IMessageReceiver
    {
        private readonly QueueClient _client;
        private CancellationTokenSource _cancelationTokenSoucre;

        public SessionQueueReceiver(string connectionString, string path)
        {
            _client = QueueClient.CreateFromConnectionString(connectionString, path, ReceiveMode.PeekLock);
        }

        public void Start(Func<BrokeredMessage, Task> messageHandler)
        {
            _cancelationTokenSoucre = new CancellationTokenSource();
            SessionReceiver.StartSessionAsync(() => _client.AcceptMessageSessionAsync(TimeSpan.FromMinutes(1)) ,messageHandler, new OnMessageOptions() {MaxConcurrentCalls = 1}, _cancelationTokenSoucre.Token);
        }
        
        public Task StopAsync()
        {
            _cancelationTokenSoucre.Cancel();
            return _client.CloseAsync();
        }
    }
}
