using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Treefort.Azure.Messaging
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
            //_client.RetryPolicy = new RetryExponential();
            //http://social.msdn.microsoft.com/Forums/live/en-US/eddc7ba3-935f-4fed-a0fa-b585ea6c5e22/service-bus-message-pump-exceptionreceived-with-null-exception
            options.ExceptionReceived += (sender, args) =>
            {
                if (args.Exception != null)
                    OnException(sender as BrokeredMessage, (dynamic) args.Exception);
            };
            _client.OnMessageAsync(messageHandler, options);
        }

        private void OnException(BrokeredMessage message, SerializationException exception)
        {
            message.DeadLetter("Failed to process", exception.Message);
        }


        private void OnException(object sender, Exception exception)
        {
            if (exception != null)
                throw exception;
        }

        public Task StopAsync()
        {
            return _client.CloseAsync();
        }
    }
}