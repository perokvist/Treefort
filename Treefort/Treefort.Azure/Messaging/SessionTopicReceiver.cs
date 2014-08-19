using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Treefort.Azure.Messaging
{
    public class SessionTopicReceiver : IMessageReceiver
    {
        private readonly Action<string> _logger;
        private readonly SubscriptionClient _client;
        private CancellationTokenSource _cancelationTokenSoucre;

        public SessionTopicReceiver(string topic, string subscription)
            : this(topic, subscription, s => { })
        {}

        public SessionTopicReceiver(string topic, string subscription, Action<string> logger)
        {
            _logger = logger;
            var messagingFactory = MessagingFactory.Create();
            _client = messagingFactory.CreateSubscriptionClient(topic, subscription);
        }
        
        public void Start(Func<BrokeredMessage, Task> messageHandler)
        {
            _logger("Process starting...");
            _cancelationTokenSoucre = new CancellationTokenSource();
            SessionReceiver.StartSessionAsync(() => _client.AcceptMessageSessionAsync(TimeSpan.FromMinutes(1)), messageHandler, _logger, new OnMessageOptions() { MaxConcurrentCalls = 1 }, _cancelationTokenSoucre.Token);
        }

        public Task StopAsync()
        {
            _logger("Process stopping...");
            _cancelationTokenSoucre.Cancel();
            return _client.CloseAsync();
        }
    }
}