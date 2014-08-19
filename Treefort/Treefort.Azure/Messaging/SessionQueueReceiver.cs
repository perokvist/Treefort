using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Treefort.Infrastructure;

namespace Treefort.Azure.Messaging
{
    public class SessionQueueReceiver : IMessageReceiver
    {
        private readonly Action<string> _logger;
        private readonly QueueClient _client;
        private CancellationTokenSource _cancelationTokenSoucre;

        public SessionQueueReceiver(string connectionString, string path) : this(connectionString, path, s => { })
        {}

        public SessionQueueReceiver(string connectionString, string path, Action<string> logger)
        {
            _logger = logger;
            _client = QueueClient.CreateFromConnectionString(connectionString, path, ReceiveMode.PeekLock);
        }

        public void Start(Func<BrokeredMessage, Task> messageHandler)
        {
            _logger("Process starting...");
            _cancelationTokenSoucre = new CancellationTokenSource();
            SessionReceiver.StartSessionAsync(() => _client.AcceptMessageSessionAsync(TimeSpan.FromMinutes(1)) ,messageHandler, _logger ,new OnMessageOptions() {MaxConcurrentCalls = 1}, _cancelationTokenSoucre.Token);
        }
        
        public Task StopAsync()
        {
            _logger("Process stopping...");
            _cancelationTokenSoucre.Cancel();
            return _client.CloseAsync();
        }
    }
}
