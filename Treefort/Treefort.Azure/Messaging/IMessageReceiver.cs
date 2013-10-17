using System;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Treefort.Azure.Messaging
{
    public interface IMessageReceiver
    {
        void Start(Func<BrokeredMessage, Task> messageHandler);

        Task StopAsync();
    }
}