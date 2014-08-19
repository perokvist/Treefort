using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Treefort.Azure.Messaging
{
    public class SessionTopicReceiver : IMessageReceiver
    {
        private readonly SubscriptionClient _client;
        private CancellationTokenSource _cancelationTokenSoucre;

        public SessionTopicReceiver(string topic, string subscription)
        {
            var messagingFactory = MessagingFactory.Create();
            _client = messagingFactory.CreateSubscriptionClient(topic, subscription);
        }
        public void Start(Func<BrokeredMessage, Task> messageHandler)
        {
            _cancelationTokenSoucre = new CancellationTokenSource();
            SessionReceiver.StartSessionAsync(() => _client.AcceptMessageSessionAsync(TimeSpan.FromMinutes(1)), messageHandler, new OnMessageOptions() { MaxConcurrentCalls = 1 }, _cancelationTokenSoucre.Token);
        }

        public Task StopAsync()
        {
            _cancelationTokenSoucre.Cancel();
            return _client.CloseAsync();
        }
    }
}