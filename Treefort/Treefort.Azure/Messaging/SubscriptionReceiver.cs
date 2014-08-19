using System;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Treefort.Azure.Messaging
{
    public class SubscriptionReceiver : IMessageReceiver
    {
        private readonly SubscriptionClient _client;
        public SubscriptionReceiver(string topic, string subscription)
        {
            var messagingFactory = MessagingFactory.Create();
            _client = messagingFactory.CreateSubscriptionClient(topic, subscription);
        }

        public async void Start(Func<BrokeredMessage, Task> messageHandler)
        {
            var options = new OnMessageOptions {MaxConcurrentCalls = 1};
            await Task.Factory.StartNew(() => _client.OnMessageAsync(messageHandler, options));
            _client.OnMessageAsync(messageHandler, options);
        }

        public Task StopAsync()
        {
            return _client.CloseAsync();
        }
    }
}